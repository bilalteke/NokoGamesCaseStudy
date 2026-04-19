using System.Collections;
using DG.Tweening;
using NokoGames.Core;
using NokoGames.Machines;
using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.Transformer
{
    public class AssetTransformer : BaseMachine
    {
        [Header("References")]
        [SerializeField] private TransformerInputStorage _inputStorage;
        [SerializeField] private TransformerOutputStorage _outputStorage;

        [Header("Settings")]
        [SerializeField] private StackItem _outputItemPrefab;
        [SerializeField] private float _transformDuration = 3f;

        [Header("Points")]
        [SerializeField] private Transform _machineHole;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private ParticleSystem _transformFX;

        protected override bool IsReady() => !_inputStorage.IsEmpty && !_outputStorage.IsFull;

        protected override IEnumerator Execute()
        {
            StackItem rawItem = _inputStorage.TryRemove();
            if (rawItem == null) yield break;

            yield return StartCoroutine(AnimateIntoMachine(rawItem));
            DestroyRawItem(rawItem);

            yield return new WaitForSeconds(_transformDuration);

            SpawnOutputItem();
        }

        private IEnumerator AnimateIntoMachine(StackItem item)
        {
            bool done = false;
            Vector3 holePos = _machineHole != null ? _machineHole.position : transform.position;

            item.transform.SetParent(null);
            item.SetAnimating(true);

            Vector3 originalScale = item.transform.localScale;

            DOTween.Sequence()
                .Append(item.transform.DOMove(holePos, 0.3f).SetEase(Ease.OutCubic))
                .Join(item.transform.DOScale(originalScale * 0.6f, 0.2f).SetEase(Ease.OutQuad))
                .Append(item.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack))
                .AppendCallback(() => done = true);

            yield return new WaitUntil(() => done);
        }

        private void DestroyRawItem(StackItem item)
        {
            if (_transformFX != null) { _transformFX.transform.position = item.transform.position; _transformFX.Play(); }
            Destroy(item.gameObject);
        }

        private void SpawnOutputItem()
        {
            Vector3 pos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            StackItem output = Instantiate(_outputItemPrefab, pos, _outputItemPrefab.transform.rotation);

            if (!_outputStorage.TryAdd(output))
            {
                Debug.LogWarning("[AssetTransformer] Output storage full.");
                Destroy(output.gameObject);
            }
        }
    }
}
using System.Collections;
using DG.Tweening;
using NokoGames.Core;
using NokoGames.Machines;
using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.Spawner
{
    public class SpawnerMachine : BaseMachine
    {
        [Header("References")]
        [SerializeField] private SpawnerOutputStorage _outputStorage;

        [Header("Prefabs")]
        [SerializeField] private GameObject _longBrickPrefab;
        [SerializeField] private StackItem _brickPrefab;

        [Header("Points")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _splitPoint;
        [SerializeField] private Transform _edgePoint;   // makinenin ucu

        [Header("Settings")]
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private float _brickSpacing = 0.6f;
        [SerializeField] private float _dropDelay = 0.2f;
        [SerializeField] private int _splitCount = 3;

        protected override bool IsReady() =>
            _outputStorage.GetEmptySlotCount() >= _splitCount;

        protected override IEnumerator Execute()
        {
            yield return new WaitForSeconds(_spawnInterval);
            yield return StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            yield return StartCoroutine(MoveLongBrick());
            DropToOutput(SpawnSplitBricks());
            // bekleme yok — SpawnInterval'a döner
        }

        private IEnumerator MoveLongBrick()
        {
            GameObject lb = Instantiate(_longBrickPrefab,
                _spawnPoint.position,
                _longBrickPrefab.transform.rotation);

            Vector3 originalScale = lb.transform.localScale; // prefabdan direkt al
            lb.transform.localScale = Vector3.zero;

            bool done = false;

            DOTween.Sequence()
                .Append(lb.transform.DOScale(originalScale, 0.2f)
                    .SetEase(Ease.OutBack))
                .Append(lb.transform.DOMove(_edgePoint.position, 0.35f)
                    .SetEase(Ease.OutQuad))
                .AppendInterval(0.1f)
                .Append(lb.transform.DOMove(_splitPoint.position, 0.5f)
                    .SetEase(Ease.Linear))
                .AppendCallback(() =>
                {
                    Destroy(lb);
                    done = true;
                });

            yield return new WaitUntil(() => done);
        }

        private StackItem[] SpawnSplitBricks()
        {
            StackItem[] bricks = new StackItem[_splitCount];

            for (int i = 0; i < _splitCount; i++)
            {
                float xOffset = (i - (_splitCount - 1) * 0.5f) * _brickSpacing;
                Vector3 pos = _splitPoint.position + _splitPoint.right * xOffset;

                StackItem brick = Instantiate(_brickPrefab, pos, _brickPrefab.transform.rotation);
                brick.transform.localScale = brick.OriginalScale;
                brick.SetAnimating(true);

                bricks[i] = brick;
            }

            return bricks;
        }

        private void DropToOutput(StackItem[] bricks)
        {
            for (int i = 0; i < bricks.Length; i++)
            {
                int idx = _outputStorage.GetEmptySlotIndex();
                if (idx == -1) { Destroy(bricks[i].gameObject); continue; }
                if (!_outputStorage.TryAdd(bricks[i])) { Destroy(bricks[i].gameObject); continue; }

                Vector3 localPos = _outputStorage.GetLocalGridPosition(idx);
                Vector3 worldPos = _outputStorage.GridOrigin.TransformPoint(localPos);

                StackItem brick = bricks[i];
                float delay = i * _dropDelay;

                DOTween.Sequence()
                    .AppendInterval(delay)
                    .Append(brick.transform.PopAndReturn(brick.OriginalScale))  // <-- TweenExtensions
                    .Append(brick.transform.DOMove(worldPos, 0.25f).SetEase(Ease.OutCubic))
                    .AppendCallback(() =>
                    {
                        brick.transform.SetParent(_outputStorage.GridOrigin);
                        brick.transform.localPosition = localPos;
                        brick.transform.localRotation = brick.OriginalRotation;
                        brick.transform.localScale = brick.OriginalScale;
                        brick.SetAnimating(false);
                    });
            }
        }
    }
}
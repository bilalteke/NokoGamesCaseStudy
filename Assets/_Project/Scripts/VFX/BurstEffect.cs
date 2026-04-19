using DG.Tweening;
using UnityEngine;

namespace NokoGames.VFX
{
    public class BurstEffect : MonoBehaviour
    {
        [Header("Burst Settings")]
        [SerializeField] private GameObject[] _piecePrefabs;
        [SerializeField] private int _pieceCount = 6;
        [SerializeField] private float _burstRadius = 0.8f;
        [SerializeField] private float _pieceDuration = 0.4f;
        [SerializeField] private float _popScale = 1.3f;

        public void Play(System.Action onComplete = null)
        {
            transform.DOKill();
            transform.DOScale(transform.localScale * _popScale, 0.1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    SpawnPieces();

                    transform.DOScale(Vector3.zero, 0.1f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => onComplete?.Invoke());
                });
        }

        private void SpawnPieces()
        {
            if (_piecePrefabs == null || _piecePrefabs.Length == 0) return;

            for (int i = 0; i < _pieceCount; i++)
            {
                GameObject piece = Instantiate(
                    _piecePrefabs[Random.Range(0, _piecePrefabs.Length)],
                    transform.position, Random.rotation);

                Vector3 dir = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

                float radius = Random.Range(_burstRadius * 0.5f, _burstRadius);

                DOTween.Sequence()
                    .Append(piece.transform.DOMove(transform.position + dir * radius, _pieceDuration).SetEase(Ease.OutQuad))
                    .Join(piece.transform.DOScale(Vector3.zero, _pieceDuration).SetEase(Ease.InQuad))
                    .Join(piece.transform.DORotate(Random.insideUnitSphere * 360f, _pieceDuration).SetEase(Ease.OutQuad))
                    .OnComplete(() => Destroy(piece));
            }
        }
    }
}
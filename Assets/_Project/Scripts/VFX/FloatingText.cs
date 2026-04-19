using DG.Tweening;
using TMPro;
using UnityEngine;

namespace NokoGames.VFX
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _riseHeight = 1.5f;

        public void Play(string message, Vector3 worldPos, System.Action onComplete = null)
        {
            transform.position = worldPos;
            transform.localScale = Vector3.zero;
            _text.text = message;
            _text.alpha = 1f;

            DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack))
                .Append(transform.DOMoveY(worldPos.y + _riseHeight, _duration).SetEase(Ease.OutQuad))
                .Join(_text.DOFade(0f, _duration).SetEase(Ease.InQuad))
                .AppendCallback(() => onComplete?.Invoke()); // Destroy yerine pool'a dön
        }
    }
}
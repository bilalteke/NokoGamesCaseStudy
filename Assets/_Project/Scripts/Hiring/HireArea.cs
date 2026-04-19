using DG.Tweening;
using NokoGames.AI;
using NokoGames.Core;
using NokoGames.Currency;
using UnityEngine;
using UnityEngine.UI;

namespace NokoGames.Hiring
{
    public class HireArea : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _fillDuration = 2f;
        [SerializeField] private int _hireCost = 100;

        [Header("AI")]
        [SerializeField] private AIController _aiController;

        [Header("UI")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private CanvasGroup _hireUIRoot;

        private bool _isHired = false;
        private bool _isFilling = false;

        private void Awake()
        {
            _fillImage.fillAmount = 0f;

            if (_aiController != null)
                _aiController.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isHired) return;
            if (!other.TryGetComponent(out Player.PlayerMovement _)) return;

            StartFilling();
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isHired) return;
            if (!other.TryGetComponent(out Player.PlayerMovement _)) return;

            CancelFilling();
        }

        private void StartFilling()
        {
            if (_isFilling) return;
            _isFilling = true;

            _fillImage.DOFillAmount(1f, _fillDuration)
                .SetEase(Ease.Linear)
                .OnComplete(OnFillComplete);
        }

        private void CancelFilling()
        {
            _isFilling = false;
            _fillImage.DOKill();
            _fillImage.DOFillAmount(0f, 0.3f).SetEase(Ease.OutQuad);
        }

        private void OnFillComplete()
        {
            if (!CurrencyManager.Instance.Spend(_hireCost))
            {
                CancelFilling();
                return;
            }

            _isHired = true;
            ActivateAI();
        }

        private void ActivateAI()
        {
            if (_aiController == null) return;

            _aiController.enabled = true;

            // AI spawn pop efekti
            _aiController.transform.PopAndReturn(
                _aiController.transform.localScale,
                popScale: 1.3f,
                duration: 0.1f
            );

            DOVirtual.DelayedCall(1f, () =>
            {
                _hireUIRoot.transform.DOScale(Vector3.zero, 0.4f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _hireUIRoot.gameObject.SetActive(false);
                    GetComponent<Collider>().enabled = false;
                });
            });
        }
    }
}
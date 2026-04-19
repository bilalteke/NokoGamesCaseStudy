using DG.Tweening;
using TMPro;
using UnityEngine;

namespace NokoGames.Currency
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int _startingAmount = 0;
        [SerializeField] private float _countDuration = 0.5f;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _currencyText;

        public int CurrentAmount { get; private set; }

        public event System.Action<int> OnCurrencyChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            CurrentAmount = _startingAmount;
            UpdateUI(CurrentAmount);
        }

        public void Add(int amount)
        {
            int from = CurrentAmount;
            CurrentAmount += amount;
            AnimateUI(from, CurrentAmount);
            OnCurrencyChanged?.Invoke(CurrentAmount);
        }

        public bool Spend(int amount)
        {
            if (CurrentAmount < amount) return false;
            int from = CurrentAmount;
            CurrentAmount -= amount;
            AnimateUI(from, CurrentAmount);
            OnCurrencyChanged?.Invoke(CurrentAmount);
            return true;
        }

        private void AnimateUI(int from, int to)
        {
            DOTween.To(() => from, x => _currencyText.text = x.ToString(), to, _countDuration)
                .SetEase(Ease.OutQuad);
        }

        private void UpdateUI(int value)
        {
            if (_currencyText != null)
                _currencyText.text = value.ToString();
        }
    }
}
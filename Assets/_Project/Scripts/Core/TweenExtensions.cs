using DG.Tweening;
using NokoGames.VFX;
using UnityEngine;

namespace NokoGames.Core
{
    public static class TweenExtensions
    {
        /// <summary>
        /// Hedefe git, scale sıfırla, tamamlanınca callback.
        /// AnimateIntoMachine + SellPoint için.
        /// </summary>
        public static Sequence FlyAndVanish(this Transform t, Vector3 target,
            float duration = 0.35f, float arcHeight = 0f, System.Action onComplete = null)
        {
            t.DOKill();

            BurstEffect burst = t.GetComponent<BurstEffect>();

            Sequence seq = DOTween.Sequence();

            if (arcHeight > 0f)
                seq.Append(t.DOJump(target, arcHeight, 1, duration).SetEase(Ease.InQuad));
            else
                seq.Append(t.DOMove(target, duration).SetEase(Ease.InBack));

            seq.Join(t.DOScale(t.localScale * 0.3f, duration * 1.5f).SetEase(Ease.InQuad));

            seq.AppendCallback(() =>
            {
                if (burst != null)
                    burst.Play(onComplete);
                else
                    onComplete?.Invoke();
            });

            return seq;
        }

        /// <summary>
        /// Slot'a git, parent'a bağla, pozisyonu kilitle.
        /// GridStorageBase.MoveToSlot için.
        /// </summary>
        public static Sequence FlyToSlot(this Transform t, Vector3 worldPos, Vector3 localPos,
            Transform parent, Vector3 originalScale, Quaternion originalRotation,
            float duration = 0.3f, System.Action onComplete = null)
        {
            t.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(t.DOMove(worldPos, duration).SetEase(Ease.OutBack));
            seq.Join(t.DOScale(originalScale, duration).SetEase(Ease.OutBack));
            seq.Join(t.DORotateQuaternion(parent.rotation, duration));
            seq.AppendCallback(() =>
            {
                t.SetParent(parent);
                t.localPosition = localPos;
                t.localRotation = originalRotation;
                t.localScale = originalScale;
                onComplete?.Invoke();
            });

            return seq;
        }

        /// <summary>
        /// Spawn noktasında küçükten büyüye aç, slot'a git.
        /// TransformerOutputStorage + SpawnerMachine için.
        /// </summary>
        public static Sequence PopAndFlyToSlot(this Transform t, Vector3 worldPos, Vector3 localPos,
            Transform parent, Vector3 originalScale, Quaternion originalRotation,
            float duration = 0.35f, System.Action onComplete = null)
        {
            t.DOKill();
            t.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            seq.Append(t.DOScale(originalScale * 0.6f, 0.15f).SetEase(Ease.OutQuad));
            seq.Append(t.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack));
            seq.Join(t.DOMove(worldPos, duration).SetEase(Ease.OutCubic));
            seq.AppendCallback(() =>
            {
                t.SetParent(parent);
                t.localPosition = localPos;
                t.localRotation = originalRotation;
                t.localScale = originalScale;
                onComplete?.Invoke();
            });

            return seq;
        }

        public static Sequence PopAndReturn(this Transform t, Vector3 originalScale,
            float popScale = 1.2f, float duration = 0.1f, System.Action onComplete = null)
        {
            return DOTween.Sequence()
                .Append(t.DOScale(originalScale * popScale, duration).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale, duration).SetEase(Ease.InQuad))
                .AppendCallback(() => onComplete?.Invoke());
        }
    }
}
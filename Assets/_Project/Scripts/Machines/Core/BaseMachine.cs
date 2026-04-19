using System.Collections;
using UnityEngine;

namespace NokoGames.Machines
{
    /// <summary>
    /// Tüm makineler için ortak pipeline.
    /// Template Method pattern: IsReady() ve Execute() subclass'ta implemente edilir.
    /// </summary>
    public abstract class BaseMachine : MonoBehaviour
    {
        [SerializeField] protected MachineAnimator _animator;

        protected virtual void Start()
        {
            _animator.SetActive(false);
            StartCoroutine(ProcessRoutine());
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            _animator.SetActive(false);
        }

        private IEnumerator ProcessRoutine()
        {
            while (true)
            {
                yield return WaitForReadyState();
                _animator.SetActive(true);
                yield return Execute();
                _animator.SetActive(IsReady());
            }
        }

        private IEnumerator WaitForReadyState()
        {
            if (IsReady()) yield break;
            _animator.SetActive(false);
            yield return new WaitUntil(IsReady);
        }

        /// <summary>Makinenin çalışmaya hazır olup olmadığı.</summary>
        protected abstract bool IsReady();

        /// <summary>Makinenin bir döngüde yapacağı iş.</summary>
        protected abstract IEnumerator Execute();
    }
}
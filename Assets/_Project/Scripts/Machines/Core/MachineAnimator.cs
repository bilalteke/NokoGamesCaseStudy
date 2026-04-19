using UnityEngine;

namespace NokoGames.Machines
{
    public class MachineAnimator : MonoBehaviour
    {
        private static readonly int IsActiveHash = Animator.StringToHash("IsActive");
        [SerializeField] private Animator _animator;

        public void SetActive(bool value) => _animator.SetBool(IsActiveHash, value);
    }
}
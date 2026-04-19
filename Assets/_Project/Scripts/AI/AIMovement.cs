using UnityEngine;
using UnityEngine.AI;

namespace NokoGames.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIMovement : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            _animator.SetFloat(SpeedHash, _agent.velocity.magnitude);
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.isStopped = false;
            _agent.SetDestination(destination);
        }

        public void Stop()
        {
            _agent.isStopped = true;
        }

        public bool ReachedDestination(float stoppingDistance)
        {
            if (_agent.pathPending) return false;
            return _agent.remainingDistance <= stoppingDistance;
        }

        private void OnEnable()
        {
            _agent.isStopped = false;
        }

        private void OnDisable()
        {
            if (_agent != null && _agent.isOnNavMesh)
                _agent.isStopped = true;
        }
    }
}
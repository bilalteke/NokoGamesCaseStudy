using System.Collections.Generic;
using NokoGames.Machines;
using NokoGames.Stack;
using NokoGames.Triggers;
using UnityEngine;

namespace NokoGames.AI
{
    public class AIController : MonoBehaviour, IAIContext
    {
        [Header("Zones")]
        [SerializeField] private PickupTrigger _spawnerZone;
        [SerializeField] private DepositTrigger _transformerInputZone;
        [SerializeField] private PickupTrigger _transformerOutputZone;
        [SerializeField] private TrashCan _trashCanZone;

        public AIMovement Movement { get; private set; }
        public StackHandler StackHandler { get; private set; }

        private Dictionary<AIState, AIStateBase> _states;
        private AIStateBase _currentState;

        private void Awake()
        {
            Movement = GetComponent<AIMovement>();
            StackHandler = GetComponent<StackHandler>();

            _states = new Dictionary<AIState, AIStateBase>
            {
                {
                    AIState.Idle,
                    new AIIdleState(this, _spawnerZone, _transformerInputZone, _transformerOutputZone)
                },
                {
                    AIState.GoToPickup,
                    new AIGoToState(this, _spawnerZone, AIState.PickingUp,
                        () => _spawnerZone.Storage.IsEmpty)
                },
                {
                    AIState.PickingUp,
                    new AIPickupState(this, _spawnerZone)
                },
                {
                    AIState.GoToDeposit,
                    new AIGoToState(this, _transformerInputZone, AIState.Depositing,
                        () => StackHandler.IsEmpty || _transformerInputZone.Storage.IsFull)
                },
                {
                    AIState.Depositing,
                    new AIDepositState(this, _transformerInputZone)
                },
                {
                    AIState.GoToCollectOutput,
                    new AIGoToState(this, _transformerOutputZone, AIState.CollectingOutput,
                        () => _transformerOutputZone.Storage.IsEmpty)
                },
                {
                    AIState.CollectingOutput,
                    new AICollectOutputState(this, _transformerOutputZone)
                },
                {
                    AIState.GoToTrashCan,
                    new AIGoToState(this, _trashCanZone, AIState.Dumping,
                        () => StackHandler.IsEmpty)
                },
                {
                    AIState.Dumping,
                    new AIDumpState(this, _trashCanZone)
                }
            };
        }

        private void Start() => ChangeState(AIState.Idle);
        private void Update() => _currentState?.Update();

        public void ChangeState(AIState newState)
        {
            _currentState?.Exit();
            _currentState = _states[newState];
            _currentState.Enter();
        }
    }
}
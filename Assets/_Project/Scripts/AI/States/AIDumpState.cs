using NokoGames.Machines;
using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.AI
{
    public class AIDumpState : AIStateBase
    {
        private readonly TrashCan _trashCan;
        private float _nextDumpTime;

        public AIDumpState(IAIContext context, TrashCan trashCan) : base(context)
        {
            _trashCan = trashCan;
        }

        public override void Update()
        {
            if (Context.StackHandler.IsEmpty)
            {
                Context.ChangeState(AIState.Idle);
                return;
            }

            if (Time.time < _nextDumpTime) return;

            StackItem item = Context.StackHandler.TryRemove();
            if (item == null) return;

            _trashCan.DestroyItem(item);
            _nextDumpTime = Time.time + Context.StackHandler.DepositInterval;
        }
    }
}
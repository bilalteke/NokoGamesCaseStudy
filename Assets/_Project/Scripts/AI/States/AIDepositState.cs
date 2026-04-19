using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.AI
{
    public class AIDepositState : AIStateBase
    {
        private readonly IAIZone _zone;
        private float _nextDepositTime;

        public AIDepositState(IAIContext context, IAIZone zone) : base(context)
        {
            _zone = zone;
        }

        public override void Update()
        {
            if (Context.StackHandler.IsEmpty || _zone.Storage.IsFull)
            {
                Context.ChangeState(AIState.Idle);
                return;
            }

            if (Time.time < _nextDepositTime) return;

            ItemDefinition accepted = Context.StackHandler.FindAcceptedDefinition(_zone.Storage);
            if (accepted == null) { Context.ChangeState(AIState.Idle); return; }

            StackItem item = Context.StackHandler.TryRemoveByDefinition(accepted);
            if (item == null) return;

            bool added = _zone.Storage.TryAdd(item);
            if (!added) { Context.StackHandler.TryAdd(item); return; }

            _nextDepositTime = Time.time + Context.StackHandler.DepositInterval;
        }
    }
}
using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.AI
{
    public class AIPickupState : AIStateBase
    {
        private readonly IAIZone _zone;
        private float _nextPickupTime;

        public AIPickupState(IAIContext context, IAIZone zone) : base(context)
        {
            _zone = zone;
        }

        public override void Update()
        {
            if (Context.StackHandler.IsFull || _zone.Storage.IsEmpty)
            {
                Context.ChangeState(AIState.GoToDeposit);
                return;
            }

            if (Time.time < _nextPickupTime) return;

            StackItem item = _zone.Storage.TryRemove();
            if (item == null) return;

            bool added = Context.StackHandler.TryAdd(item);
            if (!added) { _zone.Storage.TryAdd(item); return; }

            _nextPickupTime = Time.time + Context.StackHandler.PickupInterval;
        }
    }
}
using System;

namespace NokoGames.AI
{
    public class AIGoToState : AIStateBase
    {
        private readonly IAIZone _zone;
        private readonly AIState _nextState;
        private readonly Func<bool> _shouldAbort;

        public AIGoToState(IAIContext context,
            IAIZone zone,
            AIState nextState,
            Func<bool> shouldAbort) : base(context)
        {
            _zone = zone;
            _nextState = nextState;
            _shouldAbort = shouldAbort;
        }

        public override void Enter()
        {
            if (_shouldAbort()) { Context.ChangeState(AIState.Idle); return; }
            Context.Movement.MoveTo(_zone.Destination.position);
        }

        public override void Update()
        {
            if (_shouldAbort()) { Context.ChangeState(AIState.Idle); return; }
            if (_zone.IsInsideZone) Context.ChangeState(_nextState);
        }

        public override void Exit() => Context.Movement.Stop();
    }
}
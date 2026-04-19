namespace NokoGames.AI
{
    public class AIIdleState : AIStateBase
    {
        private readonly IAIZone _pickupZone;
        private readonly IAIZone _depositZone;
        private readonly IAIZone _outputZone;

        public AIIdleState(IAIContext context,
            IAIZone pickupZone,
            IAIZone depositZone,
            IAIZone outputZone) : base(context)
        {
            _pickupZone = pickupZone;
            _depositZone = depositZone;
            _outputZone = outputZone;
        }

        public override void Update()
        {
            var stack = Context.StackHandler;

            // Null check
            if (_pickupZone?.Storage == null || _depositZone?.Storage == null) return;

            // 1) Stack'te RoofTile var → SellPoint'e git
            if (_outputZone?.Storage != null &&
                stack.FindAcceptedDefinition(_outputZone.Storage) != null)
            {
                Context.ChangeState(AIState.GoToTrashCan);
                return;
            }

            // 2) Stack'te Brick var → Transformer input'a git
            if (stack.FindAcceptedDefinition(_depositZone.Storage) != null)
            {
                Context.ChangeState(AIState.GoToDeposit);
                return;
            }

            if (!stack.IsEmpty) return;

            // 3) Transformer output'ta item var
            if (_outputZone?.Storage != null && !_outputZone.Storage.IsEmpty)
            {
                Context.ChangeState(AIState.GoToCollectOutput);
                return;
            }

            // 4) Spawner'da item var + Transformer input dolu değil
            if (!_pickupZone.Storage.IsEmpty && !_depositZone.Storage.IsFull)
            {
                Context.ChangeState(AIState.GoToPickup);
                return;
            }
        }
    }
}
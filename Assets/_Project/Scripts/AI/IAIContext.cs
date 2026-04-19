using NokoGames.Stack;

namespace NokoGames.AI
{
    public interface IAIContext
    {
        AIMovement Movement { get; }
        StackHandler StackHandler { get; }
        void ChangeState(AIState state);
    }
}
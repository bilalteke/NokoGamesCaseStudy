namespace NokoGames.AI
{
    public abstract class AIStateBase
    {
        protected IAIContext Context;

        protected AIStateBase(IAIContext context)
        {
            Context = context;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
    }
}
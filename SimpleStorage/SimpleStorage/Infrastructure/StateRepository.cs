namespace SimpleStorage.Infrastructure
{
    public class StateRepository : IStateRepository
    {
        private State state = State.Started;

        public State GetState()
        {
            return state;
        }

        public void SetState(State value)
        {
            state = value;
        }
    }
}
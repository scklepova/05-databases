namespace SimpleStorage.Infrastructure
{
    public interface IStateRepository
    {
        State GetState();
        void SetState(State state);
    }
}
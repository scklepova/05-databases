using Domain;

namespace Client
{
    public interface ISimpleStorageClient
    {
        void Put(string id, Value value);
        Value Get(string id);
    }
}
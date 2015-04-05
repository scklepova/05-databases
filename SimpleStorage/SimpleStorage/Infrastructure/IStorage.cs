using System.Collections.Generic;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public interface IStorage
    {
        IEnumerable<ValueWithId> GetAll();
        Value Get(string id);
        bool Set(string id, Value value);
        void RemoveAll();
    }
}
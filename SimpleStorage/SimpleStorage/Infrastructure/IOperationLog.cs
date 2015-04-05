using System.Collections.Generic;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public interface IOperationLog
    {
        void Add(Operation operation);
        IEnumerable<Operation> Read(int position, int count);
        void RemoveAll();
    }
}
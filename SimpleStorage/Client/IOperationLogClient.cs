using System.Collections.Generic;
using Domain;

namespace Client
{
    public interface IOperationLogClient
    {
        IEnumerable<Operation> Read(int position, int count);
    }
}
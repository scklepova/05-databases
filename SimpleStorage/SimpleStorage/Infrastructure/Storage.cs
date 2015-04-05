using System.Collections.Generic;
using System.Linq;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public class Storage : IStorage
    {
        private readonly IDictionary<string, Value> internalStorage = new Dictionary<string, Value>();
        private readonly IOperationLog operationLog;
        private readonly IComparer<Value> valueComparer;

        public Storage(IOperationLog operationLog, IComparer<Value> valueComparer)
        {
            this.operationLog = operationLog;
            this.valueComparer = valueComparer;
        }

        public IEnumerable<ValueWithId> GetAll()
        {
            lock (internalStorage)
                return internalStorage.Select(p => new ValueWithId {Id = p.Key, Value = p.Value});
        }

        public Value Get(string id)
        {
            lock (internalStorage)
                return internalStorage.ContainsKey(id) ? internalStorage[id] : null;
        }

        public bool Set(string id, Value value)
        {
            lock (internalStorage)
            {
                if (!internalStorage.ContainsKey(id) || valueComparer.Compare(internalStorage[id], value) < 0)
                {
                    internalStorage[id] = value;
                    operationLog.Add(new Operation {Id = id, Value = value});
                    return true;
                }
            }
            return false;
        }

        public void RemoveAll()
        {
            lock (internalStorage)
                internalStorage.Clear();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace SimpleStorage.Infrastructure
{
    public class OperationLog : IOperationLog
    {
        private readonly List<Operation> log = new List<Operation>();

        public void Add(Operation operation)
        {
            lock (log)
                log.Add(operation);
        }

        public IEnumerable<Operation> Read(int position, int count)
        {
            lock (log)
            {
                if (position > log.Count)
                    return Enumerable.Empty<Operation>();
                return log.GetRange(position, Math.Min(count, log.Count - position));
            }
        }

        public void RemoveAll()
        {
            lock (log)
                log.Clear();
        }
    }
}
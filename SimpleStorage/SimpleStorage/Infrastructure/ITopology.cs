using System.Collections.Generic;
using System.Net;

namespace SimpleStorage.Infrastructure
{
    public interface ITopology
    {
        IEnumerable<IPEndPoint> Replicas { get; }
    }
}
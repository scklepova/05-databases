using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SimpleStorage.Infrastructure
{
    public class Topology : ITopology
    {
        public Topology(IEnumerable<int> ports)
        {
            Replicas = ports.Select(i => new IPEndPoint(new IPAddress(new byte[] {127, 0, 0, 1}), i));
        }

        public IEnumerable<IPEndPoint> Replicas { get; private set; }
    }
}
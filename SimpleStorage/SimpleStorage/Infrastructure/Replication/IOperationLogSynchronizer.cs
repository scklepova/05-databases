using System.Threading;
using System.Threading.Tasks;

namespace SimpleStorage.Infrastructure.Replication
{
    public interface IOperationLogSynchronizer
    {
        Task Synchronize(CancellationToken cancellationToken);
    }
}
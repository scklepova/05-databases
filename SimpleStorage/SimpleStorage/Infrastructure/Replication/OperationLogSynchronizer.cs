using System.Threading;
using System.Threading.Tasks;

namespace SimpleStorage.Infrastructure.Replication
{
    public class OperationLogSynchronizer : IOperationLogSynchronizer
    {
        private readonly IConfiguration configuration;

        public OperationLogSynchronizer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task Synchronize(CancellationToken cancellationToken)
        {
            Task synchronizationTask = Task.Factory.StartNew(() => SynchronizationAction(cancellationToken), cancellationToken);
            return synchronizationTask;
        }

        private void SynchronizationAction(CancellationToken token)
        {
            if (configuration.IsMaster)
                return;
            while (true)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(1000);
            }
        }
    }
}
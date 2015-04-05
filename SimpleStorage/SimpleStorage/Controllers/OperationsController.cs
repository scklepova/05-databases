using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class OperationsController : ApiController
    {
        private readonly IOperationLog operationLog;
        private readonly IStateRepository stateRepository;

        public OperationsController(IOperationLog operationLog, IStateRepository stateRepository)
        {
            this.operationLog = operationLog;
            this.stateRepository = stateRepository;
        }

        // GET api/operations
        public IEnumerable<Operation> Get(int position, int count)
        {
            CheckState();
            return operationLog.Read(position, count);
        }

        private void CheckState()
        {
            if (stateRepository.GetState() != State.Started)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }
    }
}
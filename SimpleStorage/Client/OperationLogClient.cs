using System.Collections.Generic;
using System.Net.Http;
using Domain;

namespace Client
{
    public class OperationLogClient: IOperationLogClient
    {
        private readonly string endpoint;

        public OperationLogClient(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public IEnumerable<Operation> Read(int position, int count)
        {
            string requestUri = endpoint + string.Format("api/operations?position={0}&count={1}", position, count);
            using (var client = new HttpClient())
            using (HttpResponseMessage response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<IEnumerable<Operation>>().Result;
            }
        }
    }
}
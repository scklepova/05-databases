using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Client;
using Domain;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly IConfiguration configuration;
        private readonly IStateRepository stateRepository;
        private readonly IStorage storage;
        private readonly int quorum;

        public ValuesController(IStorage storage, IStateRepository stateRepository, IConfiguration configuration)
        {
            this.storage = storage;
            this.stateRepository = stateRepository;
            this.configuration = configuration;
            
            quorum = (configuration.OtherReplicasPorts.Count() + 1)/2 + 1;
            Console.WriteLine("Quorum = {0}", quorum);
            
        }

        private void CheckState()
        {
            if (stateRepository.GetState() != State.Started)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }

        // GET api/values/5 
        public Value Get(string id)
        {
            CheckState();
            var nodes = GetAllNodes();
            var founded = 0;
            Value result = null;
            foreach (var client in nodes.Select(node => new InternalClient(node)))
            {
                try
                {
                    var nodeResult = client.Get(id);
                    founded++;
                    if (nodeResult != null)
                    {
                        if (result == null || nodeResult.Revision >= result.Revision)
                            result = nodeResult;
                    }
                }
                catch (HttpResponseException e)
                {
                    if (e.Response.StatusCode == HttpStatusCode.NotFound)
                        founded++;
                }
                catch { }
                
                if (founded >= quorum)
                    break;
            }
            
            if (result == null || founded < quorum)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            return result;
        }

        private List<string> GetAllNodes()
        {
            var nodes = configuration.OtherReplicasPorts.Select(port => String.Format("http://127.0.0.1:{0}/", port)).ToList();
            nodes.Add(String.Format("http://127.0.0.1:{0}/", configuration.CurrentNodePort));
            return nodes;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            CheckState();
            var nodes = GetAllNodes();
            
            var writed = 0;
            foreach (var client in nodes.Select(node => new InternalClient(node)))
            {
                try
                {
                    client.Put(id, value);
                    writed++;
                }
                catch {}
                
                if (writed >= quorum)
                    break;
            }

            if (writed < quorum)
                throw  new Exception("Wasn't writed to quorum replicas");
        }
    }
}
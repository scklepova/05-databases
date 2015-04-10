﻿using System;
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
        private int quorum;

        public ValuesController(IStorage storage, IStateRepository stateRepository, IConfiguration configuration)
        {
            this.storage = storage;
            this.stateRepository = stateRepository;
            this.configuration = configuration;
            quorum = (configuration.OtherShardsPorts.Count() + 1)/2 + 1;
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
            foreach (var node in nodes)
            {
                var client = new InternalClient(node);
                var nodeResult = client.Get(id);
                if (nodeResult == null)
                    founded++;

                result = nodeResult;
                if (founded >= quorum)
                    break;
            }
            
            if (result == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }

        private List<string> GetAllNodes()
        {
            var nodes = configuration.OtherShardsPorts.Select(port => String.Format("http://127.0.0.1:{0}/", port)).ToList();
            nodes.Add(String.Format("http://127.0.0.1:{0}/", configuration.CurrentNodePort));
            return nodes;
        }

        // PUT api/values/5
        public void Put(string id, [FromBody] Value value)
        {
            CheckState();
//            storage.Set(id, value);
            var nodes = GetAllNodes();
            
            var writed = 0;
            foreach (var node in nodes)
            {
                var client = new InternalClient(node);
                client.Put(id, value);
                writed++;
                if (writed >= quorum)
                    break;
            }
        }
    }
}
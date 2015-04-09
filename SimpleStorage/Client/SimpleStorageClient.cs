using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Domain;

namespace Client
{
    public class SimpleStorageClient : ISimpleStorageClient
    {
        private readonly IEnumerable<string> endpoints;

        public SimpleStorageClient(params string[] endpoints)
        {
            if (endpoints == null || !endpoints.Any())
                throw new ArgumentException("Empty endpoints!", "endpoints");
            this.endpoints = endpoints;
        }

        public void Put(string id, Value value)
        {
            var shardNumber = GetShardNumber(id);
            var putUri = endpoints.ElementAt(shardNumber) + "api/values/" + id;
            using (var client = new HttpClient())
            using (var response = client.PutAsJsonAsync(putUri, value).Result)
                response.EnsureSuccessStatusCode();
        }

        private static int GetShardNumber(string id)
        {
            var coordinatorClient = new CoordinatorClient("http://127.0.0.1:17000/");
            var shardNumber = coordinatorClient.Get(id);
            return shardNumber;
        }

        public Value Get(string id)
        {
            var shardNumber = GetShardNumber(id);
            var requestUri = endpoints.ElementAt(shardNumber) + "api/values/" + id;
            using (var client = new HttpClient())
            using (var response = client.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsAsync<Value>().Result;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
            foreach (var endpoint in endpoints)
            {
                var putUri = endpoint + "api/values/" + id;
                using (var client = new HttpClient())
                using (var response = client.PutAsJsonAsync(putUri, value).Result)
                    if (response.IsSuccessStatusCode)
                        return;
            }
            throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }

        public Value Get(string id)
        {
            foreach (var endpoint in endpoints)
            {
                var requestUri = endpoint + "api/values/" + id;
                using (var client = new HttpClient())
                using (var response = client.GetAsync(requestUri).Result)
                {
                    //response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                        return response.Content.ReadAsAsync<Value>().Result;
                }
            }

            throw new Exception("All replicas failed");
        }
    }
}
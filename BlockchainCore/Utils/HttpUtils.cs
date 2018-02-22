using RestSharp;
using System;
using System.Threading.Tasks;

namespace BlockchainCore
{
    class HttpUtils
    {
        private const string url = "https://bittrex.com/api/v1.1/";

        public TResponse DoApiPost<TRequest, TResponse>(string path, TRequest requestObject, params Parameter[] parameters) where TResponse : new()
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(path, Method.GET);
                request.RequestFormat = DataFormat.Json;
                request.AddBody(requestObject);

                AddParameters(request, parameters);

                var resp = client.Execute<TResponse>(request);

                if (resp == null)
                {
                    Console.WriteLine($"Post request failed: response is null! Request: {request}");
                }

                if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Post request failed! Response: {resp}, Request: {request}");
                    return default(TResponse);
                }

                return resp.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine($"API call failed{e.Message + e.StackTrace}");
                throw;
            }
        }

        public TResponse DoApiGet<TResponse>(string path, params Parameter[] parameters) where TResponse : new()
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(path, Method.GET);

                AddParameters(request, parameters);

                var resp = client.Execute<TResponse>(request);

                if (resp == null)
                {
                    Console.WriteLine($"Get request failed: response is null! Request: {request}");
                }

                if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Get request failed! Response: {resp}, Request: {request}");
                    return default(TResponse);
                }

                return resp.Data;
            }
            catch (Exception e)
            {
                Console.WriteLine($"API call failed{e.Message + e.StackTrace}");
                throw;
            }
        }

        static RestRequest AddParameters(RestRequest request, Parameter[] parameters)
        {
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    request.AddParameter(param);
                }
            }

            return request;
        }
    }
}

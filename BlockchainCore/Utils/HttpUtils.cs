using RestSharp;
using System;
using System.Threading.Tasks;

namespace BlockchainCore
{
    public class HttpUtils
    {
        public static TResponse DoApiPost<TRequest, TResponse>(string url, string path, TRequest requestObject, params Parameter[] parameters) where TResponse : new()
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(path, Method.POST);
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

        public static TResponse DoApiGet<TResponse>(string url, string path, params Parameter[] parameters) where TResponse : new()
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
                    Console.WriteLine($"Get request failed! Response: {resp}, Request: {request}, Exception: {resp.ErrorException}, Error Meesage: {resp.ErrorMessage}");
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

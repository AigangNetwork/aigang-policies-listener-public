using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Aigang.Contracts.Executor.Api.Client.Requests;
using Aigang.Contracts.Executor.Api.Client.Response;
using Aigang.Policies.Domain;
using Aigang.Policies.Utils;
using log4net;
using Newtonsoft.Json;

namespace Aigang.Contracts.Executor.Api.Client
{
    public static class ContractsExecutorClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ContractsExecutorClient));
        private static readonly string _apiAddress = ConfigurationManager.GetString("ContractsExecutor:Address");
        private static readonly string _apiSecret = ConfigurationManager.GetString("ContractsExecutor:Secret");
        
        public static Policy GetPolicy(string productAddress, string productTypeId, string policyId)
        {
            GetPolicyResponse response;
            var url = _apiAddress + $"insurance/policy/{productTypeId}/{productAddress}/{policyId}";
            var jsonResponse = string.Empty;
            
            _logger.Info($"Getting policy from executor API: {url}.\n Policy id: { policyId } \n");
            
            try
            {
                var client = GetWebClient();
                jsonResponse = client.DownloadString(url);
                
                response = JsonConvert.DeserializeObject<GetPolicyResponse>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Get Policy method. Request to path: {url}.\n Response: { jsonResponse } \n", e);
                throw;
            }

            return response.Policy;
        }
        
        public static string AddPolicy(Policy policy)
        {
            AddPolicyResponse response;
            var url = _apiAddress + $"insurance/addpolicy";
            var jsonResponse = string.Empty;

            _logger.Info($"Calling Add Policy in executor API: {url}.\n Policy id: { policy.Id } \n");
            
            try
            {
                var client = GetWebClient();
                
                var request = new AddPolicyRequest
                {
                    Payout = policy.Payout,
                    Properties = policy.Properties,
                    ProductTypeId = policy.ProductTypeId,
                    ProductAddress = policy.ProductAddress,
                    PolicyId = policy.Id
                };
                
                var jsonRequest = JsonConvert.SerializeObject(request);
                
                jsonResponse = client.UploadString(url, "POST", jsonRequest);
                response = JsonConvert.DeserializeObject<AddPolicyResponse>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Add Policy method. Request to path: {url}.\n Response: { jsonResponse } \n", e);
                throw;
            }

            return response.TxId;
        }
        

        public static string ClaimPolicy(Policy policy)
        {
            ClaimResponse response;
            var url = _apiAddress + $"insurance/claim";
            var jsonResponse = string.Empty;

            _logger.Info($"Calling Claim Policy in executor API: {url}.\n Policy id: { policy.Id } \n");
            
            try
            {
                var client = GetWebClient();
                
                var request = new ClaimRequest
                {
                    ProductAddress = policy.ProductAddress, 
                    PolicyId = policy.Id, 
                    ClaimProperties = policy.ClaimProperties, 
                    ProductTypeId = policy.ProductTypeId
                };
                
                var jsonRequest = JsonConvert.SerializeObject(request);
                
                jsonResponse = client.UploadString(url, "POST", jsonRequest);
                response = JsonConvert.DeserializeObject<ClaimResponse>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in Claim method. Request to path: {url}.\n Response: { jsonResponse } \n", e);
                throw;
            }

            return response.TxId;
        }

        private static WebClient GetWebClient()
        {
            WebClient client = new WebClient();
                
            client.Headers.Add(HttpRequestHeader.Authorization, 
                Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiSecret)));
                
            client.Headers.Add("Content-Type","application/json");

            return client;
        }
    }
}
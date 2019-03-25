using System;

namespace Aigang.Contracts.Executor.Api.Client.Requests
{
    public class AddPolicyRequest : BaseRequest
    {
        public decimal Payout { get; set; }
        
        public string Properties { get; set; }
    }
}
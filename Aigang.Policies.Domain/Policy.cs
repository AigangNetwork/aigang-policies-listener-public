using System;

namespace Aigang.Policies.Domain
{
    public class Policy
    {
        public string Id { get; set; }
        
        public string ProductAddress { get; set; }
        
        public string ProductTypeId { get; set; }

        public decimal Premium { get; set; }
        
        public PolicyStatus Status { get; set; }
        
        public DateTime StartUtc { get; set; }

        public DateTime EndUtc { get; set; }

        public decimal Payout { get; set; }

        public string Properties { get; set; }
        
        public string ClaimProperties { get; set; }
        
        public DateTime CreatedUtc { get; set; }
        
        public DateTime ModifiedUtc { get; set; }
    }
}
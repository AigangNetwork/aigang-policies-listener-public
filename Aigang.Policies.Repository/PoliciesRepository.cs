using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Aigang.Policies.Domain;
using Aigang.Policies.Utils;
using Dapper;
using MySql.Data.MySqlClient;

namespace Aigang.Policies.Repository
{
    public class PoliciesRepository: IPoliciesRepository
    {
        private string _connectionString = ConfigurationManager.GetConnectionString("MySql");
        private int _cancelAgeInHours = ConfigurationManager.GetInt("CancelAgeInHours");
        
        public IDbConnection Connection => new MySqlConnection(_connectionString);

        public IEnumerable<string> GetEndedDraftPoliciesIds()
        { 
            // Subtraction now - _cancelAgeInHours 
            DateTime deadline = DateTime.UtcNow.AddHours(_cancelAgeInHours * (-1));

            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = @"SELECT policy.Id  
                                FROM `aigang.insurance`.policy
                                WHERE Status = @StatusId AND CreateUtc < @Deadline";

                dbConnection.Open();

                var data = dbConnection.Query<string>(sQuery, new { StatusId = PolicyStatus.Draft, Deadline = deadline });

                return data;
            }
        }
        
        public IEnumerable<Policy> GetDraftPolicies() 
        { 
            DateTime deadline = DateTime.UtcNow.AddHours(_cancelAgeInHours * (-1));

            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = @"SELECT Id, Status, ProductAddress, ProductTypeId, Payout, Properties   
                                FROM `aigang.insurance`.policy
                                WHERE Status = @StatusId AND CreateUtc >= @Deadline";

                dbConnection.Open();

                var data = dbConnection.Query<Policy>(sQuery, new { StatusId = PolicyStatus.Draft, Deadline = deadline });

                return data;
            }
        }
    
        public void UpdatePolicyStatus(string policyId, PolicyStatus status)
        {
            var query = @"UPDATE `aigang.insurance`.policy
                          SET ModifiedUtc = @Now, Status = @Status
                          WHERE Id = @PolicyId;";

            var now = DateTime.UtcNow;

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                dbConnection.Execute(query, new
                {
                    Now = now,
                    Status = status,
                    PolicyId = policyId
                });
            }
        }
        
        public void UpdatePolicyAfterAdd(string policyId, string txId)
        {
            var query = @"UPDATE `aigang.insurance`.policy
                          SET ModifiedUtc = @Now, Status = @Status, AddPolicyTx = @TxId
                          WHERE Id = @PolicyId;";

            var now = DateTime.UtcNow;

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                dbConnection.Execute(query, new
                {
                    Now = now,
                    Status = PolicyStatus.Paid,
                    PolicyId = policyId,
                    TxId = txId
                });
            }
        }
        
        public void UpdatePolicyAfterClaim(string policyId, string txId)
        {
            var query = @"UPDATE `aigang.insurance`.policy
                          SET ModifiedUtc = @Now, Status = @Status, ClaimTx = @TxId, PayoutUtc = @PayoutUtc
                          WHERE Id = @PolicyId;";

            var now = DateTime.UtcNow;

            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();

                dbConnection.Execute(query, new
                {
                    Now = now,
                    PayoutUtc = now,
                    Status = PolicyStatus.Paidout,
                    PolicyId = policyId,
                    TxId = txId
                });
            }
        }
        
        public IEnumerable<Policy> GetClaimablePolicies() 
        { 
            DateTime deadline = DateTime.UtcNow.AddHours(_cancelAgeInHours * (-1));

            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = @"SELECT Id, ProductAddress, ProductTypeId, ClaimProperties   
                                FROM `aigang.insurance`.policy
                                WHERE Status = @StatusId
                                LIMIT 5";

                dbConnection.Open();

                var data = dbConnection.Query<Policy>(sQuery, new { StatusId = PolicyStatus.Claimable });

                return data;
            }
        }
    }

    public interface IPoliciesRepository
    {
        IEnumerable<Policy> GetDraftPolicies();
        
        IEnumerable<Policy> GetClaimablePolicies();
        
        IEnumerable<string> GetEndedDraftPoliciesIds();
        
        void UpdatePolicyStatus(string policyId, PolicyStatus status);
        
        void UpdatePolicyAfterAdd(string policyId,string txId);

        void UpdatePolicyAfterClaim(string policyId,string txId);
    }
}
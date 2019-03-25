using System;
using Aigang.Contracts.Executor.Api.Client;
using Aigang.Policies.Domain;
using Aigang.Policies.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Playground
{
    [TestClass]
    public class TestExecutorClient
    {
        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            ConfigurationManager.Configuration = builder.Build();
        }

        [TestMethod]
        public void TestGetPolicy()
        {
            // Manually added this policy to contract
            var result = ContractsExecutorClient.GetPolicy("0x1c301Fa05f3aAb1C4479CD8E823d7b83297ac2d6", "3",
                "c36ad8690cb7462286948da2ef152a17");

            Assert.AreEqual(15, result.Premium);
        }
        
        [TestMethod]
        public void TestAddPolicy()
        {
            // Manually added this policy to contract
            var policy = new Policy
            {
                Id = "c36ad8690cb7462286948da2ef152a17",
                ProductAddress = "0x1c301Fa05f3aAb1C4479CD8E823d7b83297ac2d6",
                ProductTypeId = "3",
                Premium = 15,
                Status = PolicyStatus.Paid,
                StartUtc = DateTime.Now,
                EndUtc = DateTime.Now.AddDays(10),
                Payout = 20,
                Properties = "Fake properties",
                CreatedUtc = DateTime.Now,
                ModifiedUtc = DateTime.Now
            };
                
            var result = ContractsExecutorClient.AddPolicy(policy);
            
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void TestClaimPolicy()
        {
            // Manually added this policy to contract
            var policy = new Policy
            {
                Id = "c36ad8690cb7462286948da2ef152a17",
                ProductAddress = "0x1c301Fa05f3aAb1C4479CD8E823d7b83297ac2d6",
                ProductTypeId = "3",
                Payout = 20,
                ClaimProperties = "Fake properties",
            };
                
            var result = ContractsExecutorClient.ClaimPolicy(policy);
            
            Assert.IsNotNull(result);
        }
    }
}
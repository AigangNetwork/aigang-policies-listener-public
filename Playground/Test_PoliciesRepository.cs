using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Aigang.Policies.Utils;
using Aigang.Policies.Repository;
using Aigang.Policies.Domain;


namespace Playground
{
    [TestClass]
    public class Test_PoliciesRepository
    {
        [TestInitialize]
        public void TestInitialize()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            ConfigurationManager.Configuration = builder.Build();
        }
    }
}
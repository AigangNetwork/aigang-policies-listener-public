using System;
using System.Numerics;
using Microsoft.Extensions.Configuration;

namespace Aigang.Policies.Utils
{
    public static class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }

        public static string GetConnectionString(string key)
        {
            return Configuration.GetConnectionString(key);
        }

        public static string GetString(string key)
        {
            return Configuration[key];
        }

        public static int GetInt(string key)
        {
            return Convert.ToInt32(Configuration[key]);
        }
        
        public static BigInteger GetBigInt(string key)
        {
            return BigInteger.Parse(Configuration[key]);
        }

        public static double GetDouble(string key)
        {
            return Convert.ToDouble(Configuration[key]);
        }
    }
}
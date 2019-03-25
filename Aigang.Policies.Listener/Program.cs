using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Aigang.Policies.Utils;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

namespace Aigang.Policies.Listener
{
    class Program
    {
         // http://www.dotnetcurry.com/patterns-practices/1407/producer-consumer-pattern-dotnet-csharp
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));

        private static Thread _executorThread;
        private static Executor _executor;

        static void Main(string[] args)
        {
            LoadConfiguration();

            _logger.Info("Program started");

            try
            {
                _executorThread = new Thread(StartExecutor);
                _logger.Info("_executorThread starting");
                _executorThread.Start();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            while (true)
            {
                Thread.Sleep(6000000);
                //DO not kill console app. Windows service workaround;
            }
        }

        static void LoadConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            log4net.GlobalContext.Properties["Environment"] = environmentName;
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            var config = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logRepository, config);
            
            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            ConfigurationManager.Configuration = builder.Build();

        }

        private static void StartExecutor()
        {
            try
            {
                _executor = new Executor();
                _logger.Info("Executor starting");
                _executor.Start();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                throw;
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Process is exiting!");
            _executor.Stop();
        }
    }
}
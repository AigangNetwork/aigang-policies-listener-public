using System;
using System.Threading;
using Aigang.Contracts.Executor.Api.Client;
using Aigang.Policies.Domain;
using Aigang.Policies.Repository;
using Aigang.Policies.Utils;
using log4net;

namespace Aigang.Policies.Listener
{
   public class Executor : IDisposable
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Executor));
        private static readonly int _producerCycleS = ConfigurationManager.GetInt("CheckCycleS") * 1000;

        private readonly IPoliciesRepository _policiesRepository;

        private Timer _timer;

        private Thread _mainConsumerThread;
        private Thread _pendingConsumerThread;
        private volatile bool _isStoping = false;

        public Executor()
        {
            _policiesRepository = new PoliciesRepository();
        }

        public void Start()
        {
            _timer = new Timer(o => TimerCallbackHandler(), state: null, dueTime: 0, period: _producerCycleS);
        }

        public void Stop()
        {
            _isStoping = true;

            _logger.Info("Stopping Consumers");
        }

        private void TimerCallbackHandler()
        {
            if (!_isStoping)
            {
                var currentTime = DateTime.UtcNow;

                try
                {
                    AddPoliciesToContract();

                    ClaimPoliciesInContract();
                    
                    CancelOldDraftPolicies();
                    
                    _logger.Info($"TimerCallbackHandlerEnd - {(DateTime.UtcNow - currentTime).TotalSeconds} s");
                }
                catch (Exception e)
                {
                    _logger.Error($"TimerCallbackHandler error: {e.Message}", e);
                }
            }
        }
        
        public void CancelOldDraftPolicies()
        {
            var draftPoliciesIds = _policiesRepository.GetEndedDraftPoliciesIds();
    
            foreach (var id in draftPoliciesIds)
            {
                _policiesRepository.UpdatePolicyStatus(id, PolicyStatus.Canceled);
                _logger.Info($"Policy status updated {id} to cancel");
            }
        }
        
        public void AddPoliciesToContract()
        { 
            var draftPolicies = _policiesRepository.GetDraftPolicies();

            foreach (var policy in draftPolicies)
            {
                var contractPolicy = ContractsExecutorClient.GetPolicy(policy.ProductAddress, policy.ProductTypeId, policy.Id);

                if (contractPolicy.Premium > 0)
                {
                    var txId = ContractsExecutorClient.AddPolicy(policy);
                    _policiesRepository.UpdatePolicyAfterAdd(policy.Id, txId);
                    _logger.Info($"Policy status updated {policy.Id} to paid");
                }
            }
        }
        
        public void ClaimPoliciesInContract()
        { 
            var claimablePolicies = _policiesRepository.GetClaimablePolicies();

            foreach (var policy in claimablePolicies)
            {
                var txId = ContractsExecutorClient.ClaimPolicy(policy);

                if (!string.IsNullOrEmpty(txId))
                {    
                    _policiesRepository.UpdatePolicyAfterClaim(policy.Id, txId);
                    _logger.Info($"Policy status updated {policy.Id} to paidout");
                }
            }
        }
       
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
using System;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;

namespace NZWalks.API.Services
{
    public class ResiliencePolicyService
    {
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        public AsyncPolicyWrap ResiliencePolicy { get; private set; }

        public ResiliencePolicyService()
        {
            // Retry policy: Retries 3 times with exponential backoff
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"[Retry Attempt: {retryCount}] - Waiting {timeSpan.TotalSeconds}s due to error: {exception.Message}");
                    });

            // Circuit breaker: Breaks for 30 sec after 3 consecutive failures
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    (exception, duration) =>
                    {
                        Console.WriteLine($"[Circuit Breaker] - Circuit OPEN! Blocking requests for {duration.TotalSeconds}s due to error: {exception.Message}");
                    },
                    () => Console.WriteLine("[Circuit Breaker] - Circuit RESET! Calls are allowed again."),
                    () => Console.WriteLine("[Circuit Breaker] - Circuit HALF-OPEN! Testing the next call."));

            // Wrap policies together
            ResiliencePolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
        }

        public AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy()
        {
            return _circuitBreakerPolicy;
        }
    }
}

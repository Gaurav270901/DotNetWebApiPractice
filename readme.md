# Resilience and Fault Handling in NZWalks API

## Overview
This project implements resilience and fault tolerance in a .NET Web API using **Polly**. Polly is used to handle **transient failures** by applying **Retry** and **Circuit Breaker** patterns.

## Key Features
- **Retry Policy:** Retries failed operations with exponential backoff.
- **Circuit Breaker Policy:** Stops calls for a certain duration after repeated failures.
- **Policy Wrapping:** Combines both retry and circuit breaker policies.
- **Global Resilience Policy:** Can be injected into repositories to ensure uniform failure handling.

## Solution Implementation
### 1. **Resilience Policy Service**
A centralized service (`ResiliencePolicyService`) creates and provides resilience policies.

```csharp
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
        public AsyncPolicyWrap ResiliencePolicy { get; }
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public ResiliencePolicyService()
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} after error: {exception.Message}");
                    });

            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
                    (exception, duration) => Console.WriteLine($"Circuit broken for {duration.TotalSeconds} sec"),
                    () => Console.WriteLine("Circuit Reset"));

            ResiliencePolicy = Policy.WrapAsync(retryPolicy, _circuitBreakerPolicy);
        }

        public CircuitState GetCircuitBreakerState()
        {
            return _circuitBreakerPolicy.CircuitState;
        }
    }
}
```

### 2. **Registering Resilience Policy in `Program.cs`**
Ensure `ResiliencePolicyService` is registered as a **singleton**:

```csharp
builder.Services.AddSingleton<ResiliencePolicyService>();
```

### 3. **Applying Resilience Policy in Repository**
Modify repositories to use Polly's **ResiliencePolicy**:

```csharp
public class SQLRegionRepository
{
    private readonly ResiliencePolicyService _resiliencePolicy;

    public SQLRegionRepository(ResiliencePolicyService resiliencePolicy)
    {
        _resiliencePolicy = resiliencePolicy;
    }

    public async Task<Region> CreateRegion(Region region)
    {
        return await _resiliencePolicy.ResiliencePolicy.ExecuteAsync(async () =>
        {
            // Database operation
        });
    }
}
```

### 4. **Circuit Breaker Status API**
Create an endpoint to check Polly’s **circuit breaker state**:

```csharp
[ApiController]
[Route("api/resilience")]
public class ResilienceController : ControllerBase
{
    private readonly ResiliencePolicyService _resiliencePolicy;

    public ResilienceController(ResiliencePolicyService resiliencePolicy)
    {
        _resiliencePolicy = resiliencePolicy;
    }

    [HttpGet("circuit-status")]
    public IActionResult GetCircuitBreakerStatus()
    {
        var state = _resiliencePolicy.GetCircuitBreakerState();
        return Ok(new { CircuitState = state.ToString() });
    }
}
```

## Testing the Solution
### **To Test Retry Policy:**
1. Simulate a database failure (throw an exception in the repository method).
2. Observe Polly retrying the operation (logs should display retries).

### **To Test Circuit Breaker Policy:**
1. Simulate 3 consecutive failures.
2. Observe the circuit breaker **opening** and rejecting requests.
3. Wait for 30 seconds and observe the **circuit resetting**.

### **To Check Circuit Status:**
Make a `GET` request to:
```
GET /api/resilience/circuit-status
```
Response:
```json
{
  "CircuitState": "Open" // or "Closed", "HalfOpen"
}
```

## Conclusion
This implementation provides a **resilient API** that can handle transient failures effectively using **Polly’s Retry and Circuit Breaker policies**. The solution ensures that failures do not immediately impact users, and when persistent failures occur, the system **prevents overload** by stopping further calls for a defined period.


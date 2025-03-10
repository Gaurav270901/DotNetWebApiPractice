using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using NZWalks.API.Services;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("api/circuit-breaker")]
    public class CircuitBreakerController : ControllerBase
    {
        private readonly ResiliencePolicyService _resiliencePolicy;

        public CircuitBreakerController(ResiliencePolicyService resiliencePolicy)
        {
            _resiliencePolicy = resiliencePolicy;
        }

        [HttpGet("status")]
        public IActionResult GetCircuitBreakerStatus()
        {
            try
            {
                var circuitBreakerPolicy = _resiliencePolicy.GetCircuitBreakerPolicy();
                var state = circuitBreakerPolicy.CircuitState;

                Console.WriteLine($"[Circuit Breaker] - Current State: {state}");

                return Ok(new { CircuitState = state.ToString() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Circuit Breaker] - Error retrieving state: {ex.Message}");
                return StatusCode(500, new { Error = "Failed to retrieve circuit state", Details = ex.Message });
            }
        }
    }
}

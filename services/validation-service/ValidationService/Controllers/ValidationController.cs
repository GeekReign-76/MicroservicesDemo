// ValidationService/Controllers/ValidationController.cs

using Microsoft.AspNetCore.Mvc;

namespace ValidationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        [HttpPost("validate")]
        public IActionResult Validate([FromBody] SubmitRequest request)
        {
            // Basic validation logic
            if (request == null)
            {
                return BadRequest(new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Request body was null." }
                });
            }

            if (string.IsNullOrWhiteSpace(request.Name) || request.Value <= 0)
            {
                return BadRequest(new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Invalid record" }
                });
            }

            return Ok(new ValidationResult
            {
                IsValid = true,
                Errors = Array.Empty<string>()
            });
        }
    }

    // IMPORTANT:
    // Must be a CLASS, not a positional record.
    // Must match BFF SubmitRequest EXACTLY.
    public class SubmitRequest
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
        public object? Metadata { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}

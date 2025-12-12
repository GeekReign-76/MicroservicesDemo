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
            var isValid = !string.IsNullOrEmpty(request.Name) && request.Value > 0;
            if (!isValid)
                return BadRequest(new { Errors = new[] { "Invalid record" } });

            return Ok(new ValidationResult { IsValid = true });
        }
    }

    public record SubmitRequest(string Name, int Value, object Metadata);
    public record ValidationResult
    {
        public bool IsValid { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
    }
}

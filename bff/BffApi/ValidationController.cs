using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    [HttpPost("validate")]
    public ActionResult<ValidationResult> Validate([FromBody] SubmitRequest req)
    {
        var errors = new List<FieldError>();
        if (string.IsNullOrWhiteSpace(req.Name)) errors.Add(new FieldError("name", "Required"));
        if (req.Value <= 0) errors.Add(new FieldError("value", "Must be > 0"));
        return Ok(new ValidationResult(errors.Count == 0, errors));
    }
}
public record ValidationResult(bool IsValid, List<FieldError> Errors);
public record FieldError(string Field, string Message);
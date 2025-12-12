// ProcessingService/Controllers/ProcessController.cs

using Microsoft.AspNetCore.Mvc;
using ProcessingService.Data;
using ProcessingService.Models;

namespace ProcessingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly DataContext _context;

        public ProcessController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Process([FromBody] SubmitRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body was null.");
            }

            // Convert SubmitRequest → DataRecord (DB entity)
            var record = new DataRecord
            {
                Name = request.Name,
                Value = request.Value,
                Metadata = request.Metadata?.ToString() ?? "",
                ProcessedAt = DateTime.UtcNow
            };

            _context.DataRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }
    }

    // IMPORTANT: Must match BFF SubmitRequest
    public class SubmitRequest
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
        public object? Metadata { get; set; }
    }
}

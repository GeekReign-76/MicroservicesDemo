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
        public async Task<IActionResult> PostData([FromBody] DataRecord record)
        {
            if (record == null)
                return BadRequest();

            record.ProcessedAt = DateTime.UtcNow;

            _context.DataRecords.Add(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }
    }
}

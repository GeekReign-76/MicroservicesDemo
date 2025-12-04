namespace ProcessingService.Models;

public class DataRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Value { get; set; }
    public DateTime ProcessedAt { get; set;  } = DateTime.UtcNow;
    public string? Metadata { get; set; } //optional JSON string
}
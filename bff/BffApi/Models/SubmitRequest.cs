// BffApi/Models/SubmitRequest.cs
namespace BffApi.Models
{
    public class SubmitRequest
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }
}


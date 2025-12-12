using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BffApi
{
    public class ProcessingServiceClient
    {
        private readonly HttpClient _httpClient;

        // Typed client expects HttpClient directly
        public ProcessingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DataRecord> Process(SubmitRequest request)
        {
            // Relative URL because BaseAddress is set
            var response = await _httpClient.PostAsJsonAsync("api/process", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DataRecord>();
            return result ?? throw new HttpRequestException("Processing service returned null");
        }
    }

    public record DataRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public int Value { get; init; }
        public DateTime ProcessedAt { get; init; }
        public string Metadata { get; init; } = "";
    }
}

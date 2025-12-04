using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BffApi
{
    public class ProcessingServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProcessingServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<DataRecord> Process(SubmitRequest request)
        {
            // Replace the URL with running processing service endpoint
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5035/api/process", request);

            response.EnsureSuccessStatusCode(); // throws if not 2xx

            var record = await response.Content.ReadFromJsonAsync<DataRecord>();
            return record!;
        }
    }

    public record DataRecord
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int Value { get; init; }
        public string Metadata { get; init; } = string.Empty;
        public DateTime ProcessedAt { get; init; }
    }
}

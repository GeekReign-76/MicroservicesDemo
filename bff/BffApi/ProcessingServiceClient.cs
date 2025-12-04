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
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5035/api/process", request);

            response.EnsureSuccessStatusCode(); // throws if not 2xx

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


public record DataRecord(int Id, string Name, int Value, DateTime ProcessedAt, string Metadata);

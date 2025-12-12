using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BffApi.Models;


namespace BffApi
{
    public class ProcessingServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProcessingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DataRecord> Process(SubmitRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/process", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DataRecord>();
            return result ?? throw new HttpRequestException("Processing service returned null");
        }
    }

    // Matches what ProcessingService returns
    public class DataRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Value { get; set; }
        public DateTime ProcessedAt { get; set; }

        // Accepts any JSON object from ProcessingService
        public object? Metadata { get; set; }
    }

}

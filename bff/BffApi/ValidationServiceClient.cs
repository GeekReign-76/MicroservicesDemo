using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BffApi.Models;

namespace BffApi
{
    public class ValidationServiceClient
    {
        private readonly HttpClient _httpClient;

        public ValidationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ValidationResult> Validate(SubmitRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/validation/validate", request);

            if (!response.IsSuccessStatusCode)
                return new ValidationResult { IsValid = false, Errors = new[] { "Validation service error" } };

            var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
            return result ?? new ValidationResult { IsValid = false, Errors = new[] { "Null response from validation service" } };
        }
    }

    public record ValidationResult
    {
        public bool IsValid { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
    }
}

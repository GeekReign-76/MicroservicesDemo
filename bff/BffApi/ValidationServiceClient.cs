using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BffApi
{
    public class ValidationServiceClient
    {
        private readonly HttpClient _httpClient;

        // Use typed HttpClient injected by DI
        public ValidationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ValidationResult> Validate(SubmitRequest request)
        {
            // Use relative URL because BaseAddress is already set in Program.cs
            var response = await _httpClient.PostAsJsonAsync("api/validation/validate", request);

            // Return a graceful failure if the call fails
            if (!response.IsSuccessStatusCode)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { $"Validation service returned status code {response.StatusCode}" }
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ValidationResult>();

            return result ?? new ValidationResult
            {
                IsValid = false,
                Errors = new[] { "Validation service returned null" }
            };
        }
    }

    public record ValidationResult
    {
        public bool IsValid { get; init; }
        public string[] Errors { get; init; } = Array.Empty<string>();
    }
}

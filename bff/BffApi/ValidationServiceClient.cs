using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        // Validate a SubmitRequest
        public async Task<ValidationResult> Validate(SubmitRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/validation/validate", request);

            if (!response.IsSuccessStatusCode)
            {
                return new ValidationResult(
                    false,
                    new List<FieldError> { new FieldError("Validation service error") }
                );
            }

            var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
            return result ?? new ValidationResult(
                false,
                new List<FieldError> { new FieldError("Null response from validation service") }
            );
        }

        // Health check method
        public async Task<string> Health()
        {
            var response = await _httpClient.GetAsync("health");

            if (!response.IsSuccessStatusCode)
                return $"Unreachable (status code: {response.StatusCode})";

            return await response.Content.ReadAsStringAsync() ?? "No response body";
        }
    }

    // DTOs for validation result
    public record ValidationResult(bool IsValid, List<FieldError> Errors);

    public record FieldError(string Message);
}

public class ValidationServiceClient
{
    private readonly HttpClient _http;

    public ValidationServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ValidationResult> Validate(SubmitRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/validation/validate", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
        if (result == null)
            throw new InvalidOperationException("Validation service returned null.");

        return result;
    }
}


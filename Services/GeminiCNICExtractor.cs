using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace SMS.Services;

public interface IGeminiCNICExtractor
{
    Task<List<string>> ExtractCNICsFromImageAsync(byte[] imageBytes, string mimeType);
    Task<List<string>> ExtractCNICsFromPDFAsync(byte[] pdfBytes);
}

public class GeminiCNICExtractor : IGeminiCNICExtractor
{
    private readonly string _apiKey;
    private readonly ILogger<GeminiCNICExtractor> _logger;
    private readonly HttpClient _httpClient;
    private const string ModelName = "gemini-2.5-flash";
    //private const string ModelName = "gemini-3-flash-preview";

    private const string GeminiApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

    // Matches: 52102-1565275-5 OR 5210215652755
    private const string CNICPattern = @"\d{5}-?\d{7}-?\d";

    public GeminiCNICExtractor(IConfiguration configuration, ILogger<GeminiCNICExtractor> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _apiKey = configuration["GeminiSettings:ApiKey"];
        _httpClient = httpClientFactory.CreateClient();

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured in appsettings.json");
        }

        _logger.LogInformation("GeminiCNICExtractor initialized with API key");
    }

    public async Task<List<string>> ExtractCNICsFromImageAsync(byte[] imageBytes, string mimeType)
    {
        var cnics = new List<string>();

        try
        {
            _logger.LogInformation($"Sending image to Gemini API for CNIC extraction (MIME type: {mimeType})");

            var base64Image = Convert.ToBase64String(imageBytes);
            var prompt = "You are an expert at reading and extracting CNIC (Computerized National Identity Card) numbers from images. Extract ALL CNIC numbers from this image. CNIC format is: XXXXX-XXXXXXX-X or XXXXXXXXXXXXX (13 digits with optional dashes). Return ONLY the CNIC numbers found, one per line, without any other text or explanation.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new { inlineData = new { mimeType = mimeType, data = base64Image } }
                        }
                    }
                }
            };

            var response = await CallGeminiApi(requestBody);
            var responseText = ExtractTextFromGeminiResponse(response);

            _logger.LogInformation($"Gemini response: {responseText}");

            if (!string.IsNullOrEmpty(responseText))
            {
                var matches = Regex.Matches(responseText, CNICPattern);
                foreach (Match match in matches)
                {
                    var cnic = match.Value.Trim();
                    if (!cnics.Contains(cnic))
                    {
                        cnics.Add(cnic);
                    }
                }
            }

            _logger.LogInformation($"Extracted {cnics.Count} CNICs from image");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting CNICs from image using Gemini API");
            throw;
        }

        return cnics;
    }

    public async Task<List<string>> ExtractCNICsFromPDFAsync(byte[] pdfBytes)
    {
        var cnics = new List<string>();

        try
        {
            _logger.LogInformation("Sending PDF to Gemini API for CNIC extraction");

            var base64Pdf = Convert.ToBase64String(pdfBytes);
            var prompt = "You are an expert at reading and extracting CNIC (Computerized National Identity Card) numbers from PDF documents. Extract ALL CNIC numbers from this PDF. CNIC format is: XXXXX-XXXXXXX-X or XXXXXXXXXXXXX (13 digits with optional dashes). Return ONLY the CNIC numbers found, one per line, without any other text or explanation. If the PDF contains images of documents (like IDs, forms, etc.), carefully read and extract all visible CNICs.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new { inlineData = new { mimeType = "application/pdf", data = base64Pdf } }
                        }
                    }
                }
            };

            var response = await CallGeminiApi(requestBody);
            var responseText = ExtractTextFromGeminiResponse(response);

            _logger.LogInformation($"Gemini PDF extraction response: {responseText}");

            if (!string.IsNullOrEmpty(responseText))
            {
                var matches = Regex.Matches(responseText, CNICPattern);
                foreach (Match match in matches)
                {
                    var cnic = match.Value.Trim();
                    if (!cnics.Contains(cnic))
                    {
                        cnics.Add(cnic);
                    }
                }
            }

            _logger.LogInformation($"Extracted {cnics.Count} CNICs from PDF");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting CNICs from PDF using Gemini API");
            throw;
        }

        return cnics;
    }

    private async Task<JsonElement> CallGeminiApi(object requestBody)
    {
        try
        {
            var url = $"{GeminiApiUrl}{ModelName}:generateContent?key={_apiKey}";
            _logger.LogInformation($"Calling Gemini API: {url}");

            var jsonContent = JsonSerializer.Serialize(requestBody);
            _logger.LogInformation($"Request: {jsonContent}");

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            _logger.LogInformation($"Response Status: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error ({response.StatusCode}): {errorContent}");
                throw new HttpRequestException($"Gemini API returned {response.StatusCode}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response: {responseContent}");

            var jsonResponse = JsonDocument.Parse(responseContent).RootElement;
            return jsonResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            throw;
        }
    }

    private string ExtractTextFromGeminiResponse(JsonElement response)
    {
        try
        {
            if (response.TryGetProperty("candidates", out var candidates) && 
                candidates.ValueKind == System.Text.Json.JsonValueKind.Array &&
                candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];
                if (candidate.TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) &&
                    parts.ValueKind == System.Text.Json.JsonValueKind.Array &&
                    parts.GetArrayLength() > 0)
                {
                    var part = parts[0];
                    if (part.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                }
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from Gemini response");
            return string.Empty;
        }
    }
}



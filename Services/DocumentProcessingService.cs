using System.Text.RegularExpressions;

namespace SMS.Services;

public interface IDocumentProcessingService
{
    Task<List<string>> ExtractCNICsAsync(Stream fileStream, string fileName);
}

public class DocumentProcessingService : IDocumentProcessingService
{
    private readonly ILogger<DocumentProcessingService> _logger;
    private readonly IGeminiCNICExtractor _geminiExtractor;

    // Matches: 52102-1565275-5 OR 5210215652755
    private const string CNICPattern = @"\d{5}-?\d{7}-?\d";

    public DocumentProcessingService(ILogger<DocumentProcessingService> logger, IGeminiCNICExtractor geminiExtractor)
    {
        _logger = logger;
        _geminiExtractor = geminiExtractor ?? throw new ArgumentNullException(nameof(geminiExtractor));
    }

    public async Task<List<string>> ExtractCNICsAsync(Stream fileStream, string fileName)
    {
        var cnics = new List<string>();

        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".pdf")
            {
                _logger.LogInformation($"Extracting CNICs from PDF: {fileName}");
                cnics = await _geminiExtractor.ExtractCNICsFromPDFAsync(fileBytes);
            }
            else if (extension is ".jpg" or ".jpeg" or ".png")
            {
                _logger.LogInformation($"Extracting CNICs from image: {fileName}");
                var mimeType = extension == ".png" ? "image/png" : "image/jpeg";
                cnics = await _geminiExtractor.ExtractCNICsFromImageAsync(fileBytes, mimeType);
            }
            else
            {
                throw new InvalidOperationException("Unsupported file format. Please upload PDF, JPEG, or PNG.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error extracting CNICs from {fileName}");
            throw;
        }

        return cnics
            .Select(c => c.Replace("-", "").Trim())  // normalize: remove dashes
            .Where(c => c.Length == 13)
            .Distinct()
            .ToList();
    }
}
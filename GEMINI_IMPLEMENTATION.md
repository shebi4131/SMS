# Google Gemini CNIC Extraction Implementation

## Overview

This document explains how the SMS application now uses Google's Gemini AI model to extract CNIC (Computerized National Identity Card) numbers from PDF documents and image files, replacing the previous Tesseract OCR approach.

## Why Gemini?

1. **Better Accuracy**: Gemini's vision capabilities are more accurate than traditional OCR for reading Pakistani CNIC numbers from various document formats and qualities
2. **Handles Scanned Documents**: Works seamlessly with both digital and scanned PDFs
3. **Free Tier Available**: The gemini-1.5-flash model is available on Google's free tier
4. **No Local Dependencies**: No need for local tessdata files or complex OCR setup
5. **Intelligent Processing**: Can understand context and extract data more reliably

## Architecture

### Services

#### 1. **GeminiCNICExtractor** (`SMS/Services/GeminiCNICExtractor.cs`)
- **Interface**: `IGeminiCNICExtractor`
- **Methods**:
  - `ExtractCNICsFromImageAsync(byte[] imageBytes, string mimeType)` - Extracts CNICs from image files (JPEG, PNG)
  - `ExtractCNICsFromPDFAsync(byte[] pdfBytes)` - Extracts CNICs from PDF documents

**Key Features**:
- Sends files to Google Gemini API as base64-encoded content
- Uses specialized prompts to instruct Gemini to focus on CNIC extraction
- Parses responses using regex to extract CNIC numbers matching the pattern: `\d{5}-?\d{7}-?\d`
- Handles duplicates automatically
- Comprehensive error logging

#### 2. **DocumentProcessingService** (`SMS/Services/DocumentProcessingService.cs`)
- **Interface**: `IDocumentProcessingService`
- **Methods**:
  - `ExtractCNICsAsync(Stream fileStream, string fileName)` - Main entry point for document processing

**Key Features**:
- Detects file type (PDF, JPEG, PNG)
- Routes to appropriate Gemini extraction method
- Normalizes CNICs by:
  - Removing dashes
  - Trimming whitespace
  - Filtering for 13-digit format
  - Removing duplicates
- Returns clean, validated CNIC list

### Configuration

#### appsettings.json
```json
{
  "GeminiSettings": {
	"ApiKey": "AIzaSyDIXT7ghbMrzMpxbkimmoBBo4_8niq5umU",
	"Model": "gemini-1.5-flash"
  }
}
```

**Important**: The API key is configured in appsettings.json. In production, this should be moved to environment variables or Azure Key Vault for security.

### Dependency Injection

In `Program.cs`, the service is registered as:
```csharp
builder.Services.AddScoped<IGeminiCNICExtractor, GeminiCNICExtractor>();
builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
```

## How It Works

### Flow Diagram

```
User uploads PDF/Image
		↓
DocumentProcessingService.ExtractCNICsAsync()
		↓
File Type Check
		├─ PDF? → GeminiCNICExtractor.ExtractCNICsFromPDFAsync()
		└─ Image? → GeminiCNICExtractor.ExtractCNICsFromImageAsync()
		↓
Convert file to Base64
		↓
Send to Gemini API with specialized prompt
		↓
Gemini analyzes and returns extracted CNICs
		↓
Parse response with regex pattern
		↓
Normalize: Remove dashes, trim, filter for 13-digit format
		↓
Remove duplicates
		↓
Return clean CNIC list
		↓
CNICLookup Component searches database
```

### Gemini Prompts

The extraction uses specialized prompts to guide Gemini:

**For Images**:
```
"You are an expert at reading and extracting CNIC (Computerized National Identity Card) numbers from images. 
Extract ALL CNIC numbers from this image. 
CNIC format is: XXXXX-XXXXXXX-X or XXXXXXXXXXXXX (13 digits with optional dashes). 
Return ONLY the CNIC numbers found, one per line, without any other text or explanation."
```

**For PDFs**:
```
"You are an expert at reading and extracting CNIC (Computerized National Identity Card) numbers from PDF documents. 
Extract ALL CNIC numbers from this PDF. 
CNIC format is: XXXXX-XXXXXXX-X or XXXXXXXXXXXXX (13 digits with optional dashes). 
Return ONLY the CNIC numbers found, one per line, without any other text or explanation. 
If the PDF contains images of documents (like IDs, forms, etc.), carefully read and extract all visible CNICs."
```

## CNIC Pattern

The application recognizes and normalizes CNICs following this pattern:
- **Format**: `52102-1565275-5` or `5210215652755`
- **Regex Pattern**: `\d{5}-?\d{7}-?\d`
- **Normalized Length**: 13 digits (always without dashes in the database)

### Normalization Process

1. Extract matches from Gemini response using regex
2. Remove all dashes: `52102-1565275-5` → `5210215652755`
3. Trim whitespace
4. Filter for exactly 13 digits
5. Remove duplicates
6. Return final list

## Usage

### In Blazor Component (CNICLookup.razor)

```csharp
@inject IDocumentProcessingService DocumentService
@inject IMemberService MemberService

private async Task HandleFileSelected(InputFileChangeEventArgs e)
{
	try
	{
		IsProcessing = true;
		using var stream = e.File.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // 10MB max

		// Extract CNICs using Gemini
		ExtractedCNICs = await DocumentService.ExtractCNICsAsync(stream, e.File.Name);

		SuccessMessage = $"Extracted {ExtractedCNICs.Count} CNICs from document";
	}
	catch (Exception ex)
	{
		ErrorMessage = $"Error: {ex.Message}";
	}
	finally
	{
		IsProcessing = false;
	}
}

private async Task SearchMembers()
{
	try
	{
		IsSearching = true;
		var results = await MemberService.GetMembersByCNICsAsync(ExtractedCNICs);
		// Display results...
	}
	finally
	{
		IsSearching = false;
	}
}
```

## Database Lookup

After CNICs are extracted by Gemini, the `MemberService` queries the database:

```csharp
public async Task<List<MemberLookupResult>> GetMembersByCNICsAsync(List<string> cnics)
{
	var normalizedCNICs = cnics.Select(c => c.Trim()).ToList();

	var results = await _context.MemberInfos
		.Where(m => normalizedCNICs.Contains(m.CNIC.Trim()))
		.Join(_context.MembersRegs, ...)
		.Join(_context.MemberInvs, ...)
		// ... additional joins for complete member information
		.ToListAsync();

	return results;
}
```

## Error Handling

Both extractors implement comprehensive error handling:

1. **API Errors**: Caught and logged with context
2. **Invalid Files**: Format validation before sending to API
3. **No CNICs Found**: Returns empty list gracefully
4. **Regex Parsing**: Handles malformed responses
5. **Network Issues**: Exceptions bubble up to UI with user-friendly messages

## Logging

The implementation logs key events at different levels:

- **Information**: File received, extraction started, number of CNICs found
- **Warning**: PDF text extraction failed (but continues with fallback)
- **Error**: API failures, invalid formats, unexpected errors

Example logs:
```
[Information] Sending PDF to Gemini API for CNIC extraction
[Information] Gemini PDF extraction response: 52102-1565275-5...
[Information] Extracted 3 CNICs from PDF
```

## Configuration in Production

### Security Considerations

1. **Never commit API key**: Use environment variables or key vault
2. **Use Service Account**: Consider using a dedicated Google Cloud service account
3. **Rate Limiting**: Monitor API usage to stay within free tier limits
4. **API Key Rotation**: Regularly rotate keys for security

### Example Production Setup

**appsettings.Production.json**:
```json
{
  "GeminiSettings": {
	"ApiKey": null,
	"Model": "gemini-1.5-flash"
  }
}
```

**Program.cs (with environment variables)**:
```csharp
var geminiApiKey = builder.Configuration["GeminiSettings:ApiKey"] 
	?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");

if (string.IsNullOrEmpty(geminiApiKey))
{
	throw new InvalidOperationException(
		"Gemini API key must be set in appsettings or GEMINI_API_KEY environment variable");
}
```

## Supported File Types

| Format | MIME Type | Status |
|--------|-----------|--------|
| PDF | application/pdf | ✅ Supported |
| JPEG | image/jpeg | ✅ Supported |
| PNG | image/png | ✅ Supported |
| JPG | image/jpeg | ✅ Supported |

Maximum file size: 10MB (configurable in upload component)

## API Quotas and Limits

**Google Gemini Free Tier**:
- **Requests per minute**: 15
- **Tokens per minute**: 1 million
- **Requests per day**: Unlimited
- **File size**: Typically up to 20MB

**Best Practices**:
1. Batch multiple CNICs from one document in a single request
2. Implement request throttling for bulk operations
3. Cache results when possible
4. Monitor API usage in Google Cloud Console

## Testing

### Manual Testing

1. Upload a PDF containing CNIC numbers
2. Observe logs in Application Output window
3. Verify extracted CNICs appear in the UI
4. Click "Search Members" to query database
5. Verify member records display correctly

### Expected Behavior

- ✅ PDFs with text-based CNICs: Instant extraction
- ✅ Scanned PDF images: Gemini vision reading
- ✅ Handwritten CNICs: Varies based on clarity
- ✅ Multiple CNICs per document: All extracted
- ✅ Duplicate CNICs: Automatically removed

## Removed Components

The following OCR-based code has been removed (packages kept for future use):

- ✅ **Tesseract OCR**: All OCR logic and image preprocessing removed
- ✅ **PDF page rendering**: No longer need to convert PDFs to images
- ✅ **ImageSharp preprocessing**: Grayscale conversion and upscaling removed
- ✅ **TessData**: No longer required

**Packages Retained** (for potential future use):
- `itext7` - PDF processing
- `SixLabors.ImageSharp` - Image handling
- `Tesseract` - OCR library

## Troubleshooting

### Issue: "Gemini API key is not configured"
**Solution**: Check `appsettings.json` has valid GeminiSettings with ApiKey

### Issue: No CNICs extracted from valid document
**Solution**: 
1. Check document contains readable text/images
2. Verify Gemini API is responsive (check Google Cloud Console)
3. Review logs for specific error messages

### Issue: Duplicate CNICs in results
**Solution**: Already handled by the service - duplicates are automatically removed

### Issue: API rate limit exceeded
**Solution**: Implement request queuing or delay between requests on bulk operations

## Future Enhancements

1. **Batch Processing**: Handle multiple documents in parallel
2. **Confidence Scoring**: Return confidence levels for extracted CNICs
3. **Validation**: Add checksum validation for CNIC format
4. **Caching**: Cache extraction results for identical documents
5. **Alternative Models**: Support for other Gemini models (Pro, Ultra)
6. **Webhook Integration**: Async extraction for large batches

## References

- [Google Generative AI C# SDK](https://github.com/googleapis/google-generative-ai-dotnet)
- [Google Gemini API Documentation](https://ai.google.dev/docs)
- [Pakistani CNIC Format](https://en.wikipedia.org/wiki/Computerized_National_Identity_Card)

## Support

For issues or questions:
1. Check the logs in Application Output window
2. Review this documentation
3. Check Google Cloud Console for API status
4. Verify appsettings.json configuration

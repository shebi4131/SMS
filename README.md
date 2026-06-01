# CNIC Member Lookup - Blazor Application

A complete single-page Blazor application that allows users to upload PDF or image documents, extract CNIC numbers, and lookup member records from SQL Server database.

## Features

✅ **PDF Upload & CNIC Extraction** - Upload PDF documents and automatically extract CNIC numbers using text extraction
✅ **Image Support** - Support for JPEG and PNG formats
✅ **Database Lookup** - Search extracted CNICs against SQL Server database
✅ **Beautiful UI** - Modern, responsive Bootstrap 5 design with animations
✅ **Member Details Modal** - View complete member information including plot details, contact info, and allocation dates
✅ **Error Handling** - Comprehensive error messages and validation

## Project Structure

```
SMS/
├── Models/                          # Data Models
│   ├── MemberInfo.cs               # Member Information model
│   ├── MembersReg.cs               # Member Registration model
│   ├── MemberInv.cs                # Member Investment model
│   ├── MInvPlotFlat.cs             # Plot/Flat Information model
│   ├── PlotSize.cs                 # Plot Size model
│   └── MemberLookupResult.cs       # DTO for query results
│
├── Data/
│   └── ApplicationDbContext.cs      # Entity Framework Core DbContext
│
├── Services/                        # Business Logic Services
│   ├── DocumentProcessingService.cs # PDF/Image CNIC extraction
│   └── MemberService.cs             # Database query operations
│
├── Components/
│   └── Pages/
│       └── CNICLookup.razor         # Main UI Component
│
├── wwwroot/
│   └── app.css                      # Custom styling with gradients & animations
│
├── Program.cs                       # Dependency Injection & Configuration
├── appsettings.json                 # Configuration & Connection String
└── SMS.csproj                       # Project File with NuGet packages
```

## Setup Instructions

### 1. Update Database Connection

Open `appsettings.json` and update your SQL Server connection string:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**Example:**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=SHOAIB-PC\\SQLEXPRESS;Database=SMS_DB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

### 2. Database Schema

The application expects the following SQL Server tables (as per your query):
- `Tbl_Memberinfo` - Member information
- `Tbl_MembersReg` - Member registration (junction table)
- `Tbl_MemberInv` - Member investment details
- `Tbl_MInvPlotFlat` - Plot/Flat allocation
- `Tbl_PlotSize` - Plot size reference

### 3. Run the Application

```powershell
cd "F:\Personal\shoaib ahmed\.Net Blazor\SMS"
dotnet run
```

The application will start on:
- Development: `https://localhost:5001`
- Open in your browser and start using!

## How to Use

1. **Upload Document**
   - Click on the upload zone or select a PDF/JPEG/PNG file
   - Supports files up to 10MB

2. **Extract CNICs**
   - CNICs are automatically extracted from PDF documents
   - All 13-digit numbers in format are extracted

3. **Manage Extracted CNICs**
   - View all extracted CNICs in badges
   - Remove any incorrect CNICs with the X button

4. **Search Members**
   - Click "Search Members" button
   - Results appear in a table with key information

5. **View Details**
   - Click "View" button on any row
   - Popup modal shows complete member details

## NuGet Packages Used

| Package | Version | Purpose |
|---------|---------|---------|
| itext7 | 9.0.0 | PDF text extraction |
| SixLabors.ImageSharp | 3.0.2 | Image processing |
| Tesseract | 5.2.0 | OCR for images (optional) |
| Microsoft.EntityFrameworkCore | 8.0.0 | ORM for database |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 | SQL Server provider |
| Microsoft.EntityFrameworkCore.Tools | 8.0.0 | EF CLI tools |

## Key Files Explained

### Program.cs
- Configures Blazor rendering
- Registers Entity Framework Core DbContext
- Registers dependency injection for services

### DocumentProcessingService.cs
- `ExtractCNICsAsync()` - Main method for CNIC extraction
- Supports PDF and image formats
- Uses regex pattern `\d{13}` to find 13-digit CNICs

### MemberService.cs
- `GetMembersByCNICsAsync()` - Query multiple CNICs
- `GetMemberByCNICAsync()` - Query single CNIC
- Joins multiple tables to get complete member info

### CNICLookup.razor
- Interactive Blazor Server component
- Handles file upload and processing
- Displays results in responsive tables
- Modal for detailed member view

### app.css
- Bootstrap 5 with custom extensions
- Gradient background animation
- Smooth transitions and hover effects
- Responsive design for mobile & desktop

## PDF Extraction Details

The application uses itext7 library to:
1. Open PDF file as stream
2. Iterate through all pages
3. Extract text using `LocationTextExtractionStrategy`
4. Apply regex to find CNIC patterns
5. Return unique CNICs as list

**Regex Pattern:** `\d{13}` (exactly 13 digits)

## Database Connection Notes

### For Local SQL Server:
```
Server=.;Database=YourDB;Trusted_Connection=true;Encrypt=false;
```

### For Named Instance:
```
Server=COMPUTER_NAME\INSTANCE_NAME;Database=YourDB;Trusted_Connection=true;Encrypt=false;
```

### For SQL Server Authentication:
```
Server=YOUR_SERVER;Database=YourDB;User Id=sa;Password=YourPassword;Encrypt=false;
```

## Troubleshooting

### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet build
```

### Connection String Issues
- Verify SQL Server is running
- Check database name is correct
- Ensure user has permissions
- Try connecting with SQL Server Management Studio first

### PDF Extraction Not Working
- Ensure PDF is text-based (not scanned image)
- Check CNIC format matches `\d{13}` pattern
- Verify text is readable in Adobe Reader

### No Results Found
- Verify CNICs exist in database
- Check CNIC values are trimmed (spaces removed)
- Use SQL query to verify data exists

## Performance Optimization

For large datasets:
1. Add database indexes on `Tbl_Memberinfo.CNIC`
2. Use pagination if many results
3. Cache member data if used frequently

## Security Considerations

1. **Connection String** - Store sensitive credentials in user secrets
   ```powershell
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YourConnectionString"
   ```

2. **Input Validation** - CNIC format is validated (13 digits only)

3. **SQL Injection** - Uses Entity Framework Core which prevents SQL injection

4. **File Upload** - Limited to 10MB, validates file extensions

## Future Enhancements

- [ ] OCR for scanned documents (full Tesseract integration)
- [ ] Barcode scanning support
- [ ] Export results to Excel
- [ ] Batch import CNICs from CSV
- [ ] Advanced search filters
- [ ] Audit logging
- [ ] Multi-language support
- [ ] Mobile app version

## Support & Debugging

### Enable Detailed Logging
In `appsettings.json`:
```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Debug",
	  "Microsoft.EntityFrameworkCore": "Debug"
	}
  }
}
```

### Visual Studio Debug
1. Set breakpoints in code
2. Press F5 to debug
3. Use Debug > Windows > Output to view logs

## License

This application is provided as-is for internal use.

## Contact & Questions

For issues or questions, please contact the development team.

---

**Last Updated:** 2024
**Framework:** .NET 8.0 with Blazor Server
**Database:** SQL Server

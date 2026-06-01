## Troubleshooting Guide

### Build & Compilation Issues

#### Error: "The type or namespace name 'X' could not be found"
**Solution:**
- Make sure all using statements are present
- Run `dotnet restore` to restore NuGet packages
- Clean and rebuild: `dotnet clean && dotnet build`

#### Error: Package 'X' could not be found
**Solution:**
```powershell
dotnet restore
dotnet clean
dotnet build
```

#### Warning: Package 'SixLabors.ImageSharp' has vulnerabilities
**Note:** This is a known issue with ImageSharp 3.0.x. It's safe to use for this application.
To suppress warnings, add to `.csproj`:
```xml
<PropertyGroup>
  <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
</PropertyGroup>
```

---

### Runtime Issues

#### Error: "No connection string named 'DefaultConnection' found"
**Problem:** Connection string not configured in appsettings.json
**Solution:**
1. Open `appsettings.json`
2. Add your connection string:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

#### Error: "Cannot open database 'X' requested by the login"
**Problem:** Database doesn't exist or wrong database name
**Solution:**
- Verify database exists in SQL Server
- Check spelling in connection string
- Use SQL Server Management Studio to verify

#### Error: "Login failed for user"
**Problem:** Wrong credentials or SQL Server not running
**Solution:**
1. Verify SQL Server service is running
2. Test connection with SQL Server Management Studio
3. Check username/password in connection string

#### Error: "An error occurred while configuring Entity Framework Core"
**Problem:** Database connection or schema mismatch
**Solution:**
1. Verify connection string works in SSMS
2. Check all required tables exist in database
3. Review error details in browser console

---

### PDF Upload Issues

#### "No CNICs found in the document"
**Problem:** PDF doesn't contain text or CNIC format is different
**Solution:**
1. Verify PDF is text-based (not scanned image)
2. Try opening PDF in Adobe Reader and selecting text
3. Check CNIC format is exactly 13 digits
4. Try different PDF files

#### PDF extraction hangs or times out
**Problem:** Large or complex PDF
**Solution:**
- Try uploading smaller PDF files first
- Ensure PDF is not corrupted
- Check file size doesn't exceed 10MB

#### "File size exceeds 10MB limit"
**Problem:** Uploaded file is too large
**Solution:**
- Upload smaller files
- Split PDF into multiple files
- To increase limit, modify in CNICLookup.razor:
```csharp
maxAllowedSize: 20 * 1024 * 1024  // Change to desired size
```

---

### Database Query Issues

#### "No member records found"
**Problem:** CNICs don't exist in database or query issue
**Solution:**
1. Run test query in SQL Server:
   ```sql
   SELECT * FROM Tbl_Memberinfo WHERE CNIC = '1234567890123'
   ```
2. Verify CNIC exists in database
3. Check for extra spaces in CNIC values
4. Try using different CNICs you know exist

#### "Null reference exception" or "Object reference error"
**Problem:** Missing data or relationship mismatch
**Solution:**
1. Verify all foreign key relationships are set up
2. Check that MembersReg has entries linking MemberInfo to MemberInv
3. Use SQL to verify data integrity:
   ```sql
   SELECT mr.* FROM Tbl_MembersReg mr
   WHERE mr.MemberSID NOT IN (SELECT MemberinfoID FROM Tbl_Memberinfo)
   OR mr.MemberInvID NOT IN (SELECT MemberInvID FROM Tbl_MemberInv)
   ```

#### Very slow search/query
**Problem:** Missing indexes
**Solution:**
Run the SQL commands in DATABASE_SETUP.sql to create indexes:
```sql
CREATE NONCLUSTERED INDEX IX_Memberinfo_CNIC 
ON Tbl_Memberinfo(CNIC);
```

---

### Browser/UI Issues

#### "Application loads but buttons don't work"
**Problem:** Blazor JavaScript interop issue
**Solution:**
1. Clear browser cache: Ctrl+Shift+Delete
2. Hard refresh: Ctrl+F5
3. Open browser DevTools: F12
4. Check console for JavaScript errors

#### "Upload button not responding"
**Problem:** InputFile component issue
**Solution:**
1. Check browser console for errors
2. Try different file format
3. Clear temporary files: `dotnet clean`
4. Restart application

#### Styling looks wrong (no colors/layout)
**Problem:** CSS file not loaded
**Solution:**
1. Check browser network tab (F12)
2. Verify `app.css` exists in wwwroot/
3. Hard refresh browser: Ctrl+Shift+Delete then F5
4. Check for CSS loading errors in console

---

### Connection String Troubleshooting

#### Testing Connection String Locally
Use this PowerShell script to test:
```powershell
# Test SQL Server Connection
$connectionString = "Server=.;Database=YourDB;Trusted_Connection=true;Encrypt=false;"
$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString
try {
	$connection.Open()
	Write-Host "Connection successful!"
	$connection.Close()
} catch {
	Write-Host "Connection failed: $($_.Exception.Message)"
}
```

#### Different Connection String Formats

**Local (Default Instance):**
```
Server=.;Database=MyDB;Trusted_Connection=true;Encrypt=false;
```

**Named Instance (SQLEXPRESS):**
```
Server=COMPUTER_NAME\SQLEXPRESS;Database=MyDB;Trusted_Connection=true;Encrypt=false;
```

**SQL Authentication:**
```
Server=SERVERNAME;Database=MyDB;User Id=sa;Password=MyPassword;Encrypt=false;
```

**Network SQL Server:**
```
Server=192.168.1.100;Database=MyDB;User Id=sa;Password=MyPassword;Connection Timeout=30;Encrypt=false;
```

---

### Performance Optimization

#### Application is slow
**Solutions:**
1. Add database indexes (see DATABASE_SETUP.sql)
2. Increase connection pool size:
   ```json
   "DefaultConnection": "Server=.;Database=DB;Min Pool Size=5;Max Pool Size=100;Trusted_Connection=true;"
   ```
3. Use async/await (already implemented)
4. Cache frequently accessed data

#### High memory usage
**Solutions:**
1. Process smaller batches
2. Dispose resources properly
3. Monitor with Task Manager
4. Check for memory leaks in browser DevTools

---

### Logging & Debugging

#### Enable Debug Logging
In `appsettings.Development.json`:
```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Debug",
	  "Microsoft": "Debug",
	  "Microsoft.EntityFrameworkCore": "Debug",
	  "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
	}
  }
}
```

#### View Application Logs
1. In Visual Studio: Debug Output window (Ctrl+Alt+O)
2. In browser: DevTools Console (F12)
3. Check Event Viewer for Windows Event Log

#### SQL Query Logging
Add to Program.cs:
```csharp
builder.Services.AddLogging(config =>
{
	config.AddConsole();
	config.SetMinimumLevel(LogLevel.Debug);
});
```

---

### Common Solutions Checklist

- [ ] Updated `appsettings.json` with correct connection string
- [ ] Verified SQL Server is running
- [ ] Confirmed database exists and contains required tables
- [ ] Ran `dotnet build` successfully
- [ ] Started application with `dotnet run`
- [ ] Opened browser to `https://localhost:5001`
- [ ] Tested with valid PDF containing CNICs
- [ ] Verified CNIC data exists in database
- [ ] Checked for errors in browser console (F12)
- [ ] Reviewed application output window

---

### Getting Help

**If issues persist:**
1. Collect error details (exact error message)
2. Check browser DevTools (F12) for JavaScript errors
3. Check Visual Studio Output window for build/runtime errors
4. Review SQL Server query results
5. Enable debug logging (see above)
6. Search issue in documentation or GitHub

**Error Information to Gather:**
- Full error message
- Stack trace (if available)
- Steps to reproduce
- Connection string format (without credentials)
- Database table names
- SQL Server version

---

**Remember:** Most issues are related to database connection or data structure mismatches. Always verify your database first!

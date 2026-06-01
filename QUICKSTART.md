## Quick Start Guide

### Prerequisites
- Visual Studio 2026 or Visual Studio Code
- .NET 8.0 SDK installed
- SQL Server (local or remote)
- Your database with the required tables

### Step 1: Configure Database Connection (IMPORTANT!)

1. Open `appsettings.json`
2. Find the connection string section
3. Replace with your server details:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;Trusted_Connection=true;Encrypt=false;"
}
```

### Step 2: Run the Application

**Option A - Using Visual Studio:**
1. Open the solution
2. Press F5 to run
3. Application opens automatically

**Option B - Using Command Line:**
```powershell
cd "F:\Personal\shoaib ahmed\.Net Blazor\SMS"
dotnet run
```

The app will be available at: `https://localhost:5001`

### Step 3: Test the Application

1. **Upload a PDF**
   - Click upload area
   - Select your PDF file with CNICs
   - Wait for extraction

2. **Extract CNICs**
   - System will automatically extract 13-digit numbers
   - Review extracted CNICs
   - Remove any incorrect entries

3. **Search Database**
   - Click "Search Members" button
   - View results in the table
   - Click "View" for full details

### Common Database Connection Strings

**Local SQL Server (Windows Auth):**
```
Server=.;Database=YourDB;Trusted_Connection=true;Encrypt=false;
```

**Named Instance:**
```
Server=COMPUTER_NAME\SQLEXPRESS;Database=YourDB;Trusted_Connection=true;Encrypt=false;
```

**SQL Server Authentication:**
```
Server=SERVERNAME;Database=YourDB;User Id=username;Password=password;Encrypt=false;
```

**Azure SQL Database:**
```
Server=tcp:servername.database.windows.net,1433;Initial Catalog=dbname;Persist Security Info=False;User ID=username;Password=password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### Verify Your Setup

**Check Database Connection:**
```powershell
# Test connection using SQL Server Management Studio or
# Verify tables exist:
# SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES

# Check data:
# SELECT * FROM Tbl_Memberinfo LIMIT 5
```

### Port Configuration

If port 5001 is already in use, configure in `Properties/launchSettings.json`:
```json
"applicationUrl": "https://localhost:YOUR_PORT"
```

### Stop the Application

Press `Ctrl+C` in the terminal, or close the browser window.

### Enable Developer Mode

For enhanced debugging, modify `appsettings.Development.json`:
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

### Update NuGet Packages

If you need to update packages:
```powershell
dotnet restore
```

### Clean Build

If you encounter build issues:
```powershell
dotnet clean
dotnet build
```

### Publish for Production

```powershell
dotnet publish -c Release -o ./publish
```

Then deploy the `publish` folder to your server.

---

**Next Steps:**
1. ✅ Update `appsettings.json` with your database connection
2. ✅ Run `dotnet build` to verify everything compiles
3. ✅ Run `dotnet run` to start the application
4. ✅ Open browser to `https://localhost:5001`
5. ✅ Upload your first PDF!

**Need Help?**
- Check `README.md` for detailed documentation
- Review error messages in the browser console
- Check Visual Studio Output window for build issues

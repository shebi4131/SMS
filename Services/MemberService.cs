using Microsoft.EntityFrameworkCore;
using SMS.Data;
using SMS.Models;
using Microsoft.Data.SqlClient;

namespace SMS.Services;

public interface IMemberService
{
    Task<List<MemberLookupResult>> GetMembersByCNICsAsync(List<string> cnics);
    Task<MemberLookupResult?> GetMemberByCNICAsync(string cnic);
    Task<List<MemberLookupResult>> SearchMembersAsync(MemberSearchRequest request);
}

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MemberService> _logger;

    public MemberService(ApplicationDbContext context, ILogger<MemberService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MemberLookupResult>> GetMembersByCNICsAsync(List<string> cnics)
    {
        try
        {
            if (!cnics.Any())
            {
                _logger.LogWarning("No CNICs provided for lookup");
                return new List<MemberLookupResult>();
            }

            var normalizedCNICs = cnics.Select(c => c.Trim()).ToList();
            var cnicList = string.Join(",", normalizedCNICs.Select(c => $"'{c}'"));

            var sql = $@"
    SELECT 
        m.FullName, m.CNIC, mpf.PFlotNo, mpf.PFlotNo2, mpf.StNo,
        ps.PlotSize AS PlotSize,
        CAST(mi.ProID AS NVARCHAR(50)) AS ProID,
        CAST(m.MeshipNo AS NVARCHAR(50)) AS MeshipNo,
        CAST(m.oldMNo AS NVARCHAR(50)) AS OldMNo,
        m.Address1, m.CellNo, mi.AlotTransEntryDate,
        CAST(mi.BookNo AS NVARCHAR(50)) AS BookNo
    FROM Tbl_Memberinfo m
    INNER JOIN Tbl_MembersReg mr ON m.MemberinfoID = mr.MemberSID
    INNER JOIN Tbl_MemberInv mi ON mr.MemberInvID = mi.MemberInvID
    INNER JOIN Tbl_MInvPlotFlat mpf ON mi.MemberInvID = mpf.MemberInvID
    INNER JOIN Tbl_PlotSize ps ON mpf.PlotSizeID = ps.PlotSizeID
    WHERE LTRIM(RTRIM(m.CNIC)) IN ({cnicList})";

            var results = await _context.Database
                .SqlQueryRaw<MemberLookupResult>(sql)
                .ToListAsync();

            results = results
                .GroupBy(r => new { r.CNIC, r.PFlotNo })
                .Select(g => g.First())
                .ToList();

            _logger.LogInformation($"Found {results.Count} member records for {cnics.Count} CNICs");
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving members by CNICs");
            throw;
        }
    }

    public async Task<MemberLookupResult?> GetMemberByCNICAsync(string cnic)
    {
        try
        {
            var normalizedCNIC = cnic.Trim();

            var result = await _context.MemberInfos
.Where(m => m.CNIC == normalizedCNIC)
                .Join(_context.MembersRegs,
                    m => m.MemberinfoID,
                    mr => mr.MemberSID,
                    (m, mr) => new { MemberInfo = m, MembersReg = mr })
                .Join(_context.MemberInvs,
                    x => x.MembersReg.MemberInvID,
                    mi => mi.MemberInvID,
                    (x, mi) => new { x.MemberInfo, x.MembersReg, MemberInv = mi })
                .Join(_context.MInvPlotFlats,
                    x => x.MemberInv.MemberInvID,
                    mpf => mpf.MemberInvID,
                    (x, mpf) => new { x.MemberInfo, x.MembersReg, x.MemberInv, MInvPlotFlat = mpf })
                .Join(_context.PlotSizes,
                    x => x.MInvPlotFlat.PlotSizeID,
                    ps => ps.PlotSizeID,
                    (x, ps) => new MemberLookupResult
                    {
                        FullName = x.MemberInfo.FullName,
                        CNIC = x.MemberInfo.CNIC,
                        PFlotNo = x.MInvPlotFlat.PFlotNo,
                        PFlotNo2 = x.MInvPlotFlat.PFlotNo2,
                        StNo = x.MInvPlotFlat.StNo,
                        PlotSize = ps.PlotSizeValue,
                        ProID = x.MemberInv.ProID,
                        MeshipNo = x.MemberInfo.MeshipNo,
                        OldMNo = x.MemberInfo.OldMNo,
                        Address1 = x.MemberInfo.Address1,
                        CellNo = x.MemberInfo.CellNo,
                        AlotTransEntryDate = x.MemberInv.AlotTransEntryDate,
                        BookNo = x.MemberInv.BookNo
                    })
                .FirstOrDefaultAsync();

            if (result != null)
            {
                _logger.LogInformation($"Found member with CNIC: {cnic}");
            }
            else
            {
                _logger.LogWarning($"No member found with CNIC: {cnic}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving member by CNIC: {cnic}");
            throw;
        }
    }

    public async Task<List<MemberLookupResult>> SearchMembersAsync(MemberSearchRequest request)
    {
        try
        {
            if (request == null)
            {
                _logger.LogWarning("MemberSearchRequest is null");
                return new List<MemberLookupResult>();
            }

            // Use the stored procedure directly with SqlParameter
            var connection = _context.Database.GetDbConnection();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SPR_MemberSearch";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Add parameters matching the stored procedure
                command.Parameters.Add(new SqlParameter("@OpCode", request.OpCode));
                command.Parameters.Add(new SqlParameter("@CNIC", (object?)request.CNIC ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Search", request.Search ?? ""));
                command.Parameters.Add(new SqlParameter("@SearchVal", request.SearchVal ?? ""));
                command.Parameters.Add(new SqlParameter("@PFCatID", request.PFCatID));
                command.Parameters.Add(new SqlParameter("@PlotSizeID", request.PlotSizeID));
                command.Parameters.Add(new SqlParameter("@ProID", request.ProID));
                command.Parameters.Add(new SqlParameter("@PFlotNo", request.PFlotNo ?? ""));
                command.Parameters.Add(new SqlParameter("@PFlotNo2", request.PFlotNo2 ?? ""));
                command.Parameters.Add(new SqlParameter("@StNo", request.StNo ?? ""));

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var results = new List<MemberLookupResult>();

                    while (await reader.ReadAsync())
                    {
                        var result = new MemberLookupResult
                        {
                            MemberNo = GetStringValue(reader, "MemberNo"),
                            MemberNo2 = GetStringValue(reader, "MemberNo2"),
                            BookNo = GetStringValue(reader, "BookNo"),
                            FullName = GetStringValue(reader, "Member"),
                            CNIC = GetStringValue(reader, "CNIC"),
                            CardEDate = GetStringValue(reader, "CardEDate"),
                            CardSDate = GetStringValue(reader, "CardSDate"),
                            DOB = GetStringValue(reader, "DOB"),
                            PFlotNo = GetStringValue(reader, "PFlotNo"),
                            PFlotNo2 = GetStringValue(reader, "PFlotNo2"),
                            StNo = GetStringValue(reader, "StNo"),
                            PlotSize = GetStringValue(reader, "PlotSize"),
                            PlotStatus = GetStringValue(reader, "PlotStatus"),
                            ProID = GetStringValue(reader, "ProID")
                        };
                        results.Add(result);
                    }

                    _logger.LogInformation($"Member search returned {results.Count} results");
                    return results;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing member search");
            throw;
        }
    }

    private static string? GetStringValue(System.Data.IDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            // Get the value and convert to string
            var value = reader.GetValue(ordinal);

            if (value == null || value is DBNull)
            {
                return null;
            }

            // Convert any type to string
            return value.ToString();
        }
        catch (IndexOutOfRangeException)
        {
            // Column doesn't exist
            return null;
        }
        catch (Exception ex)
        {
            // Log and return null for any other error
            System.Diagnostics.Debug.WriteLine($"Error reading column {columnName}: {ex.Message}");
            return null;
        }
    }
}

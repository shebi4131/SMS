namespace SMS.Models;

/// <summary>
/// DTO class for returning member lookup results
/// </summary>
public class MemberLookupResult
{
    public string? FullName { get; set; }
    public string? CNIC { get; set; }
    public string? PFlotNo { get; set; }
    public string? PFlotNo2 { get; set; }
    public string? StNo { get; set; }
    public string? PlotSize { get; set; }
    public string? ProID { get; set; }
    public string? MeshipNo { get; set; }
    public string? OldMNo { get; set; }
    public string? Address1 { get; set; }
    public string? CellNo { get; set; }
    public DateTime? AlotTransEntryDate { get; set; }
    public string? BookNo { get; set; }
}

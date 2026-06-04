namespace SMS.Models;

/// <summary>
/// DTO class for member search request parameters
/// Maps to SPR_MemberSearch stored procedure parameters
/// </summary>
public class MemberSearchRequest
{
    /// <summary>
    /// Operation code (typically 1 for search)
    /// </summary>
    public int OpCode { get; set; } = 1;

    /// <summary>
    /// CNIC to search for (optional)
    /// </summary>
    public string? CNIC { get; set; }

    /// <summary>
    /// Free-text search term
    /// </summary>
    public string? Search { get; set; } = "";

    /// <summary>
    /// Column name to search in (e.g., "Tbl_Memberinfo.FullName")
    /// </summary>
    public string? SearchVal { get; set; } = "";

    /// <summary>
    /// Plot/Flat Category ID (-1 = all)
    /// </summary>
    public short PFCatID { get; set; } = -1;

    /// <summary>
    /// Plot Size ID (-1 = all)
    /// </summary>
    public int PlotSizeID { get; set; } = -1;

    /// <summary>
    /// Project ID (-1 = all)
    /// </summary>
    public long ProID { get; set; } = -1;

    /// <summary>
    /// Plot/Flat Number (first part)
    /// </summary>
    public string? PFlotNo { get; set; } = "";

    /// <summary>
    /// Plot/Flat Number (second part)
    /// </summary>
    public string? PFlotNo2 { get; set; } = "";

    /// <summary>
    /// Street Number
    /// </summary>
    public string? StNo { get; set; } = "";
}

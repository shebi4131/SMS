namespace SMS.Models;

public class MInvPlotFlat
{
    public int MInvPlotFlatID { get; set; }
    public int MemberInvID { get; set; }
    public string PFlotNo { get; set; } = string.Empty;
    public string PFlotNo2 { get; set; } = string.Empty;
    public string StNo { get; set; } = string.Empty;
    public int PlotSizeID { get; set; }

    // Navigation properties
    public MemberInv? MemberInv { get; set; }
    public PlotSize? PlotSize { get; set; }
}

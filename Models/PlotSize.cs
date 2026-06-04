namespace SMS.Models;

public class PlotSize
{
    public int PlotSizeID { get; set; }
    public int? PlotCategoryID { get; set; }
    public string PlotSizeValue { get; set; } = string.Empty;
    public DateTime? CreateDate { get; set; }
    public bool? isShow { get; set; } = true;

    // Navigation properties
    public ICollection<MInvPlotFlat> MInvPlotFlats { get; set; } = new List<MInvPlotFlat>();
}

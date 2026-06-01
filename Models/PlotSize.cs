namespace SMS.Models;

public class PlotSize
{
    public int PlotSizeID { get; set; }
    public string PlotSizeValue { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<MInvPlotFlat> MInvPlotFlats { get; set; } = new List<MInvPlotFlat>();
}

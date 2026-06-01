namespace SMS.Models;

public class MemberInv
{
    public int MemberInvID { get; set; }
    public string ProID { get; set; } = string.Empty;
    public DateTime? AlotTransEntryDate { get; set; }
    public string BookNo { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<MInvPlotFlat> MInvPlotFlats { get; set; } = new List<MInvPlotFlat>();
    public ICollection<MembersReg> MembersRegs { get; set; } = new List<MembersReg>();
}

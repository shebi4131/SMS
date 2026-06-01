namespace SMS.Models;

public class MemberInfo
{
    public int MemberinfoID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string CNIC { get; set; } = string.Empty;
    public string MeshipNo { get; set; } = string.Empty;
    public string OldMNo { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string CellNo { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<MembersReg> MembersRegs { get; set; } = new List<MembersReg>();
}

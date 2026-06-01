namespace SMS.Models;

public class MembersReg
{
    public int MembersRegID { get; set; }
    public int MemberInvID { get; set; }
    public int MemberSID { get; set; }

    // Navigation properties
    public MemberInfo? MemberInfo { get; set; }
    public MemberInv? MemberInv { get; set; }
}

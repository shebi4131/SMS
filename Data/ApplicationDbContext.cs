using Microsoft.EntityFrameworkCore;
using SMS.Models;

namespace SMS.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MemberInfo> MemberInfos { get; set; }
    public DbSet<MembersReg> MembersRegs { get; set; }
    public DbSet<MemberInv> MemberInvs { get; set; }
    public DbSet<MInvPlotFlat> MInvPlotFlats { get; set; }
    public DbSet<PlotSize> PlotSizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure MemberInfo
        modelBuilder.Entity<MemberInfo>(entity =>
        {
            entity.HasKey(e => e.MemberinfoID);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.CNIC).HasMaxLength(20);
            entity.Property(e => e.MeshipNo).HasMaxLength(50);
            entity.Property(e => e.OldMNo).HasColumnName("oldMNo").HasMaxLength(50); // ← ADD THIS
            entity.Property(e => e.Address1).HasMaxLength(500);
            entity.Property(e => e.CellNo).HasMaxLength(20);
        });

        // Configure MembersReg
        modelBuilder.Entity<MembersReg>(entity =>
        {
            entity.HasKey(e => e.MembersRegID);
            entity.HasOne(e => e.MemberInfo)
                .WithMany(m => m.MembersRegs)
                .HasForeignKey(e => e.MemberSID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.MemberInv)
                .WithMany(m => m.MembersRegs)
                .HasForeignKey(e => e.MemberInvID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MemberInv
        modelBuilder.Entity<MemberInv>(entity =>
        {
            entity.HasKey(e => e.MemberInvID);
            entity.Property(e => e.ProID).HasMaxLength(50);
            entity.Property(e => e.BookNo).HasMaxLength(50);
        });

        // Configure MInvPlotFlat
        modelBuilder.Entity<MInvPlotFlat>(entity =>
        {
            entity.HasKey(e => e.MInvPlotFlatID);
            entity.Property(e => e.PFlotNo).HasMaxLength(50);
            entity.Property(e => e.PFlotNo2).HasMaxLength(50);
            entity.Property(e => e.StNo).HasMaxLength(50);
            entity.HasOne(e => e.MemberInv)
                .WithMany(m => m.MInvPlotFlats)
                .HasForeignKey(e => e.MemberInvID)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.PlotSize)
                .WithMany(p => p.MInvPlotFlats)
                .HasForeignKey(e => e.PlotSizeID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PlotSize
        modelBuilder.Entity<PlotSize>(entity =>
        {
            entity.HasKey(e => e.PlotSizeID);
            entity.Property(e => e.PlotSizeValue)
                  .HasColumnName("PlotSize")  // ← ADD THIS
                  .HasMaxLength(50);
        });

        // Configure table names to match your database
        modelBuilder.Entity<MemberInfo>().ToTable("Tbl_Memberinfo");
        modelBuilder.Entity<MembersReg>().ToTable("Tbl_MembersReg");
        modelBuilder.Entity<MemberInv>().ToTable("Tbl_MemberInv");
        modelBuilder.Entity<MInvPlotFlat>().ToTable("Tbl_MInvPlotFlat");
        modelBuilder.Entity<PlotSize>().ToTable("Tbl_PlotSize");
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Entities;

public partial class EmployeeDocumentDbsContextNew : DbContext
{
    public EmployeeDocumentDbsContextNew()
    {
    }

    public EmployeeDocumentDbsContextNew(DbContextOptions<EmployeeDocumentDbsContextNew> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminUser> AdminUsers { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    //public virtual DbSet<File> Files { get; set; }
    public virtual DbSet<Files> EmployeeFiles { get; set; }


    public virtual DbSet<Historyleave> Historyleaves { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<PasswordReset> PasswordResets { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkExperience> WorkExperiences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EmployeeDocumentDBS;User Id=sa;Password=panatda1122;TrustServerCertificate=yes;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__AdminUse__719FE4E88324CF22");

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__1ABEEF6F5B8AA7D9");

            entity.ToTable("Document");

            entity.Property(e => e.DocumentId).HasColumnName("DocumentID");
            entity.Property(e => e.ApprovedDate).HasColumnName("approvedDate");
            entity.Property(e => e.Contact)
                .HasMaxLength(10)
                .HasColumnName("contact");
            entity.Property(e => e.Createdate)
                .HasColumnType("datetime")
                .HasColumnName("createdate");
            entity.Property(e => e.Enddate)
                .HasColumnType("datetime")
                .HasColumnName("enddate");
            entity.Property(e => e.FriendeContact)
                .HasMaxLength(255)
                .HasColumnName("friende_contact");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.HrApprovedDate).HasColumnName("hrApprovedDate");
            entity.Property(e => e.HrSignature)
                .HasMaxLength(255)
                .HasColumnName("hrSignature");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeId");
            entity.Property(e => e.LeavedEnddate)
                .HasColumnType("datetime")
                .HasColumnName("leaved_enddate");
            entity.Property(e => e.LeavedStartdate)
                .HasColumnType("datetime")
                .HasColumnName("leaved_startdate");
            entity.Property(e => e.LeavedType).HasColumnName("leaved_Type");
            entity.Property(e => e.ManagerComment)
                .HasMaxLength(255)
                .HasColumnName("managerComment");
            entity.Property(e => e.ManagerName)
                .HasMaxLength(100)
                .HasColumnName("managerName");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.Rolesid).HasColumnName("rolesid");
            entity.Property(e => e.SentToHrdate).HasColumnName("sentToHRDate");
            entity.Property(e => e.Startdate)
                .HasColumnType("datetime")
                .HasColumnName("startdate");
            entity.Property(e => e.Totalleave).HasColumnName("totalleave");
            entity.Property(e => e.Totalleaved).HasColumnName("totalleaved");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Workingstart)
                .HasColumnType("datetime")
                .HasColumnName("workingstart");

        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PK__Educatio__4BBE38E5E2FD510F");

            entity.Property(e => e.EducationId).HasColumnName("EducationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FieldOfStudy).HasMaxLength(255);
            entity.Property(e => e.Gpa)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("GPA");
            entity.Property(e => e.Institute).HasMaxLength(255);
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Year)
                .HasMaxLength(9)
                .IsUnicode(false);
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__Files__6F0F989F0F6CF93D");

            entity.Property(e => e.FileId).HasColumnName("FileID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Historyleave>(entity =>
        {
            entity.HasKey(e => e.Historyleaveid).HasName("PK__historyl__583E125F1C4E7CC1");

            entity.ToTable("historyleave");

            entity.Property(e => e.Historyleaveid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("historyleaveid");
            entity.Property(e => e.DocumentId).HasColumnName("DocumentID");
            entity.Property(e => e.LastTotalMaternityDaystotal).HasColumnName("last_total_maternityDaystotal");
            entity.Property(e => e.LastTotalOrdinationDays).HasColumnName("last_total_ordinationDays");
            entity.Property(e => e.LastTotalPersonDay).HasColumnName("last_total_personDay");
            entity.Property(e => e.LastTotalStickDay).HasColumnName("last_total_stickDay");
            entity.Property(e => e.LastTotalVacationDays).HasColumnName("last_total_vacationDays");
            entity.Property(e => e.SumMaternityDaystotal).HasColumnName("sum_maternityDaystotal");
            entity.Property(e => e.SumOrdinationDays).HasColumnName("sum_ordinationDays");
            entity.Property(e => e.SumPersonDay).HasColumnName("sum_personDay");
            entity.Property(e => e.SumStickDay).HasColumnName("sum_stickDay");
            entity.Property(e => e.SumVacationDays).HasColumnName("sum_vacationDays");
            entity.Property(e => e.TotalMaternityDaystotal).HasColumnName("total_maternityDaystotal");
            entity.Property(e => e.TotalOrdinationDays).HasColumnName("total_ordinationDays");
            entity.Property(e => e.TotalPersonDay).HasColumnName("total_personDay");
            entity.Property(e => e.TotalStickDay).HasColumnName("total_stickDay");
            entity.Property(e => e.TotalVacationDays).HasColumnName("total_vacationDays");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeid).HasName("PK__leaveTyp__CEF9365028D9ADE6");

            entity.ToTable("leaveType");

            entity.Property(e => e.LeaveTypeid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("leaveTypeid");
            entity.Property(e => e.LeaveTypeTh)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("leaveType_TH");
        });

        modelBuilder.Entity<PasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC07E5AD4301");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.ResetToken).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserEmail).HasMaxLength(255);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Rolesid).HasName("PK__roles__5B0A6E8C9D24C432");

            entity.ToTable("roles");

            entity.Property(e => e.Rolesid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("rolesid");
            entity.Property(e => e.Rolesname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("rolesname");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACF76CBC8D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105341E47BFCF").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CanViewAllData).HasDefaultValue(false);
            entity.Property(e => e.Contact).HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasDefaultValue("None");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Jdate).HasColumnName("JDate");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("Employee");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<WorkExperience>(entity =>
        {
            entity.HasKey(e => e.ExperienceId).HasName("PK__WorkExpe__2F4E34690BA71C16");

            entity.Property(e => e.ExperienceId).HasColumnName("ExperienceID");
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.EndDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.JobTitle).HasMaxLength(200);
            entity.Property(e => e.StartDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.WorkExperiences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__WorkExper__UserI__4316F928");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

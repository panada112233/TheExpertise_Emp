using Microsoft.EntityFrameworkCore;
using TheExpertise_Emp.Entities;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Data
{
    public class EmployeeDocumentDBSContext : DbContext
    {
        public EmployeeDocumentDBSContext(DbContextOptions<EmployeeDocumentDBSContext> options)
            : base(options)
        {
        }


        // เพิ่ม DbSet สำหรับแต่ละโมเดล
        public DbSet<AdminUsers> AdminUsers { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Educations> Educations { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<PasswordResets> PasswordResets { get; set; }
        public DbSet<WorkExperiences> WorkExperiences { get; set; }



    }
}

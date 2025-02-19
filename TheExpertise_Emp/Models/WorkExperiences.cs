using System;
using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class WorkExperiences
    {
        [Key]
        public int ExperienceID { get; set; } // รหัสประสบการณ์ทำงาน (Primary Key)

        [Required]
        public int UserID { get; set; } // รหัสผู้ใช้ (Foreign Key)

        [Required]
        [MaxLength(255)]
        public string JobTitle { get; set; } // ตำแหน่งงาน

        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; } // ชื่อบริษัท

        [Required]
        public string StartDate { get; set; } // วันที่เริ่มทำงาน

        public string? EndDate { get; set; } // วันที่สิ้นสุดการทำงาน (Nullable)

        [MaxLength(2000)]
        public string? Description { get; set; } // รายละเอียดประสบการณ์ (Nullable)

        [Required]
        public int? Salary { get; set; }

    }
}

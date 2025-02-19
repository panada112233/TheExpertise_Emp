using System;
using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class Educations
    {
        [Key]
        public int EducationID { get; set; } // รหัสการศึกษา (Primary Key)

        [Required]
        public int UserID { get; set; } // รหัสผู้ใช้ (Foreign Key)

        [Required]
        [MaxLength(50)]
        public string? Level { get; set; } // ระดับการศึกษา (เช่น ปริญญาตรี, ปริญญาโท)

        [Required]
        [MaxLength(255)]
        public string? Institute { get; set; } // ชื่อสถาบันการศึกษา

        [Required]
        [MaxLength(255)]
        public string? FieldOfStudy { get; set; } // สาขาวิชา

        [Required]
        public string Year { get; set; } // ปีที่ศึกษา

        [Range(0.00, 4.00)]
        public decimal? GPA { get; set; } // เกรดเฉลี่ยสะสม (อาจเป็น NULL ได้)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่บันทึก

        public DateTime? UpdatedAt { get; set; } = DateTime.Now; // วันที่แก้ไขข้อมูลล่าสุด
    }
}

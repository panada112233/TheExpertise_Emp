using System;
using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class Files
    {
        [Key]
        public int FileID { get; set; } // รหัสไฟล์ (Primary Key)

        [Required]
        public int UserID { get; set; } // รหัสผู้ใช้ (Foreign Key)

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } // ที่อยู่ไฟล์ (Path ในเซิร์ฟเวอร์)

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } // ประเภทไฟล์ (เช่น PDF, DOCX, JPG)

        
        [MaxLength(50)]
        public string? Category { get; set; } // หมวดหมู่ไฟล์ (เช่น Contract, Report)

        
        [MaxLength(255)]
        public string? Description { get; set; } // คำอธิบายไฟล์

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now; // วันที่อัปโหลดไฟล์

        [Required]
        public bool IsActive { get; set; } = true; // สถานะการใช้งานไฟล์ (true = ใช้งานอยู่)
    }
}

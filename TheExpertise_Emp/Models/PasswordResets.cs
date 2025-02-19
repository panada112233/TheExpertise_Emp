using System;
using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class PasswordResets
    {
        [Key]
        public int Id { get; set; } // รหัสการรีเซ็ตรหัสผ่าน (Primary Key)

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string UserEmail { get; set; } // อีเมลผู้ใช้

        [Required]
        [MaxLength(255)]
        public string ResetToken { get; set; } // โทเค็นที่ใช้สำหรับรีเซ็ตรหัสผ่าน

        [Required]
        public DateTime ExpirationDate { get; set; } // วันที่หมดอายุของโทเค็น

        [Required]
        public bool IsUsed { get; set; } = false; // สถานะการใช้งานโทเค็น (false = ยังไม่ใช้)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่สร้างการรีเซ็ตรหัสผ่าน

        public DateTime UpdatedAt { get; set; } = DateTime.Now; // วันที่อัปเดตข้อมูลล่าสุด
    }
}

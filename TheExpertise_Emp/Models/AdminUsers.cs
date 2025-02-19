using System;
using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class AdminUsers
    {
        [Key]
        public int AdminID { get; set; } // รหัสผู้ดูแลระบบ (Primary Key)

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } // ชื่อผู้ใช้

        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } // รหัสผ่านที่เข้ารหัส

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } // อีเมลสำหรับติดต่อ

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่สร้างบัญชี

        [Required]
        public bool IsActive { get; set; } = true; // สถานะการใช้งาน (1 = ใช้งาน, 0 = ไม่ใช้งาน)

        // เพิ่มคอลัมน์สำหรับเก็บ URL ของรูปโปรไฟล์
        public string? ProfilePictureUrl { get; set; } // URL ของรูปโปรไฟล์

    }
    public class LoginModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        public string Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        public string Password { get; set; }
    }
}

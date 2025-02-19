using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ต้องใช้สำหรับ [NotMapped]

namespace TheExpertise_Emp.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; } // รหัสผู้ใช้ (Primary Key)

        //[Required]
        [MaxLength(100)]
        public string? FirstName { get; set; } // ชื่อ

        //[Required]
        [MaxLength(100)]
        public string? LastName { get; set; } // นามสกุล

        //[Required]
        [MaxLength(256)]
        [EmailAddress]
        public string? Email { get; set; } // อีเมล (ต้องเป็นรูปแบบอีเมล)

        //[Required]
        [MaxLength(256)]
        public string? PasswordHash { get; set; } // รหัสผ่านที่เข้ารหัส

        [MaxLength(50)]
        public string? Role { get; set; } // บทบาท (ค่าเริ่มต้น: Employee)

        [MaxLength(100)]
        public string? Designation { get; set; } // ตำแหน่ง

        [MaxLength(20)]
        public string? Contact { get; set; } // เบอร์โทรศัพท์

        [MaxLength(10)]
        public string? Gender { get; set; } // เพศ (ค่าเริ่มต้น: None)

        public bool IsActive { get; set; } = true; // สถานะการใช้งาน (1 = ใช้งาน, 0 = ไม่ใช้งาน)

        public DateTime CreatedAt { get; set; } = DateTime.Now; // วันที่สร้างบัญชี

        public DateTime? UpdatedAt { get; set; } = DateTime.Now; // วันที่อัปเดตข้อมูลล่าสุด
        public DateTime? JDate { get; set; }
        public bool? CanViewAllData { get; set; } = false; // ค่าเริ่มต้น
    }
}

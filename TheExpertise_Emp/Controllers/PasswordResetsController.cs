using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetsController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;
        private readonly EmailService _emailService;

        public PasswordResetsController(EmployeeDocumentDBSContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("reset-request")]
        public async Task<IActionResult> CreatePasswordResetRequest([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.Email))
                return BadRequest("กรุณาระบุอีเมล");

            // ค้นหาใน Users และ AdminUsers
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailRequest.Email);
            var admin = await _context.AdminUsers.FirstOrDefaultAsync(a => a.Email == emailRequest.Email);

            if (user == null && admin == null)
                return NotFound("อีเมลนี้ไม่พบในระบบ");

            // สร้างรหัสผ่านใหม่
            var newPassword = GenerateRandomPassword();
            if (user != null) await UpdatePasswordAsync(user, newPassword);
            if (admin != null) await UpdatePasswordAsync(admin, newPassword);

            // อัปเดตรหัสผ่านในตารางที่ตรงกัน
            if (user != null)
            {
                user.PasswordHash = newPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                _context.Users.Update(user);
            }

            if (admin != null)
            {
                admin.PasswordHash = newPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                _context.AdminUsers.Update(admin);
            }

            await _context.SaveChangesAsync();

            // ส่งอีเมลพร้อมรหัสผ่านใหม่
            await _emailService.SendEmailAsync(
                emailRequest.Email,
                "รหัสผ่านใหม่ของคุณ",
                $"<p>รหัสผ่านใหม่ของคุณคือ:</p> " +
                $"<h3>{newPassword}</h3>" +
                $"<p style='color: red; font-weight: bold;'>กรุณาเปลี่ยนรหัสผ่านทันทีเพื่อความปลอดภัย</p>"
            );

            return Ok("รหัสผ่านใหม่ถูกส่งไปที่อีเมลของคุณแล้ว");
        }
        private async Task UpdatePasswordAsync(object entity, string newPassword)
        {
            switch (entity)
            {
                case Users user:
                    user.PasswordHash = newPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                    _context.Users.Update(user);
                    break;
                case AdminUsers admin:
                    admin.PasswordHash = newPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                    _context.AdminUsers.Update(admin);
                    break;
            }
            await _context.SaveChangesAsync();
        }

        private string GenerateRandomPassword()
        {
            const string validChars = "ABCDEFGHJKLMghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            var password = new char[8];

            for (int i = 0; i < password.Length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(password);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

    }

}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.DTO;
using TheExpertise_Emp.Entities;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;
        private readonly EmployeeDocumentDbsContextNew _contextExp;
        public UsersController(EmployeeDocumentDBSContext context, EmployeeDocumentDbsContextNew contextNew)
        {
            _context = context;
            _contextExp = contextNew;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Create(Users user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest("อีเมลนี้มีผู้ใช้งานแล้ว.");
            }

            var data = new Users
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };

            _context.Users.Add(data);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { id = user.UserID }, user);
        }
        [HttpGet("GetRoles")]
        public async Task<IActionResult>  GetRoles() 
        {
            try
            {
                var role = await _contextExp.Roles.ToListAsync();

                return Ok(role);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Login")]
        public IActionResult Login(Users user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);

            if (existingUser == null)
            {
                return Unauthorized("อีเมลไม่ถูกต้อง");
            }

            if (existingUser.PasswordHash != user.PasswordHash)
            {
                return Unauthorized("รหัสผ่านไม่ถูกต้อง");
            }

            var dtoRespones = new DTORespone()
            {
                role = existingUser.Role,
                message = "success",
                userid = existingUser.UserID,
                username = existingUser.Email
            };

            return Ok(dtoRespones);

            // เพิ่มการส่ง Role กลับไป
            //return Ok(new
            //{
            //    message = "เข้าสู่ระบบสำเร็จ",
            //    userid = existingUser.UserID,
            //    role = existingUser.Role // ใช้ Role จาก Users
            //});
        }


        [HttpGet]
        [Route("Getbyid/{id}")]
        public IActionResult Getusersbyid(int id)
        {
            var usersobject = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (usersobject is object)
            {
                return Ok(usersobject);
            }
            return BadRequest("ไม่พบข้อมูลของ usersid = " + id);
        }

        [HttpGet("Profile/{id}")]
        public IActionResult Profile(int id)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (existingUser is object)
            {
                return Ok(existingUser);
            }
            return BadRequest("ไม่พบข้อมูลของ usersid = " + id);
        }

        [HttpPost("Update")]
        public IActionResult UpdateProfile(Users user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == user.UserID);
            if (existingUser is object)
            {
                // อัปเดตข้อมูลในฐานข้อมูล
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.PasswordHash = user.PasswordHash;
                existingUser.Role = user.Role;
                existingUser.Designation = user.Designation;
                existingUser.Contact = user.Contact;
                existingUser.Gender = user.Gender;
                existingUser.JDate = user.JDate;
                existingUser.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                return Ok(new { message = "อัปเดตข้อมูลสำเร็จ" });
            }
            return BadRequest("ไม่พบข้อมูลของ UserID = " + user.UserID);
        }

        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "ข้อมูลที่ส่งมาไม่ถูกต้อง" });
                }

                // ตรวจสอบว่ารหัสผ่านเก่าและใหม่ถูกส่งมาหรือไม่
                if (string.IsNullOrWhiteSpace(model.OldPassword) || string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    return BadRequest(new { message = "กรุณากรอกรหัสผ่านเก่าและรหัสผ่านใหม่" });
                }

                // ดึงผู้ใช้จาก Users และ AdminUsers โดยใช้รหัสผ่านเก่า
                var existingUser = _context.Users.FirstOrDefault(u => u.PasswordHash == model.OldPassword);
                var existingAdmin = _context.AdminUsers.FirstOrDefault(a => a.PasswordHash == model.OldPassword);

                if (existingUser == null && existingAdmin == null)
                {
                    return BadRequest(new { message = "รหัสผ่านเดิมไม่ถูกต้อง" });
                }

                // ตรวจสอบความปลอดภัยของรหัสผ่านใหม่
                if (model.NewPassword.Length < 6)
                {
                    return BadRequest(new { message = "รหัสผ่านใหม่ต้องมีความยาวอย่างน้อย 6 ตัวอักษร" });
                }

                // อัปเดตรหัสผ่านใหม่ใน Users หรือ AdminUsers
                if (existingUser != null)
                {
                    existingUser.PasswordHash = model.NewPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                    existingUser.UpdatedAt = DateTime.Now;
                    _context.Users.Update(existingUser);
                }

                if (existingAdmin != null)
                {
                    existingAdmin.PasswordHash = model.NewPassword; // เก็บเป็น Plain Text ตามที่คุณต้องการ
                    
                    _context.AdminUsers.Update(existingAdmin);
                }

                _context.SaveChanges();

                return Ok(new { message = "เปลี่ยนรหัสผ่านสำเร็จ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ข้อผิดพลาดภายในระบบ", error = ex.Message });
            }
        }

        // คลาสสำหรับรับข้อมูล ChangePassword
        public class ChangePasswordRequest
        {
            public string OldPassword { get; set; } // รหัสผ่านเก่า
            public string NewPassword { get; set; } // รหัสผ่านใหม่
        }

    }
}
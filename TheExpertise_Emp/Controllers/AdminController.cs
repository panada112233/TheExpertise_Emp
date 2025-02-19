using Microsoft.AspNetCore.Mvc;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TheExpertise_Emp.DTO;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;

        public AdminController(EmployeeDocumentDBSContext context)
        {
            _context = context;
        }

        // API สำหรับล็อกอินผู้ดูแลระบบ
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            // ตรวจสอบว่า ModelState ถูกต้องหรือไม่
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ตรวจสอบว่าชื่อผู้ใช้และรหัสผ่านไม่เป็นค่าว่าง
            if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest(new { message = "ชื่อผู้ใช้หรือรหัสผ่านไม่สามารถเป็นค่าว่างได้" });
            }

            // ค้นหา AdminUser จากฐานข้อมูลโดยใช้ Username
            var adminUser = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Username == login.Username
                && a.PasswordHash == login.Password
                && a.IsActive);

            // ตรวจสอบว่าพบผู้ใช้งานหรือไม่
            if (adminUser == null)
            {
                return Unauthorized(new { message = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง" });
            }

            var dtoRespones = new DTORespone()
            {
                role = "admin",
                message = "success",
                userid = adminUser.AdminID,
                username = adminUser.Username
            };


            // ส่งคืนข้อมูลของผู้ใช้ที่เข้าสู่ระบบสำเร็จ
            return Ok(dtoRespones);
        }

        [HttpGet("GetUserRole")]
        public IActionResult GetUserRole()
        {
            try
            {
                var currentUserId = HttpContext.Session.GetInt32("UserID");
                if (!currentUserId.HasValue)
                {
                    return Unauthorized(new { message = "ไม่ได้เข้าสู่ระบบ" });
                }

                var userRole = _context.Users
                    .Where(u => u.UserID == currentUserId)
                    .Select(u => u.Role)
                    .FirstOrDefault();

                if (userRole == null)
                {
                    return NotFound(new { message = "ไม่พบ Role ของผู้ใช้งาน" });
                }

                return Ok(new { Role = userRole });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "เกิดข้อผิดพลาด", error = ex.Message });
            }
        }

        // สมัครแอดมิน
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminUsers newAdmin)
        {
            if (newAdmin == null || string.IsNullOrWhiteSpace(newAdmin.Username) || string.IsNullOrWhiteSpace(newAdmin.PasswordHash))
            {
                return BadRequest(new { message = "ข้อมูลไม่ถูกต้อง: ชื่อผู้ใช้และรหัสผ่านจำเป็นต้องมีค่า" });
            }

            try
            {
                // ตรวจสอบว่าชื่อผู้ใช้ซ้ำหรือไม่
                var existingAdmin = await _context.AdminUsers
                    .FirstOrDefaultAsync(a => a.Username == newAdmin.Username);

                if (existingAdmin != null)
                {
                    return Conflict(new { message = "ชื่อผู้ใช้นี้มีอยู่ในระบบแล้ว" });
                }

                // ตั้งค่าข้อมูลเพิ่มเติม
                newAdmin.CreatedAt = DateTime.Now;
                newAdmin.IsActive = true;

                // เพิ่มข้อมูลลงฐานข้อมูล
                _context.AdminUsers.Add(newAdmin);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "สมัครแอดมินสำเร็จ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "เกิดข้อผิดพลาดในเซิร์ฟเวอร์", error = ex.Message });
            }
        }

        [HttpGet("GetAdminInfo")]
        public async Task<IActionResult> GetAdminInfo(string adminid)
        {
            try
            {
                // ดึงข้อมูลผู้ดูแลระบบจากฐานข้อมูล
                var admin = await _context.AdminUsers.FirstOrDefaultAsync(u => u.AdminID == int.Parse(adminid));

                if (admin == null)
                {
                    return NotFound(new { Message = "ไม่พบผู้ดูแลระบบ" });
                }

                // ส่งข้อมูลพร้อม userid
                return Ok(new
                {
                    userid = admin.AdminID,
                    name = admin.Username,
                    email = admin.Email,
                    profilePictureUrl = admin.ProfilePictureUrl ?? "/uploads/admin/default-profile.jpg"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost]
        [Route("UpdateAdminInfo")]
        public async Task<IActionResult> UpdateAdminInfo(
     [FromForm] string? name = null, // ชื่อเป็นออปชัน
     [FromForm] string? email = null, // อีเมลเป็นออปชัน
     [FromForm] List<IFormFile>? profilePictures = null,
     [FromForm] string? id = null) // รองรับอัปโหลดรูปโปรไฟล์
        {
            try
            {
                // ตรวจสอบว่า ID ถูกต้องหรือไม่
                if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int adminId))
                {
                    return BadRequest(new { Message = "ID ไม่ถูกต้องหรือไม่ได้รับ" });
                }

                // ดึงข้อมูลผู้ดูแลระบบจากฐานข้อมูล
                var admin = await _context.AdminUsers.FirstOrDefaultAsync(u => u.AdminID == adminId);
                if (admin == null)
                {
                    return NotFound(new { Message = "ไม่พบผู้ดูแลระบบ" });
                }

                // อัปเดตชื่อผู้ดูแลระบบ ถ้ามีการระบุ
                if (!string.IsNullOrWhiteSpace(name))
                {
                    admin.Username = name;
                }

                // อัปเดตอีเมล ถ้ามีการระบุ
                if (!string.IsNullOrWhiteSpace(email))
                {
                    admin.Email = email;
                }

                // จัดการการอัปโหลดรูปโปรไฟล์
                if (profilePictures != null && profilePictures.Count > 0)
                {
                    var uploadedFiles = new List<string>();
                    var uploadsFolder = Path.Combine("wwwroot", "uploads", "admin");

                    // ตรวจสอบและสร้างโฟลเดอร์หากไม่มี
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    foreach (var profilePicture in profilePictures)
                    {
                        // ตรวจสอบนามสกุลไฟล์
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                        var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            return BadRequest(new { Message = $"ไฟล์ {profilePicture.FileName} ไม่รองรับ" });
                        }

                        // ตั้งชื่อไฟล์ใหม่
                        var fileName = Guid.NewGuid().ToString() + fileExtension;
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // บันทึกไฟล์
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await profilePicture.CopyToAsync(stream);
                        }

                        // เก็บ URL ของไฟล์ที่อัปโหลด
                        uploadedFiles.Add($"/uploads/admin/{fileName}");
                    }

                    // อัปเดต URL ของรูปโปรไฟล์
                    admin.ProfilePictureUrl = uploadedFiles.FirstOrDefault();
                }

                // บันทึกการเปลี่ยนแปลงในฐานข้อมูล
                await _context.SaveChangesAsync();

                // ส่งคืนข้อมูลที่อัปเดต
                return Ok(new
                {
                    Name = admin.Username,
                    Email = admin.Email,
                    ProfilePictureUrl = admin.ProfilePictureUrl ?? "/uploads/admin/default-profile.jpg",
                    Message = "อัปเดตข้อมูลสำเร็จ!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"เกิดข้อผิดพลาด: {ex.Message}" });
            }
        }


        // ดึงข้อมูลพนักงานทั้งหมด
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // ดึงข้อมูลพนักงานตาม ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลพนักงาน" });
            }
            return Ok(user);
        }

        // เพิ่มพนักงานใหม่
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] Users newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (newUser.Role == "Hr" || newUser.Role == "GM")
            {
                newUser.CanViewAllData = true;
            }
            else
            {
                newUser.CanViewAllData = false;
            }

            // ตรวจสอบอีเมลซ้ำ
            var existingUser = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
            if (existingUser)
            {
                return Conflict(new { message = "อีเมลนี้มีอยู่ในระบบแล้ว" });
            }

            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // แก้ไขข้อมูลพนักงาน
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] Users updatedUser)
        {
            if (id != updatedUser.UserID)
            {
                return BadRequest(new { message = "ID ไม่ตรงกัน" });
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลพนักงาน" });
            }

            // ตั้งค่า CanViewAllData
            if (updatedUser.Role == "Hr" || updatedUser.Role == "GM")
            {
                existingUser.CanViewAllData = true;
            }
            else
            {
                existingUser.CanViewAllData = false;
            }

            // อัปเดตข้อมูล
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.PasswordHash = updatedUser.PasswordHash;
            existingUser.Role = updatedUser.Role;
            existingUser.Designation = updatedUser.Designation;
            existingUser.JDate = updatedUser.JDate;
            existingUser.Contact = updatedUser.Contact;
            existingUser.Gender = updatedUser.Gender;
            existingUser.IsActive = updatedUser.IsActive;
            existingUser.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("ManageData")]
        public async Task<IActionResult> ManageData()
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // ดึง AdminID จาก Session
                var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(a => a.AdminID == currentUserId);

                // ตรวจสอบสิทธิ์
                if (adminUser == null || !adminUser.IsActive)
                {
                    return Forbid("คุณไม่มีสิทธิ์ในการจัดการข้อมูล");
                }

                // ดำเนินการจัดการข้อมูลที่ต้องการ
                return Ok("จัดการข้อมูลสำเร็จ");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ลบพนักงาน
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลพนักงาน" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ดึงข้อมูลการศึกษาทั้งหมด
        [HttpGet("educations")]
        public async Task<IActionResult> GetEducations()
        {
            var educations = await _context.Educations.ToListAsync();
            return Ok(educations);
        }

        // ดึงข้อมูลการศึกษาตาม ID
        [HttpGet("educations/{id}")]
        public async Task<IActionResult> GetEducationById(int id)
        {
            var education = await _context.Educations.FindAsync(id);
            if (education == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลการศึกษาที่ระบุ" });
            }
            return Ok(education);
        }

        // API: เพิ่มข้อมูลการศึกษา
        [HttpPost("educations")]
        public async Task<IActionResult> CreateEducation([FromBody] Educations newEducation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // ตรวจสอบว่า newEducation ถูกต้องหรือไม่ก่อนบันทึก
            if (newEducation == null || string.IsNullOrEmpty(newEducation.Level))
            {
                return BadRequest("ข้อมูลไม่ถูกต้อง");
            }

            _context.Educations.Add(newEducation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEducations), new { id = newEducation.EducationID }, newEducation);
        }


        // แก้ไขข้อมูลการศึกษา
        [HttpPut("educations/{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] Educations updatedEducation)
        {
            if (id != updatedEducation.EducationID)
            {
                return BadRequest(new { message = "ID ไม่ตรงกัน" });
            }

            var existingEducation = await _context.Educations.FindAsync(id);
            if (existingEducation == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลการศึกษา" });
            }

            existingEducation.Level = updatedEducation.Level;
            existingEducation.Institute = updatedEducation.Institute;
            existingEducation.FieldOfStudy = updatedEducation.FieldOfStudy;
            existingEducation.Year = updatedEducation.Year;
            existingEducation.GPA = updatedEducation.GPA;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // API: ลบข้อมูลการศึกษา
        [HttpDelete("educations/{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var education = await _context.Educations.FindAsync(id);
            if (education == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลการศึกษา" });
            }

            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _context.Files.ToListAsync();
            return Ok(files);
        }

        [HttpDelete("files/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid file ID.");
            }

            try
            {
                var file = await _context.Files.FindAsync(id);
                if (file == null)
                {
                    return NotFound($"File with ID {id} not found.");
                }

                // ลบไฟล์ในระบบไฟล์ (ถ้ามี)
                if (!string.IsNullOrEmpty(file.FilePath) && System.IO.File.Exists(file.FilePath))
                {
                    System.IO.File.Delete(file.FilePath);
                }

                // ลบไฟล์จากฐานข้อมูล
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("WorkExperiences")]
        public async Task<IActionResult> GetWorkExperiences()
        {
            var workExperiences = await _context.WorkExperiences.ToListAsync();
            return Ok(workExperiences);
        }

        // ดึงข้อมูลการทำงานตาม ID
        [HttpGet("WorkExperiences/{id}")]
        public async Task<IActionResult> GetWorkExperienceById(int id)
        {
            var workExperience = await _context.WorkExperiences.FindAsync(id);
            if (workExperience == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลประสบการณ์การทำงานที่ระบุ" });
            }
            return Ok(workExperience);
        }

        // แก้ไขข้อมูลการทำงาน
        [HttpPut("WorkExperiences/{id}")]
        public async Task<IActionResult> UpdateWorkExperience(int id, [FromBody] WorkExperiences updatedWorkExperience)
        {
            if (id != updatedWorkExperience.ExperienceID)
            {
                return BadRequest(new { message = "ID ไม่ตรงกัน" });
            }

            var existingWorkExperience = await _context.WorkExperiences.FindAsync(id);
            if (existingWorkExperience == null)
            {
                return NotFound(new { message = "ไม่พบข้อมูลประสบการณ์การทำงาน" });
            }

            existingWorkExperience.JobTitle = updatedWorkExperience.JobTitle;
            existingWorkExperience.CompanyName = updatedWorkExperience.CompanyName;
            existingWorkExperience.StartDate = updatedWorkExperience.StartDate;
            existingWorkExperience.EndDate = updatedWorkExperience.EndDate;
            existingWorkExperience.Description = updatedWorkExperience.Description;
            existingWorkExperience.Salary = updatedWorkExperience.Salary;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // เพิ่มข้อมูลการทำงาน (POST)
        [HttpPost("WorkExperiences")]
        public async Task<IActionResult> AddWorkExperience([FromBody] WorkExperiences newWorkExperience)
        {
            if (newWorkExperience == null)
            {
                return BadRequest(new { message = "ข้อมูลไม่สมบูรณ์" });
            }

            // เพิ่มข้อมูลใหม่ลงในฐานข้อมูล
            _context.WorkExperiences.Add(newWorkExperience);
            await _context.SaveChangesAsync();

            // ส่งการตอบกลับที่มีข้อมูลใหม่ที่เพิ่มเข้าไป
            return CreatedAtAction(nameof(GetWorkExperienceById), new { id = newWorkExperience.ExperienceID }, newWorkExperience);
        }


        [HttpDelete("WorkExperiences/{id}")]
        public async Task<IActionResult> DeleteWorkExperience(int id)
        {
            var experience = await _context.WorkExperiences.FindAsync(id);
            if (experience == null)
            {
                return NotFound();
            }

            _context.WorkExperiences.Remove(experience);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);


            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("Users")]
        public async Task<IActionResult> CreateUsers([FromBody] Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user }, user);
        }

        [HttpPut("Users/{id}")]
        public async Task<IActionResult> UpdateUsers(int id, [FromBody] Users updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Role = updatedUser.Role;
            user.Contact = updatedUser.Contact;
            user.Designation = updatedUser.Designation;
            user.JDate = updatedUser.JDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // ดึงข้อมูลผู้ใช้งานทั้งหมด
        [HttpGet("user")]
        public async Task<IActionResult> GetUserss()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // Delete ผู้ใช้งาน
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue)
            {
                throw new Exception("ไม่พบข้อมูลผู้ใช้งานใน Session");
            }
            return userId.Value;
        }

        [HttpDelete("DeleteAdmin/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.AdminUsers.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "ไม่พบแอดมินที่ระบุ" });
            }

            _context.AdminUsers.Remove(admin);
            await _context.SaveChangesAsync();
            return Ok(new { message = "ลบแอดมินสำเร็จ" });
        }
        [HttpGet("WorkExperiences/user/{userID}")]
        public async Task<IActionResult> GetWorkExperiencesByUserId(int userID)
        {
            var workExperiences = await _context.WorkExperiences
                .Where(w => w.UserID == userID) // กรองข้อมูลตาม userID
                .ToListAsync();

            if (workExperiences == null || !workExperiences.Any())
            {
                return NotFound(new { message = "ไม่พบข้อมูลประสบการณ์การทำงานของผู้ใช้งานนี้" });
            }
            return Ok(workExperiences);
        }

        // ดึงข้อมูลการศึกษาตาม userID (API ใหม่)
        [HttpGet("educations/user/{userID}")]
        public async Task<IActionResult> GetEducationsByUserId(int userID)
        {
            var educations = await _context.Educations
                .Where(e => e.UserID == userID)
                .ToListAsync();

            if (educations == null || !educations.Any())
            {
                return NotFound(new { message = "ไม่พบข้อมูลการศึกษาของผู้ใช้งานนี้" });
            }
            return Ok(educations);
        }

    }
}

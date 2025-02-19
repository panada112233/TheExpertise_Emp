using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.DTO;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;

        public FilesController(EmployeeDocumentDBSContext context)
        {
            _context = context;
        }

        // GET: api/Files
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()    
        {
            var files = await _context.Files
                .Where(x => x.FileType == "Document" && x.IsActive == true )
                
                .ToListAsync();
            
            return Ok(files); // ส่งข้อมูลทั้งหมดในรูปแบบ JSON
        }
     
    

        // GET: api/Files/Document
        [HttpGet("Document")]
        public async Task<IActionResult> GetAllDocumentFiles(int userID)
        {
            if (userID <= 0) // ตรวจสอบว่า userID ถูกส่งมาหรือไม่
            {
                return BadRequest("UserID is required and must be a valid integer.");
            }

            var files = await _context.Files
                .Where(e => e.FileType.Contains("Document") && e.UserID == userID) // ใช้ UserID จาก Model (ไม่ใช้ userID ตัวเล็ก)
                .ToListAsync();

            return Ok(files); // ส่งข้อมูลทั้งหมดในรูปแบบ JSON
        }

        // GET: api/Files/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileDetails(int id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(m => m.FileID == id);
            if (file == null)
            {
                return NotFound(); // ถ้าไม่พบข้อมูล ส่งสถานะ 404
            }

            // ตรวจสอบว่า Description หรือ Category เป็น NULL หรือไม่
            file.Description = file.Description ?? "No Description"; // ใช้ข้อความเริ่มต้นถ้า NULL
            file.Category = file.Category ?? "Uncategorized"; // ใช้ข้อความเริ่มต้นถ้า NULL

            return Ok(file); // ส่งข้อมูลของไฟล์ที่พบ
        }

        [HttpPost("uploaddocument")]
        public async Task<IActionResult> UploadDocument(DocumentUpload  documentupload)
        {
            if (string.IsNullOrEmpty(documentupload.userid))
                return BadRequest("User is Null");


                //var docCreate = new 


            return Ok();
        }

        // POST: api/Files/Create
        [HttpPost("Create")]
        public async Task<IActionResult> CreateFile([FromForm] FileCreatDto fileDto)
        {
            Console.WriteLine("123123");
            Console.WriteLine(ModelState.IsValid);

            if (ModelState.IsValid)
            {
                // กำหนดโฟลเดอร์สำหรับเก็บไฟล์
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // สร้างโฟลเดอร์หากไม่มี
                }

                // สร้างชื่อไฟล์ที่ไม่ซ้ำกัน
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileDto.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                var relativePath = $"/uploads/{uniqueFileName}"; // Path สำหรับให้ Frontend เรียกใช้งาน

                // บันทึกไฟล์ในเซิร์ฟเวอร์
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileDto.File.CopyToAsync(fileStream);
                }

                // บันทึกข้อมูลไฟล์ในฐานข้อมูล
                var newFile = new Files
                {
                    UserID = fileDto.UserID,
                    FilePath = relativePath, // Path สำหรับให้ Frontend ใช้งาน
                    FileType = "Document",
                    Category = fileDto.Category,

                    Description = fileDto.Description,
                    UploadDate = DateTime.Now,
                    IsActive = true
                };

                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetFileDetails), new { id = newFile.FileID }, newFile);
            }
            return BadRequest(ModelState); // ถ้ามีปัญหากับข้อมูลที่ส่งมา
        }


        // PUT: api/Files/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditFile(int id, [FromBody] Files file)
        {
            if (id != file.FileID)
            {
                return BadRequest("File ID mismatch."); // ถ้า ID ไม่ตรงกัน
            }

            _context.Entry(file).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FileExists(id))
                {
                    return NotFound(); // ถ้าไม่พบไฟล์ที่ต้องการอัปเดต
                }
                else
                {
                    throw;
                }
            }
            return NoContent(); // คืนค่า NoContent เมื่อสำเร็จ
        }

        // DELETE: api/Files/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null)
            {
                return NotFound(); // ถ้าไม่พบไฟล์ที่ต้องการลบ
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
            return NoContent(); // คืนค่า NoContent เมื่อสำเร็จ
        }

        private bool FileExists(int id)
        {
            return _context.Files.Any(e => e.FileID == id); // ตรวจสอบว่าไฟล์มีอยู่ในฐานข้อมูลหรือไม่
        }

        [HttpPost("VerifyPassword")]
        public IActionResult VerifyPassword([FromBody] PasswordVerificationRequest request)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == request.UserID);

            if (existingUser == null)
            {
                return Unauthorized("ผู้ใช้ไม่พบ");
            }

            // ตรวจสอบรหัสผ่านที่ผู้ใช้ป้อน
            if (existingUser.PasswordHash != request.PasswordHash)
            {
                return Unauthorized("รหัสผ่านไม่ถูกต้อง");
            }

            // ถ้ารหัสผ่านถูกต้อง
            return Ok(new { isValid = true });
        }


        [HttpPost("UploadProfile")]
        public async Task<IActionResult> UploadProfile(IFormFile file, int userID)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded or file is empty." });
            }

            try
            {
                // กำหนดโฟลเดอร์สำหรับเก็บไฟล์
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // สร้างโฟลเดอร์หากไม่มี
                }

                // สร้างชื่อไฟล์ที่ไม่ซ้ำกัน
                var uniqueFileName = $"{userID}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                var relativePath = $"/uploads/{uniqueFileName}"; // Path สำหรับให้ Frontend เรียกใช้งาน

                // บันทึกไฟล์ในเซิร์ฟเวอร์
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // บันทึกข้อมูลไฟล์ในฐานข้อมูล
                var newFile = new Files
                {
                    UserID = userID,
                    FilePath = relativePath, // Path สำหรับให้ Frontend ใช้งาน
                    FileType = "Profile",
                    UploadDate = DateTime.Now,
                    IsActive = true
                };

                _context.Files
      .Where(f => f.UserID == userID && f.FileType == "Profile" && f.IsActive)
      .ExecuteUpdate(e => e.SetProperty(f => f.IsActive, false));

                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();

                // ส่งข้อมูลกลับไปยัง Frontend
                return Ok(new { Message = "Profile picture updated successfully.", FilePath = newFile.FilePath });
            }
            catch (Exception ex)
            {
                // หากมีข้อผิดพลาด ให้ส่งข้อความกลับไปยัง Frontend
                return StatusCode(500, new { Message = "An error occurred while uploading the profile picture.", Error = ex.Message });
            }
        }


        [HttpGet("GetProfileImage")]
        public async Task<IActionResult> GetProfileImage(int userID)
        {
            try
            {
                var profileFile = await _context.Files
                    .Where(f => f.UserID == userID && f.FileType == "Profile" && f.IsActive)
                    .OrderByDescending(f => f.UploadDate)
                    .FirstOrDefaultAsync();

                if (profileFile == null || string.IsNullOrEmpty(profileFile.FilePath))
                {
                    return NotFound(new { Message = "No profile image found for this user." });
                }

                var fullPath = Path.Combine("wwwroot", "uploads", Path.GetFileName(profileFile.FilePath));
                if (!System.IO.File.Exists(fullPath))
                {
                    return NotFound(new { Message = "File not found on server." });
                }

                // ตรวจสอบนามสกุลไฟล์และกำหนด Content-Type ตามชนิดของไฟล์
                var fileExtension = Path.GetExtension(profileFile.FilePath)?.ToLower();
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" or ".jfif" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".bmp" => "image/bmp",
                    ".webp" => "image/webp",
                    ".svg" => "image/svg+xml",
                    ".tiff" => "image/tiff",
                    _ => "application/octet-stream", // หากไม่พบชนิดไฟล์ที่รองรับ
                };

                // ใช้ FileStreamResult เพื่อส่งไฟล์
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                return File(fileStream, contentType); // ส่งกลับด้วย content-type ที่เหมาะสม
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error occurred", Error = ex.Message });
            }
        }
        public class PasswordVerificationRequest
        {
            public int UserID { get; set; }
            public string PasswordHash { get; set; }
        }


    }
}



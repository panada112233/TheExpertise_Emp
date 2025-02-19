using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationsController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;

        public EducationsController(EmployeeDocumentDBSContext context)
        {
            _context = context;
        }

        // ดึงข้อมูลการศึกษาทั้งหมด
        [HttpGet]
        public IActionResult GetAll()
        {
            var educations = _context.Educations.ToList();
            return Ok(educations);
        }

        // ดึงข้อมูลการศึกษาตาม UserID
        [HttpGet("Getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var educations = _context.Educations.Where(e => e.UserID == id)
                                .OrderByDescending(e => e.Year)
                                .ToList();
            return Ok(educations);
        }

        // เพิ่มข้อมูลการศึกษา
        [HttpPost("Insert")]
        public IActionResult InsertEducation(Educations education)
        {
            if (!Regex.IsMatch(education.Year, @"^\d{4}-\d{4}$"))
            {
                return BadRequest("รูปแบบปีที่ศึกษาไม่ถูกต้อง (เช่น 2567-2568)");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == education.UserID);
            if (existingUser is object)
            {
                var data = new Educations
                {
                    UserID = education.UserID,
                    Level = education.Level,
                    Institute = education.Institute,
                    FieldOfStudy = education.FieldOfStudy,
                    Year = education.Year,
                    GPA = education.GPA,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Educations.Add(data);
                _context.SaveChanges();

                var userEducations = GetById(education.UserID);
                return Created("Insert success.", userEducations);
            }
            return BadRequest("ไม่พบข้อมูลของ UserID.");
        }


        // แก้ไขข้อมูลการศึกษา
        [HttpPut("Update/{id}")]
        public IActionResult UpdateEducation(int id, [FromBody] Educations updatedEducation)
        {
            if (!Regex.IsMatch(updatedEducation.Year, @"^\d{4}-\d{4}$"))
            {
                return BadRequest("รูปแบบปีที่ศึกษาไม่ถูกต้อง (เช่น 2567-2568)");
            }

            var existingEducation = _context.Educations.FirstOrDefault(e => e.EducationID == id);
            if (existingEducation == null)
            {
                return NotFound("ไม่พบข้อมูลการศึกษา.");
            }

            existingEducation.Level = updatedEducation.Level;
            existingEducation.Institute = updatedEducation.Institute;
            existingEducation.FieldOfStudy = updatedEducation.FieldOfStudy;
            existingEducation.Year = updatedEducation.Year;
            existingEducation.GPA = updatedEducation.GPA;
            existingEducation.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok("แก้ไขข้อมูลสำเร็จ.");
        }


        // ลบข้อมูลการศึกษา
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteEducation(int id)
        {
            var education = _context.Educations.FirstOrDefault(e => e.EducationID == id);
            if (education is null)
            {
                return NotFound("ไม่พบข้อมูลการศึกษา.");
            }

            _context.Educations.Remove(education);
            _context.SaveChanges();
            return Ok("ลบข้อมูลสำเร็จ.");
        }
    }
}

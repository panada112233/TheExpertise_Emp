using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TheExpertise_Emp.Data;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkExperiencesController : ControllerBase
    {
        private readonly EmployeeDocumentDBSContext _context;

        public WorkExperiencesController(EmployeeDocumentDBSContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var experiences = _context.WorkExperiences.ToList();
            return Ok(experiences);
        }

        [HttpGet("Getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var experiences = _context.WorkExperiences.Where(ex => ex.UserID == id).ToList().OrderByDescending(ex => ex.StartDate);
            return Ok(experiences);
        }

        // ดึงข้อมูลตาม UserID
        [HttpPost("Insert")]
        public IActionResult WorkExperiences(WorkExperiences exp)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserID == exp.UserID);
            if (existingUser is object)
            {
                var data = new WorkExperiences
                {
                    UserID = exp.UserID,
                    JobTitle = exp.JobTitle,
                    CompanyName = exp.CompanyName,
                    StartDate = exp.StartDate,
                    EndDate = exp.EndDate,
                    Description = exp.Description,
                    Salary = exp.Salary,
                };
                _context.WorkExperiences.Add(data);
                _context.SaveChanges();

                var userDetails = GetById(exp.UserID);
                //return CreatedAtAction(nameof(GetById), new { id = exp.UserID }, userDetails);
                return Created("Insert success.", userDetails);
            }
            return BadRequest("ไม่พบข้อมูลของ usersid.");
        }
        // แก้ไขข้อมูลประสบการณ์การทำงาน
        [HttpPut("Update/{id}")]
        public IActionResult UpdateExperience(int id, [FromBody] WorkExperiences updatedExperience)
        {
            var existingExperience = _context.WorkExperiences.FirstOrDefault(ex => ex.ExperienceID == id);
            if (existingExperience == null)
            {
                return NotFound("ไม่พบข้อมูลประสบการณ์การทำงาน.");
            }

            existingExperience.JobTitle = updatedExperience.JobTitle;
            existingExperience.CompanyName = updatedExperience.CompanyName;
            existingExperience.StartDate = updatedExperience.StartDate;
            existingExperience.EndDate = updatedExperience.EndDate;
            existingExperience.Description = updatedExperience.Description;
            existingExperience.Salary = updatedExperience.Salary;

            _context.SaveChanges();
            return Ok("แก้ไขข้อมูลสำเร็จ.");
        }


        // ลบข้อมูลประสบการณ์การทำงาน
        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteExperience(int id)
        {
            var experience = _context.WorkExperiences.FirstOrDefault(ex => ex.ExperienceID == id);
            if (experience is null)
            {
                return NotFound("ไม่พบข้อมูลประสบการณ์การทำงาน.");
            }

            _context.WorkExperiences.Remove(experience);
            _context.SaveChanges();
            return Ok("ลบข้อมูลสำเร็จ.");
        }


    }
}

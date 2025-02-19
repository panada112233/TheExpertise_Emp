using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TheExpertise_Emp.Models;

namespace TheExpertise_Emp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailServiceController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailServiceController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.Email))
                return BadRequest("กรุณาระบุอีเมล");

            await _emailService.SendEmailAsync(
                emailRequest.Email,
                "คุณได้รับรหัสผ่านยืนยันหรือยัง",
                "<p>เมื่อคุณได้รับรหัสผ่านยืนยันแล้ว คุณควรเปลี่ยนรหัสผ่านใหม่ทันที!!!</p>"
            );

            return Ok("อีเมลถูกส่งแล้ว");
        }
    }

    public class EmailRequest
    {
        public string Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TheExpertise_Emp.Models
{
    public class EmailRequest
    {
        [Required(ErrorMessage = "กรุณาระบุอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        public string Email { get; set; }
    }
}

namespace TheExpertise_Emp.Models
{
      //userId: form.userId || 0,  // ✅ ป้องกัน null
      //              fullname: form.fullname || "ไม่ระบุชื่อ",  // ✅ ป้องกัน null
      //              documentId: form.documentId || "",
      //              leaveTypeId: form.leaveTypeId || ""
    public class ApprovedRequest
    {
        public int? userId { get; set; }
        public string? fullname { get; set; }
        public string? documentId { get; set; }
        public string? leaveTypeId { get; set; }

    }
}

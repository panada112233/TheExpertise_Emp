namespace TheExpertise_Emp.Models
{
    public class ApproveRequest
    {
        public Guid? DocumentID { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerComment { get; set; }
        public string? HRSignature { get; set; }
    }
}

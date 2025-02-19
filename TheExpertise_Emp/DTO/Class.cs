namespace TheExpertise_Emp.DTO
{
    public class FileCreatDto
    {
        public IFormFile File { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
    }
    public class DTORespone 
    {
        public string message { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public string role { get; set; }
    }
}

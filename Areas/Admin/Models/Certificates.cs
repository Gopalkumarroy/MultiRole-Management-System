namespace Sewa360.Areas.Admin.Models
{
    public class Certificates
    {
        public int Student_Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RollNo { get; set; }

        // New fields
        public string Course { get; set; }
        public string CompanyName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

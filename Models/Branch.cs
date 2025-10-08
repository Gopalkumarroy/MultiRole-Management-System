using Microsoft.Identity.Client;

namespace Sewa360.Models
{
    public class Branch
    {

        public int Branch_Id {  get; set; } 
        public string? Branch_Name { get; set; }  
        public string? Branch_Description { get; set; } 
        public String? Address { get; set; }
        public String? Branch_Phone { get; set; }
        public String? Branch_Email {  get; set; }  
        public String? JoiningDate { get; set; }       
        public String? Salary { get; set; }
        public String? Remarks { get; set; }
        public String? Department { get; set; }
        public String? Branch_Code { get; set; }
        public String? Lattitude { get; set; }
        public String? Longitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Company_Id { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Sewa360.Models
{
    public class SericeProvider
    {
        public int ServiceProvider_Id { get; set; }

        [Required]
        public string ServiceProv_Name { get;   set; }
        public int Branch_Id { get; set; } 
        public int Service_Id { get; set; }
        [Required]
        public String ServiceProv_Email { get; set; }
        public String Service_Phone { get; set; }
        public String ServiceProv_Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public String Refferal_Code { get; set; }

        [Required]
        public String First_Name { get; set; }
        [Required]
        public String Last_Name { get; set; }
        [Required]
        public String Company_Name { get; set; }
        [Required]
        public String ServiceProv_Password { get; set; }
        [Required]
        public string Manager_Code { get; set; } // ✅ New
        public int Manager_Id { get; set; } // ✅ New
        public string Action { get; set; }// ✅ New

    }
}

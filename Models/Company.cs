using System.ComponentModel.DataAnnotations;

namespace Sewa360.Models
{
    public class Company
    {
        [Key]
        public int Company_Id { get; set; }
        public string? Company_Code { get; set; }
        public string? Company_Name { get; set; }
        public string? Company_Address { get; set; }
        public string? Company_State { get; set; }
        public string? Company_City { get; set; }
        public string? Company_ZipCode { get; set; }
        public string? Company_Country { get; set; }
        public string? Company_Phone { get; set; }
        public string? Company_Email { get; set; }
        public string? Company_Website { get; set; }
        public DateTime? Company_RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? Company_IndustryType { get; set; }
        public string? Company_TaxId { get; set; }
        public string? Company_Description { get; set; }
    }
}

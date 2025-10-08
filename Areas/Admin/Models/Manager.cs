namespace Sewa360.Areas.Admin.Models
{
    public class Manager
    {
        public int Manager_Id { get; set; }

        //public int Branch_Id { get; set; }

        public string? Manager_Name { get; set; }
        public String? Manager_Password { get; set; }
        public string? Manager_Description { get; set; }
        public string? Manager_Phone { get; set; }
        public String? Manager_Email { get; set; }
        public string? Manager_City { get; set; }
        public string? Manager_Region { get; set; }
        public string? Manager_PostalCode { get; set; }
        public string? Manager_Country { get; set; }
        public String? Manager_Address { get; set; }
        public String? Manager_Department { get; set; }
        public String? Manager_Code { get; set; }
        public String? Manager_gender { get; set; }
        public String? Manager_Type { get; set; }
        public String? Manager_Department_Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Action { get; set; }


    }
}

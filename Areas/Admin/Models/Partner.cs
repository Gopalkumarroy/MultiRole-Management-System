using System.ComponentModel.DataAnnotations;

namespace Sewa360.Areas.Admin.Models
{
    public class Partner
    {
        public int Partner_Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression(@"^[A-Za-z\s]{2,30}$", ErrorMessage = "Enter a valid first name (letters only)")]
        public String First_Name { get; set; }


        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression(@"^[A-Za-z\s]{2,30}$", ErrorMessage = "Enter a valid last name (letters only)")]
        public String Last_Name { get; set; }
        public string Partner_Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
           ErrorMessage = "Password must be at least 6 characters and include upper, lower case and number")]
        public String Partner_Password { get; set; }

        public string Partner_Description { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number")]
        public string Partner_Phone { get; set; }
        public string Partner_City { get; set; }    
        public string Partner_Country { get; set; }
        public string Partner_State { get; set; }
        public string Partner_ZipCode { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
   ErrorMessage = "Enter a valid email address")]
        public string Partner_Email { get; set; }    
        public string Partner_Address { get; set; }
        public string Partner_BankDetails { get; set; }
        public string Partner_Type { get; set; }
        public String Partner_Contract_StartDate { get; set; }
        public string Partner_Contract_EndDate { get; set; }

        [RegularExpression(@"^[A-Z0-9]{5,15}$", ErrorMessage = "Referral code must be alphanumeric and 5-15 characters")]
        public string Partner_Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Action { get; set; }
        public String Partner_IDproof { get; set; }
        public string Manager_Code { get; set; } // ✅ New
        public int Manager_Id { get; set; } // ✅ New

        public string FullName => $"{First_Name} {Last_Name}";

        [Required(ErrorMessage = "Business is required")]
        public string Business { get; set; }




    }
}

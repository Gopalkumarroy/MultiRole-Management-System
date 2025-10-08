using System.ComponentModel.DataAnnotations;

namespace Sewa360.Models
{
    public class ServiceProviderKYC
    {
        public int KYC_Id { get; set; }

        public int ServiceProvider_Id { get; set; }

        public string AadhaarDocumentPath { get; set; }

        public string PanCardDocumentPath { get; set; }

        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar number must be exactly 12 digits.")]
        public string AadhaarNumber { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "PAN number must be in format ABCDE1234F.")]
        public string PanNumber { get; set; }

        [Required]
        public string AccountHolderName { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Account number must contain only digits.")]
        public string AccountNumber { get; set; }

        [Required]
        public string IFSC { get; set; }

        public string CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string IsActive { get; set; }

        public string IsDeleted { get; set; }

        public string KycStatus { get; set; } // "Pending", "Approved", "Rejected"
    }
}

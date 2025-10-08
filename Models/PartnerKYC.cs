using System.ComponentModel.DataAnnotations;

namespace Sewa360.Models
{
    public class PartnerKYC
    {
        public int KYC_Id { get; set; }
        public int Partner_Id { get; set; }
        public String AadhaarDocumentPath { get; set; }
        public String PanCardDocumentPath { get; set; }
        public String AccountHolderName { get; set; }
        public String BankName { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Account number must contain only digits.")]
        public string AccountNumber { get; set; }
        public String IFSC { get; set; }
        public String CreatedOn { get; set; }
        public String IsActive { get; set; }
        public String IsDeleted { get; set; }
        public String CreatedBy { get; set; }
        public String ModifiedBy { get; set; }
        public String ModifiedOn { get; set; }

        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhaar number must be exactly 12 digits.")]
        public String AadhaarNumber { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "PAN number must be in format ABCDE1234F.")]
        public String PanNumber { get; set; }

        public string KycStatus { get; set; } // "Pending", "Approved", or "Rejected"
    }
}

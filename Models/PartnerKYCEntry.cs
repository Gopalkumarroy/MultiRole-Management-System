using System.ComponentModel.DataAnnotations;
namespace Sewa360.Models
{
    public class PartnerKYCEntry
    {
        public int Partner_Id { get; set; }
        public string PartnerName { get; set; }
        public string KycStatus { get; set; }

        public String Partner_Phone { get; set; }
    }
}

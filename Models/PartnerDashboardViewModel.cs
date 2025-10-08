namespace Sewa360.Models
{
    public class PartnerDashboardViewModel
    {
        public int JobPostCount { get; set; }
        public decimal SalesAmount { get; set; }
        public int NumberOfServices { get; set; }
        public decimal CommissionAmount { get; set; }
        public int ListingCount { get; set; }

        public List<decimal> MonthlySales { get; set; }  // e.g., for 12 months
        public List<string> Months { get; set; } // e.g., Jan, Feb, Mar, etc.
    }
}

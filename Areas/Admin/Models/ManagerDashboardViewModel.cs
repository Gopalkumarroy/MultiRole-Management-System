namespace Sewa360.Areas.Admin.Models
{
    public class ManagerDashboardViewModel
    {
        public int TotalPartners { get; set; }
        public int KYCSubmitted { get; set; }
        public int KYCPending { get; set; }
        public int KYCRejected { get; set; }
        public int KYCNotDone { get; set; }
        public int TotalServiceProviders { get; set; }


        public decimal TotalSales { get; set; }
        public List<decimal> MonthlySales { get; set; } // Jan to Dec or partial
    }

}

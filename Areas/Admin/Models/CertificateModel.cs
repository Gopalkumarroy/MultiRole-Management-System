namespace Sewa360.Areas.Admin.Models
{
    public class CertificateModel
    {
        public int Certificate_Id { get; set; }
        public string CertificateNo { get; set; }
        public string PdfPath { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string StudentName { get; set; }
        public string RollNo { get; set; }
        public string Course { get; set; }
    }
}

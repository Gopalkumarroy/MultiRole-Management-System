using System.ComponentModel.DataAnnotations;

namespace Sewa360.Areas.Admin.Models
{
    public class AdminLogin
    {
        public int Admin_Id { get; set; }

        [Required]
        public string Admin_Name { get; set; }

        [Required]
        public string Admin_Password { get; set; }
    }
}

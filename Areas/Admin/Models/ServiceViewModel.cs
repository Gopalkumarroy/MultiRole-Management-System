namespace Sewa360.Areas.Admin.Models
{
    public class ServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubServiceViewModel> SubServices { get; set; }
    }

    public class SubServiceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BookViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubBookViewModel> SubBooks { get; set; }
    }
    public class SubBookViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


}

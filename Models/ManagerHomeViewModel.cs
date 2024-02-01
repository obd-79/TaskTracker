namespace Proje4.Models
{
    public class ManagerHomeViewModel
    {
        public Project CurrentProject { get; set; }
        public IEnumerable<User> Employees { get; set; }
    }

}

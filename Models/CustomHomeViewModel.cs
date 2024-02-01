namespace Proje4.Models
{
    public class CustomHomeViewModel
    {
        public IEnumerable<Proje4.Models.Project>? Projects { get; set; }
        public IEnumerable<Proje4.Models.User>? Users { get; set; }
        public IEnumerable<Proje4.Models.Task>? Tasks { get; set; }
        public IEnumerable<Proje4.Models.Message>? Messages { get; set; }
        public string? searchQuery { get; set; }
    }
}

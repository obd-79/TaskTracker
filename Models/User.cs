namespace Proje4.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }
        public int TotalNumberOfTasks { get; set; }
        public int TotalDoneTasks { get; set; }


    }
}

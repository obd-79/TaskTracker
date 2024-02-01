using System.ComponentModel.DataAnnotations;

namespace Proje4.Models
{
    public class TaskDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public bool Completed { get; set; }
        public string AssignedToUserName { get; set; }

        public string Status => Completed ? "Completed" : "In Progress"; // This will compute the status as a string
    }

}

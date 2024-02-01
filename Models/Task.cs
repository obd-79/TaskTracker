using Proje4.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proje4.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        [Display(Name = "Assigned to: ")]
        public int? UserId { get; set; }
        public User? User { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public bool Completed { get; set; }
    }
}

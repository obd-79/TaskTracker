using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace Proje4.Models
{
    public class CreateTaskViewModel
    {
        [Required(ErrorMessage = "Task name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Assignment to an employee is required.")]
        public int? AssignedToUserId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Employees { get; set; }
    }
}

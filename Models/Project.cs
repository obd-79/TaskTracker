using Proje4.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Proje4.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [Display(Name = "Manager")]
        public int? UserId { get; set; }

        public User? User { get; set; }

        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        public ICollection<Task>? Tasks { get; set; }
        

    }

}

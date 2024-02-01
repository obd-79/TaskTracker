using System.ComponentModel.DataAnnotations;

namespace Proje4.Models
{
    public class Message
    {
        
        public int Id { get; set; }
        public string Text { get; set; }
        [Display(Name ="From: ")]
        public int? UserId { get; set; }
        public User? User { get; set; }

    }
}

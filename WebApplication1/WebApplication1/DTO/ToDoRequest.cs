using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class ToDoRequest
    {
        [Required]
        public string title { get; set; }

        [Required]
        public string description { get; set; }

        public bool done { get; set; } = false;
    }
}

using System.ComponentModel.DataAnnotations;

namespace _4th_Task.ViewModels
{
    public class RegisterModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [Required]
        public string Name { get; set; }
    }
}

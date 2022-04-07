using System.ComponentModel.DataAnnotations;

namespace _4th_Task.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

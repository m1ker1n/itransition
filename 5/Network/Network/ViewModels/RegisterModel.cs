using System.ComponentModel.DataAnnotations;

namespace Network.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Minimum length is 3, Maximum length is 50.")]
        [RegularExpression(@"[A-Za-z0-9]+",ErrorMessage = @"Valid symbols for name are A-Z, a-z, 0-9.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must be the same.")]
        public string ConfirmPassword { get; set; }
    }
}

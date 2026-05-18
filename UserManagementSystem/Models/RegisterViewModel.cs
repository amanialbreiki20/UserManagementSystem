using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = " Required !")]
        public string FullName { get; set; }

        [Required(ErrorMessage = " Required !")]
        [EmailAddress(ErrorMessage = "Invalid Email Address !")]
        public string Email { get; set; }

        [Required(ErrorMessage = " Required !")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long !")]
        public string Password { get; set; }

        [Required(ErrorMessage = " Required !")]
        [Compare("Password", ErrorMessage = "Passwords do not match !")]
        public string ConfirmPassword { get; set; }
    }
}

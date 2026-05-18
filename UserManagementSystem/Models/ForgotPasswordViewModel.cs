using System.ComponentModel.DataAnnotations;

namespace UserManagementSystem.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage ="Emaail is Required")]
        [EmailAddress(ErrorMessage ="Invalid Email!")]
        public string Email { get; set; }
    }
}

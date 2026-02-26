using System.ComponentModel.DataAnnotations;
using Identity.API.Models;

namespace Identity.API.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100,
            ErrorMessage = "Password must be at least {2} and at most {1} characters.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",
            ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        // Optional – nếu UI cần bind user object
        public ApplicationUser? User { get; set; }
    }
}

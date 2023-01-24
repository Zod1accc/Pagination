using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Pustok.ViewModels.Member
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(maximumLength: 50)]
        public string Fullname { get; set; }
        [Required]
        [StringLength(maximumLength: 30)]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 8), DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 8), DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}

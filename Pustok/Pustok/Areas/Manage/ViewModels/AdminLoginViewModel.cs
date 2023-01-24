using System.ComponentModel.DataAnnotations;

namespace Pustok.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [StringLength(maximumLength:30)]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength:30)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

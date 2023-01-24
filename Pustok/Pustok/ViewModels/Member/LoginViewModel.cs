using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace Pustok.ViewModels.Member
{
    public class LoginViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        [StringLength(maximumLength: 30)]
        public string Username { get; set; }
        [System.ComponentModel.DataAnnotations.Required]

        [StringLength(maximumLength: 30, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

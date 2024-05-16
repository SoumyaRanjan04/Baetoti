using System.ComponentModel.DataAnnotations;

namespace Baetoti.Shared.Request.Auth
{
    public class ChangePasswordRequest
    {

        [Required(ErrorMessage = "Please enter your current password.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter new password.")]
        public string NewPassword { get; set; }

    }
}

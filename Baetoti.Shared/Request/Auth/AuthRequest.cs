using System.ComponentModel.DataAnnotations;

namespace Baetoti.Shared.Request.Auth
{
    public class AuthRequest
    {

        [Required(ErrorMessage = "Please enter Username.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter Password.")]
        public string Password { get; set; }

    }
}

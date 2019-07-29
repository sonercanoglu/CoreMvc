using System.ComponentModel.DataAnnotations;

namespace CoreMvc.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please Enter Username."),StringLength(50)]
        public string Username{ get; set; }
        [Required(ErrorMessage = "Please Enter Password."), StringLength(50)]
        public string Password { get; set; }
    }
}

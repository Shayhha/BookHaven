using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookHeaven.Models
{
    public class Login
    {
        [Required(ErrorMessage = "email and password are required")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "email or password is incorrect.")]
        public string email { get; set; }

        [Required(ErrorMessage = "email and password are required")]
        [RegularExpression("^(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d!@#$%^&*()-_=+{}\\[\\]|;:'\",.<>?]{6,12}$", ErrorMessage = "email or password is incorrect.")]
        public string password { get; set; }

        public Login() { }

        public Login(string _email, string _password)
        {
            email = _email;
            password = _password;
        }
    }
}

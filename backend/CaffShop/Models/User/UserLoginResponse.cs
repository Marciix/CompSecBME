namespace CaffShop.Models
{
    public class UserLoginResponse
    {
        public string JwtToken { get; set; }
        public string Role { get; set; }
    }
}
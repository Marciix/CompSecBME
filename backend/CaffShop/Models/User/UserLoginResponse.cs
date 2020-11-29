namespace CaffShop.Models.User
{
    public class UserLoginResponse
    {
        public string JwtToken { get; set; }
        public string Role { get; set; }
    }
}
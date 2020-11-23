using System;

namespace CaffShop.Models.Exceptions
{
    public class LoginFailedException : Exception
    {
        public LoginFailedException(string message) : base(message)
        {
        }
    }
}
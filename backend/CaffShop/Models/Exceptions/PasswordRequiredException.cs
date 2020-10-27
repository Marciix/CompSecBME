using System;

namespace CaffShop.Models.Exceptions
{
    public class PasswordRequiredException : Exception
    {
        public PasswordRequiredException(string message) : base(message)
        {
        }
    }
}
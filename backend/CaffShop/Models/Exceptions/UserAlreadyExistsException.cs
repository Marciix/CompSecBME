using System;

namespace CaffShop.Models.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message): base(message)
        {
        }
    }
}
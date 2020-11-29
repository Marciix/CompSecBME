using System;

namespace CaffShop.Models.Exceptions
{
    public class InvalidUserDataException : Exception
    {
        public InvalidUserDataException(string message) : base(message)
        {
        }
    }
}
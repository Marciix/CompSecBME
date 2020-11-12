using System;

namespace CaffShop.Models.Exceptions
{
    public class InvalidCaffFileException : Exception
    {
        public InvalidCaffFileException(string message) : base(message)
        {
        }
    }
}
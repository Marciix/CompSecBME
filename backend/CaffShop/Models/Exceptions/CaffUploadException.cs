using System;

namespace CaffShop.Models.Exceptions
{
    public class CaffUploadException : Exception
    {
        public CaffUploadException(string message) : base(message)
        {
        }
    }
}
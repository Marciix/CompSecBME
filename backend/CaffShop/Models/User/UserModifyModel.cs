using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Models.User
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class UserModifyModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
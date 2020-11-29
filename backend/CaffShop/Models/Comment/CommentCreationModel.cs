using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CaffShop.Models.Comment
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CommentCreationModel
    {
        [Required]
        public string Content { get; set; }
    }
}
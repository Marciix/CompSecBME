using AutoMapper;
using CaffShop.Entities;
using CaffShop.Models;

namespace CaffShop.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CommentCreationModel, Comment>();
        }
    }
}
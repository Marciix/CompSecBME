using AutoMapper;
using CaffShop.Entities;
using CaffShop.Models;
using CaffShop.Models.CaffItem;

namespace CaffShop.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CommentCreationModel, Comment>();
            CreateMap<Comment, CommentPublic>();
            CreateMap<User, UserPublic>();
            CreateMap<CaffItem, CaffItemPublic>();
            CreateMap<UserRegistrationModel, User>();
        }
    }
}
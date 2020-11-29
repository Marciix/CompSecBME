using AutoMapper;
using CaffShop.Entities;
using CaffShop.Models.CaffItem;
using CaffShop.Models.Comment;
using CaffShop.Models.User;

namespace CaffShop.Helpers
{
    // ReSharper disable once UnusedType.Global
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
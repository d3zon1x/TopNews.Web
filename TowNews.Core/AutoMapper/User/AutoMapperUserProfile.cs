﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopNews.Core.DTOS.User;
using TopNews.Core.Entities.User;

namespace TopNews.Core.AutoMapper.User
{
    public class AutoMapperUserProfile : Profile
    {
        public AutoMapperUserProfile()
        {
            CreateMap<UsersDTO, AppUser>().ReverseMap();
            CreateMap<UpdateUserDTO, AppUser>().ReverseMap();
            CreateMap<AppUser, EditUserDTO>().ReverseMap();
            CreateMap<CreateUserDTO, AppUser>().ForMember(dst => dst.UserName, act => act.MapFrom(src => src.Email));
        }
    }
}

using AuthenticationSystem.Identity;
using AuthenticationSystem.Models.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.DTOMapping
{
    public class MappingPofile:Profile
    {
        public MappingPofile()
        {
            CreateMap<UserRegisterDTO,ApplicationUser>().ReverseMap();
        }
    }
}

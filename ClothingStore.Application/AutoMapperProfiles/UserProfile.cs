using AutoMapper;
using ClothingStore.Domain.Entities;
using ClothingStore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.AutoMapperProfiles
{
        public class UserProfile : Profile
        {
            public UserProfile()
            {
                CreateMap<User, UserDTO>();
                CreateMap<AddUserDTO, Client>()
                .ForMember(dest => dest.Role, opt => opt.Ignore()) // Ignorar la propiedad Role que si esta en la entidad client (se ignora porque el rol es asignado luego del mapeo)
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorar la propiedad Id que si esta en la entidad client

                CreateMap<AddUserDTO, Seller>()
                    .ForMember(dest => dest.Role, opt => opt.Ignore()) // Ignorar la propiedad Role que si esta en la entidad seller
                    .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorar la propiedad Id que si esta en la entidad seller

                CreateMap<User, UserWithProductsDTO>()
                    .ForMember(dest => dest.UserProducts, opt => opt.Ignore()); // Ignorar Products en User
                
                
            }

        }
 }
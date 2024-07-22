using AutoMapper;
using ClothingStore.Application.Dtos;
using ClothingStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.AutoMapperProfiles
{
    public class SellerProfile: Profile
    {
        public SellerProfile()
        {
            // Mapeo de Seller a SellerDto 
            CreateMap<Seller, SellerDTO>();

            CreateMap<Seller, UserWithProductsDTO>()
                     .IncludeBase<User, UserWithProductsDTO>()
                      .ForMember(dest => dest.UserProducts, opt => opt.MapFrom(src => src.PublishedProducts));
        }

    }
}

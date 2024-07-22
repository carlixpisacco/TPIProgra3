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
    public class ProductProfile : Profile
    {
        public  ProductProfile()
        {
            // Mapeo de Product a ProductWithSellerDTO
            CreateMap<Product, ProductWithSellerDTO>()
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.Seller));

            // Mapeo de Product a ProductWithClientDTO
            CreateMap<Product, ProductWithClientDTO>()
           .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Client == null ? "Este producto aún no fue vendido" : "El producto fue comprado por el vendedor especificado abajo"));

            // Mapeo de AddProductDTO a Product
            CreateMap<AddProductDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignorar el ID si es auto-generado

            // Mapeo de Product a ProductDTO
            CreateMap<Product, ProductDTO>();
        }
    }
}

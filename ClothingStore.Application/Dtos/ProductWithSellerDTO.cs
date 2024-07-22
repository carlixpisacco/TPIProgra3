using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Dtos
{
    public class ProductWithSellerDTO
    {
         public string Description { get; set; }
         public decimal Price { get; set; }
         public SellerDTO Seller { get; set; } // DTO para el vendedor
 
    }
}

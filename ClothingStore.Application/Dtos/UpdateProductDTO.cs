using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Dtos
{
    public class UpdateProductDTO
    {
        public int Id { get; set; }  // ID del producto
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }

    public enum UpdateProductResult
    {
        Success,
        NotFound,
        NotFound2,
        Unauthorized,
    }


}

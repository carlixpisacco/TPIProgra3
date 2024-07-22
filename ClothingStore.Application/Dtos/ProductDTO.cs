using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Dtos
{
    public class ProductDTO
    {
        public int Id { get; set; } // Clave primaria
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}

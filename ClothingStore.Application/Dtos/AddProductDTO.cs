using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Dtos
{
    public class AddProductDTO
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
        
    }
}

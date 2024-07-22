using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Domain.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Clave primaria

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }

        // Clave foránea a Vendedor
        public int SellerId { get; set; }
        public Seller Seller { get; set; }

        // Clave foránea a Client (Puede ser null si el producto no está dado de baja)
        public int? ClientId { get; set; }
        public Client Client { get; set; }

        public Product() { }
    }
}

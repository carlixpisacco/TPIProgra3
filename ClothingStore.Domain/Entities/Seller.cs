using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Domain.Entities
{
    public class Seller : User
    {
        public ICollection<Product> PublishedProducts { get; set; }

        public Seller() { }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Domain.Entities
{
    public class Client : User
    {
        public ICollection<Product> PurchasedProducts { get; set; }

        public Client() { }
    }
}

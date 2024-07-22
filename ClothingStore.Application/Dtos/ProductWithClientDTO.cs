﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClothingStore.Application.Dtos
{
    public class ProductWithClientDTO
    {
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public ClientDTO Client { get; set; }
    }
}

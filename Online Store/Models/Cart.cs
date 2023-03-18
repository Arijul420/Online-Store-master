﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_Store.Models
{
    public class Cart
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public int ProductTotal { get; set; }

    }
}
﻿using Newtonsoft.Json;
using System;
namespace HoaHoeHoaSoi.Model
{
    public class Products
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Img { get; set; }
        public int Quantity { get; internal set; }
        public string Description { get; set; }

        public Products()
        {

        }
    }

    public class ProductCreateModel
    {
        
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
		public string Name { get; set; }

		public double Price { get; set; }

		public string Img { get; set; }

        public string Description { get; set; }
    }
}
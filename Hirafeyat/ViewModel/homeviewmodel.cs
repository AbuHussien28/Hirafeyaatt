﻿using Hirafeyat.Models;

namespace Hirafeyat.ViewModel
{
    public class homeviewmodel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public Dictionary<string, List<Product>> ProductDictionary { get; set; } = new(); 



    }
}

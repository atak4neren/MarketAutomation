using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025.Products
{
    public class Product
    {
        public int Id { get; set; }         //Ürünün benzersiz kimliği
        public string Name { get; set; }    //Ürünün adı
        public decimal Price { get; set; }  //Ürünün fiyatı
        public int Stock { get; set; }      //Ürünün stokta bulunan sayısı
    }
}

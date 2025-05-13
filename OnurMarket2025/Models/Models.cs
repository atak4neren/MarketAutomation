using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnurMarket2025.Users;
using OnurMarket2025.Products;
using OnurMarket2025.Orders;


namespace OnurMarket2025.Models
{
    public class ProductModel
    {
        public List<Product> Products { get; set; } = new List<Product>();

    }
    public class UserModel
    {
        public List<User> Users { get; set; } = new List<User>();

    }
    public class OrderModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();

    }
}

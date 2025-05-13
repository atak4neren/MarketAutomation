using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025.Orders
{
    public class Order
    {
        public int Id { get; set; }                 //Sipariş kimliği
        public int CustomerId { get; set; }         //Sipariş veren müşteriye ait kimlik
        public List<int> ProductIds { get; set; }   //Ürünlerin Id'lerini listeler
        public bool IsApproved { get; set; }        //Onay durumu (Admin tarafından onaylandı mı?
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        //OrderDetail sınıfında tuttuğumuz verileri listelememize ve farklı ürünleri tutmamızı sağlar.
    }
}

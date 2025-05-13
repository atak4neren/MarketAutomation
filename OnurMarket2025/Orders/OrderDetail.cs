using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025.Orders
{
    public class OrderDetail
    {
        public int OrderId { get; set; }        //Siparişin kimliği
        public int ProductId { get; set; }      //Hangi ürün sipariş edildi?
        public int Quantity { get; set; }       //Kaç adet ürün alındı?
    }
}

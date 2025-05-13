using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025.Users
{
    public class Customer : User
    {
        public int Id { get; set; }             //Kullanıcı Id'si
        public string FullName { get; set; }    //Kullanıcı nesnesi
    }
}

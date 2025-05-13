using OnurMarket2025.Orders;
using OnurMarket2025.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025
{
    public class LoginService
    {
        public static User CurrentUser { get; private set; } // herkes buradan erişebilecek

        public User Login(List<User> users)
        {
            Console.Write("Kullanıcı Adı: ");
            string username = Console.ReadLine();

            Console.Write("Şifre: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                CurrentUser = user; // doğru giriş yaparsa CurrentUser'ı set ediyoruz
            }

            return user;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnurMarket2025.Users
{
    public class User
    {
        public int Id { get; set; }             //Kullanıcıya ait benzersiz kimlik
        public string Username { get; set; }    //Kullanıcının Adı
        public string Password { get; set; }    //Kullanıcının şifresi
        public bool IsAdmin {  get; set; }      //Kullanıcının yetki sorgusu (Admin mi/müşteri mi?)
        public bool IsBlocked { get; set; }     //Kullanıcı bloke durumu
    }
}

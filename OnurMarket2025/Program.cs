using Newtonsoft.Json;          //Json verilerini serialize/deserialize etmek için.
using OnurMarket2025.Models;    //Models sınıfının tutulduğu dosya.
using OnurMarket2025.Orders;    //Orders sınıfının tutulduğu dosya.
using OnurMarket2025.Products;  //Products sınıfının tutulduğu dosya.
using OnurMarket2025.Users;     //Users sınıfının tutulduğu dosya.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OnurMarket2025
{
    class Program
    {
        //Uygulama start edildiğinde JSON dosyalarından ürün, kullanıcı ve sipariş bilgileri yüklenir.
        private static ProductModel product = JsonHelper.LoadProduct();
        private static UserModel userModel = JsonHelper.LoadUser();
        private static OrderModel orderModel = JsonHelper.LoadOrder();
        static List<Order> pendingOrders = new List<Order>();               //Onay bekleyen siparişleri geçici olarak tutar.

        public static string input;
        public static string orderSummary;


        //Program Başlangıcı Kullanıcı Login Ekranı
        static void Main(string[] args)
        {
            List<User> Users = userModel.Users;
            int logincount = 0;
            int trycount = 3;

        login:
            while (true)
            {
                LoginService login = new LoginService();
                User loginuser = login.Login(Users);        //Kullanıcı giriş bilgilerini alır.
                Console.Clear();


                if (loginuser == null)
                {
                    //3 denemenin ardından kullanıcıyı bloke eder
                    logincount++;
                    trycount--;

                    Console.Clear();
                    Console.WriteLine($"Giriş Başarısız! Tekrar deneyin. Kalan Deneme: {trycount}");

                    if (trycount == 0)
                    {
                        Console.Write("Kullanıcı Adınızı Tekrar Girin: ");
                        string username = Console.ReadLine();
                        var user = Users.FirstOrDefault(u => u.Username == username && !u.IsAdmin);

                        if (user != null)
                        {
                            user.IsBlocked = true;
                            JsonHelper.SaveUser(new UserModel { Users = Users }); //Json'a kaydet
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bu Kullanıcı Bloke Edildi!");
                            Console.ResetColor();
                        }
                        logincount=0; trycount = 3;
                        goto login;
                    }
                    goto login;
                }

                if (loginuser.IsBlocked)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bu Kullanıcı Bloke Edilmiştir! Lütfen Admin ile İletişime Geçiniz.");
                    Console.ResetColor();
                    goto login;
                }

                if (loginuser.IsAdmin)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Admin Paneline Hoş Geldiniz");
                    Console.ResetColor();
                    AdminPanel();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Müşteri Paneline Hoş Geldiniz");
                    Console.ResetColor();
                    CustomerPanel();
                }
            }
        }

        //Admin paneli
        static void AdminPanel()
        {
            while (true)
            {
                //Adminin işlem yetkileri:
                Console.WriteLine("\n Admin Paneli");  // \n -> string interpolition
                Console.WriteLine("1. Kullanıcı Yönetimi");
                Console.WriteLine("2. Ürün Ekle");
                Console.WriteLine("3. Ürünleri Gör");
                Console.WriteLine("4. Siparişleri Gör");
                Console.WriteLine("5. Geri dön");
                Console.WriteLine("6. Çıkış");
                Console.Write("Seçim Yapınız:");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        UserControl();
                        break;
                    case "2":
                        AddProduct();
                        break;
                    case "3":
                        ShowProducts();
                        break;
                    case "4":
                        ViewOrders();
                        break;
                    case "5":
                        Console.Clear();
                        return;
                    case "6":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz Seçim");
                        break;

                }
            }
        }

        //Kullanıcı yönetimi
        static void UserControl()
        {
            //Json dosyasından verileri okur. Her kullanıcı için admin ya da müşteri bloke durumu gösterir.
            //Admin kullanıcısının bloke durumu değiştirilemez.
            string path = @"../../Users.json";
            string json = File.ReadAllText(path);
            UserModel users = JsonConvert.DeserializeObject<UserModel>(json);

            Console.Clear();
            Console.WriteLine("Kayıtlı Kullanıcılar: ");
            Console.WriteLine("__________________________________________________________________");
            if (users != null || users.Users.Count > 0)
            {
                foreach (User user in users.Users)
                {
                    string role = user.IsAdmin ? "Admin" : "Müşteri";
                    string blockStatus = user.IsAdmin ? "-" : (user.IsBlocked ? "Bloklu" : "Aktif");

                    Console.WriteLine($"{user.Id} | {user.Username} | {role} | Blok Durumu: {blockStatus}");
                }

                Console.Write("\nBlok durumu değiştirilecek kullanıcı Id'sini girin (iptal etmek için 'Enter'): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) return;

                if (int.TryParse(input, out int userId))
                {
                    var selectedUser = users.Users.FirstOrDefault(u => u.Id == userId);
                    if (selectedUser == null)
                    {
                        Console.WriteLine("Kullanıcı Bulunamadı!");
                    }
                    else if (selectedUser.IsAdmin)
                    {
                        Console.WriteLine("Admin Kullanıcıların blok durumu değiştirilemez!");
                    }
                    else
                    {
                        selectedUser.IsBlocked = !selectedUser.IsBlocked;
                        File.WriteAllText(path, JsonConvert.SerializeObject(users, Formatting.Indented));
                        Console.WriteLine($"Kullanıcının blok durumu güncellendi: {(selectedUser.IsBlocked ? "Bloklu" : "Aktif")}");
                    }
                }
                else
                {
                    Console.WriteLine("Geçersiz ID Girdiniz!");
                }
            }
            else
            {
                Console.WriteLine("Mevcut Kullanıcı Bulunamadı!");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
            Console.Clear();

        }

        //Ürün ekle
        static void AddProduct()
        {
            //Ürün bilgileri; isim, fiyat, stok alınıp json'a yazılır.
            //Id değeri otomatik olarak artar.
            //Ürün listesine eklenip kaydedilir.
            Console.Clear();
            Console.Write("Ürün Adı: ");
            string name = Console.ReadLine();

            Console.Write("Fiyatı: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Stok Adedi: ");
            int stock = Convert.ToInt32(Console.ReadLine());

            if (product.Products == null)
            {
                product.Products = new List<Product>();
            }

            int newId = product.Products.Any() ? product.Products.Max(p => p.Id) + 1 : 1;

            product.Products.Add(new Product
            {
                Id = newId,
                Name = name,
                Price = price,
                Stock = stock
            });

            JsonHelper.SaveProduct(product);

            Console.Clear();
            Console.WriteLine("Ürün Eklendi.");
        }

        //Sipariş görüntüle
        static void ViewOrders()
        {
            Console.WriteLine("\n Bekleyen Siparişler: ");

            if (pendingOrders.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("Henüz Sipariş Yok.");
                return;
            }


            //Users.json dosyasını oku
            string userjson = File.ReadAllText(@"../../Users.json");
            UserModel users = JsonConvert.DeserializeObject<UserModel>(userjson);
            ProductModel productModel = JsonHelper.LoadProduct();


            int index = 1;
            foreach (var order in pendingOrders)
            {
                //Kullanıcıyı Id'ye göre bul.
                var user = users.Users.FirstOrDefault(u => u.Id == order.CustomerId);
                string userName = user != null ? user.Username : "Kullanıcı Adı Bilinmiyor";

                Console.WriteLine($"\n Sipariş #{index++}");
                Console.WriteLine($"Kullanıcı: {userName}");
                Console.WriteLine("Ürünler:");

                foreach (var detail in order.OrderDetails)
                {
                    var urun = product.Products.FirstOrDefault(p => p.Id == detail.ProductId);
                    string urunAdi = urun != null ? urun.Name : "Ürün Bulunamadı";
                    Console.WriteLine($"  - {urunAdi} x {detail.Quantity}");
                }

                Console.WriteLine("______________________________");


                Console.Write("\n Siparişi Onaylamak İçin (True/False): ");
                string onay = Console.ReadLine();

                switch (onay.ToLower())
                {
                    case "true":
                        order.IsApproved = true;

                        foreach (var detail in order.OrderDetails)
                        {
                            var urun = productModel.Products.FirstOrDefault(p => p.Id == detail.ProductId);
                            if (urun != null)
                            {
                                if (urun.Stock >= detail.Quantity)
                                {
                                    urun.Stock -= detail.Quantity;
                                }
                                else
                                {
                                    Console.WriteLine($"{urun.Name} için yeterli stok yok! Sipariş onaylanamadı.");
                                    order.IsApproved = false;   //Sipariş iptali.
                                    break;
                                }
                            }
                        }
                        Console.Clear();
                        Console.WriteLine("Sipariş Onaylandı");
                        break;

                    case "false":
                        Console.Clear();
                        Console.WriteLine("Sipariş İptal Edildi");
                        order.IsApproved = false;
                        break;

                    default:
                        Console.WriteLine("Lütfen geçerli bir işlem yapınız!");
                        return;
                }
            }

            //Onaylanan siparişleri kalıcı olarak Order listesine aktar:
            foreach (var Approved in pendingOrders.Where(a => a.IsApproved))
            {
                orderModel.Orders.Add(Approved);
            }

            //Güncellemeleri kaydet
            JsonHelper.SaveOrder(orderModel);
            JsonHelper.SaveProduct(productModel);   //Stok güncellemesini kaydet

            pendingOrders.Clear(); //Listeyi temizle (Onaylandı veya iptal edildi)
        }

        //Müşteri paneli
        static void CustomerPanel()
        {
            while (true)
            {
                Console.WriteLine("\n Müşteri Paneli");
                Console.WriteLine("1. Ürünleri Gör");
                Console.WriteLine("2. Sipariş ver (max 3 ürün)");
                Console.WriteLine("3. Çıkış");
                Console.Write("Lütfen Bir İşlem Seçiniz: ");
                string choice = Console.ReadLine();


                switch (choice)
                {
                    case "1":
                        ShowProducts();
                        break;
                    case "2":
                        PlaceOrder();
                        break;
                    case "3":
                        Console.Clear();
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Geçersiz Seçim!");
                        break;
                }
            }
        }

        //Ürün Listeleme
        static void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("\n Ürün Listesi:");
            foreach (var product in product.Products)
            {
                Console.WriteLine($"{product.Id} - {product.Name} - {product.Stock}Adet | {product.Price}TL");
            }
        }

        //Sipariş verme
        static void PlaceOrder()
        {
        siparis:
            Console.Clear();
            ShowProducts();

            Console.Write("Sipariş vermek istediğiniz ürün ID'lerini boşlukla ayırarak girin (max 3 ürün): ");
            string input = Console.ReadLine();

            var ids = input.Split(' ')
                           .Select(s => int.TryParse(s, out int id) ? id : -1)
                           .Where(id => id > 0)
                           .ToList();

            if (ids.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("Lütfen en az 1 adet ürün seçiniz!");
                goto siparis;
            }

            if (ids.Count > 3)
            {
                Console.Clear();
                Console.WriteLine("En fazla 3 ürün sipariş edebilirsiniz!");
                goto siparis;
            }

            var selectedProducts = product.Products.Where(p => ids.Contains(p.Id)).ToList();

            if (selectedProducts.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("Seçilen ürünler bulunamadı.");
                return;
            }

            List<OrderDetail> orderDetailsList = new List<OrderDetail>();

            foreach (var productItem in selectedProducts)
            {
                Console.Write($"{productItem.Name} ürünü için adet girin: ");
                int quantity;
                while (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                {
                    Console.Write("Geçerli bir adet giriniz:");
                }

                if (productItem.Stock < quantity)
                {
                    Console.Clear();
                    Console.WriteLine($"{productItem.Name} için yeterli stok yok. Kalan stok: {productItem.Stock}");
                    return;
                }

                //Sipariş detayı oluştur:
                orderDetailsList.Add(new OrderDetail
                {
                    ProductId = productItem.Id,
                    Quantity = quantity
                });
            }

            // Sipariş ekleme
            int newId = orderModel.Orders.Any() ? orderModel.Orders.Max(x => x.Id) + 1 : 1;
            var user = LoginService.CurrentUser;

            var newOrder = new Order
            {
                Id = newId,
                CustomerId = user.Id, //Kullanıcı Id
                ProductIds = selectedProducts.Select(p => p.Id).ToList(),
                IsApproved = false,
                OrderDetails = orderDetailsList
            };

            //Siparişi listeye ekleme ve json'a kaydetme
            orderModel.Orders.Add(newOrder);    //Sipariş ekleme
            JsonHelper.SaveOrder(orderModel);   //Siparişi kaydet
            JsonHelper.SaveProduct(product);    //Stok güncellemesini kaydet
            pendingOrders.Add(newOrder);        //Admin paneli için ekle


            Console.Clear();
            Console.WriteLine("Siparişiniz alınmıştır. Admin onayı bekleniyor...");
        }

    }
}

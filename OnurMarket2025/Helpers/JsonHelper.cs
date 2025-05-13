using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using OnurMarket2025.Models;

public static class JsonHelper
{
    public static readonly string ProductPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Product.json");
    public static readonly string UserPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Users.json");
    public static readonly string OrderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../Orders.json");
    public static ProductModel LoadProduct()
    {
        if (!File.Exists(ProductPath))
            return new ProductModel();

        var json = File.ReadAllText(ProductPath);
        return JsonConvert.DeserializeObject<ProductModel>(json) ?? new ProductModel();
    }

    public static void SaveProduct(ProductModel data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(ProductPath, json);
    }
    public static UserModel LoadUser()
    {
        if (!File.Exists(UserPath))
            return new UserModel();

        var json = File.ReadAllText(UserPath);
        return JsonConvert.DeserializeObject<UserModel>(json) ?? new UserModel();
    }

    public static void SaveUser(UserModel data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(UserPath, json);
    }

    public static OrderModel LoadOrder()
    {
        if (!File.Exists(OrderPath))
            return new OrderModel();

        var json = File.ReadAllText(OrderPath);
        return JsonConvert.DeserializeObject<OrderModel>(json) ?? new OrderModel();
    }
    public static void SaveOrder(OrderModel data)
    {
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(OrderPath, json);
    }
}
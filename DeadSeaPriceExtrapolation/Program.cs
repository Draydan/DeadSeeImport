using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.Entity;


using DeadSeaCatalogueDAL;
using Logger;

namespace DeadSeaPriceExtrapolation
{
    /// <summary>
    /// этот проект предназначен для рассчета тех оптовых цен, которые не удалось вытянуть с сайта
    /// берется наибольшая из оптовых цен для товара, с которым совпала розничная цена
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // допустимое отклонение цены, которую мы считаем почти равной искомой
            const float priceSearchProximity = 0.1f;

            ProductContext db = new ProductContext();
            // берем все товары, у кот. опт. и розн. цены равны
            foreach (Product prodWoPrice in db.Products.Where(p => p.price == p.priceFull))
            {
                Logger.Logger.ErrorLog("not set price with full {1} for {0}", prodWoPrice.title, prodWoPrice.priceFull);
                // берем товар с такой же розн.ценой и макс оптовой
                string maxprice = db.Products.Where(p2 => p2.priceFull == prodWoPrice.priceFull
                //&& Product.numericBaks(p2.price) < Product.numericBaks(p2.priceFull)
                && p2.price != p2.priceFull).Max(x => x.price);

                Product prodWithPrice = db.Products.FirstOrDefault(p => p.price != p.priceFull
                && p.priceFull == prodWoPrice.priceFull
                //&& Product.numericBaks(p.price) < Product.numericBaks(p.priceFull)
                && p.price == maxprice);

                //float numPWPrice = float.Parse(prodWithPrice.priceFull.Replace("$", ""));

                if (prodWithPrice == null)
                    foreach (var p in db.Products.Where(pr => pr.priceFull != null))
                    {
                        if (p.price != p.priceFull
                    && Math.Abs(Product.numericBaks(prodWoPrice.priceFull) - Product.numericBaks(p.priceFull))
                    < Product.numericBaks(prodWoPrice.priceFull) * priceSearchProximity
                    && Product.numericBaks(p.price) < Product.numericBaks(p.priceFull)
                    && ((prodWithPrice == null) || (Product.numericBaks(p.price) > Product.numericBaks(prodWithPrice.price))))
                            prodWithPrice = p;
                    }
                if (prodWithPrice == null)
                        continue;
                prodWoPrice.priceIsFromSiteNotExtrapolated = false;
                prodWoPrice.price = prodWithPrice.price;
                Logger.Logger.SuccessLog("take price {0} of {1} from {2}", prodWithPrice.price, prodWithPrice.priceFull, prodWithPrice.title);
                // задаем
                // профит        
            }
            db.SaveChanges();

            Console.WriteLine("done");

            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data.Entity;

using VkNet;
using DeadSeaCatalogueDAL;
using Logger;
using System.IO;

namespace DeadSeaVKExport
{
    public class DeadSeaVKExportProgram
    {
        private VkApi vk;
        private long GroupID;

        static bool locker = false;
        static DateTime now = DateTime.Now;
        static int counter = 1;
        static int sleepTime = 500;
        //static List<Product> goodsList = new List<Product>();
        static ProductContext db = new ProductContext();


        static void Main(string[] args)
        {
            ProductVKExporter vke = new ProductVKExporter();

            using (var db = new ProductContext())
            {
                foreach (Product g in db.Products)
                {
                    while (locker)
                    {
                        Console.Write(".");
                        Thread.Sleep(sleepTime);
                    }
                    locker = true;
                    long prodID = 0;
                    int counter = 0;
                    while (prodID == 0 && counter < 20)
                        try
                        {
                            prodID = vke.ExportProduct(g.title, g.desc.Replace("&", " n "), g.price, g.imageFileName);

                            foreach (LinkProductWithCategory link in g.Links)
                                vke.AddProductToAlbum(g.title, prodID, link.category.title, g.imageFileName);
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Trace(ex.Message);
                            Logger.Logger.Trace("Неудачная попытка... ждемс {0}", counter++);
                            Thread.Sleep(sleepTime);
                            if (counter > 10)
                            {
                                Logger.Logger.ErrorLog("Внимание внимание, что то пошло не так!");
                                Logger.Logger.ErrorLog(ex.ToString());
                                //Console.ReadLine();
                            }
                        }
                    if(counter>=20)
                    {
                        File.Copy(vke.GetImageFilePath(g.imageFileName), vke.GetTooSmallImageFilePath(g.imageFileName));
                    }
                    locker = false;
                }
            }
            Console.WriteLine("Press smth");
            Console.ReadLine();
        }

    }
}

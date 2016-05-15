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
            string[] modes = new string[] {"Полный импорт", "Импорт переводов"};
            int chosenMode = -1;
            while (chosenMode >= modes.Length || chosenMode < 0)
            {
                Console.WriteLine("Выберите режим импорта из списка:");
                for (int mi = 0; mi < modes.Length; mi++)
                    Console.WriteLine("{0}:{1}", mi, modes[mi]);
                string schosenMode = Console.ReadLine();
                if (!int.TryParse(schosenMode, out chosenMode))
                    chosenMode = -1;
            }

            ProductVKExporter vke = new ProductVKExporter();
            int exportedCount = 0;
            using (var db = new ProductContext())
            {
                foreach (Product p in db.Products)
                {
                    switch (chosenMode)                        
                    {
                        case 0:
                            if (ExportProductToVK(p, vke, db))
                                exportedCount++;
                            break;
                        case 1:
                            if (db.Translations.Any(t => t.titleEng == p.title))
                                if (ExportProductToVK(p, vke, db))
                                    exportedCount++;
                            break;
                }
                }
            }
            Logger.Logger.SuccessLog("Экспортировано {0} товаров", exportedCount);
            
            Console.WriteLine("Press smth");
            Console.ReadLine();
        }

        public static bool ExportProductToVK(Product g, ProductVKExporter vke, ProductContext db)
        {
            bool result = false;
            const int maxTries = 20;
            while (locker)
            {
                Console.Write(".");
                Thread.Sleep(sleepTime);
            }
            locker = true;
            long prodID = 0;
            int counter = 0;
            while (prodID == 0 && counter < maxTries)
                try
                {
                    Translation translation = db.Translations.FirstOrDefault(t => t.titleEng == g.title);
                    if (translation != null)
                    {
                        prodID = vke.ExportProduct(translation.title, translation.desc.Replace("&", " n "), g.price, g.imageFileName);
                        vke.AddProductToAlbum(g.title, prodID, ProductVKExporter.mainAlbumTitle, g.imageFileName);
                    }
                    else
                        prodID = vke.ExportProduct(g.title, g.desc.Replace("&", " n "), g.price, g.imageFileName);

                    foreach (LinkProductWithCategory link in g.Links)
                        vke.AddProductToAlbum(g.title, prodID, link.category.title, g.imageFileName);

                    result = true;
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
            if (counter >= maxTries)
            {
                File.Copy(vke.GetImageFilePath(g.imageFileName), vke.GetTooSmallImageFilePath(g.imageFileName), true);
            }
            locker = false;
            return result;
        }

    }
}

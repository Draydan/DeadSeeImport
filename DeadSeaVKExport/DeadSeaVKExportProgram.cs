using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data.Entity;

using VkNet;
using DeadSeaCatalogueDAL;
using Logger;
using System.IO;
using FuzzyHelper;

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
            string[] modes = new string[] {"Полный импорт", "Импорт переводов", "Забрать переводы с ВК", "Удалить непереведенные дубли переводов в ВК"};
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

            if (new[] { 0, 1 }.Contains(chosenMode))
            {
                /*
                // Удаляем продукты вошедшие в ВК, но не вошедшие в БД
                using (var db = new ProductContext())
                {
                    Logger.Logger.ErrorLog("Продукты вошедшие в ВК, но не вошедшие в БД: {0}",
                        vke.ProductList.Select(p => p.Title).
                        Where(title => !db.Products.Any(pl => pl.title == title)).Count());
                    foreach (var diff in vke.ProductList.Select(p => p.Title).
                        Where(title => !db.Products.Any(pl => pl.title == title)))
                    {
                        Logger.Logger.ErrorLog("{0}", diff);
                        foreach (var prod in vke.ProductList.Where(x => x.Title == diff))
                            vke.DeleteProduct(prod.ID);
                    }
                }*/
                foreach (var album in vke.AlbumList.Where(alb => alb.ID > 0))
                {
                    var goodsInAlbum = vke.GetAllGoods(album.ID);
                    if (goodsInAlbum.Count == 0)
                    {
                        Logger.Logger.ErrorLog("Удаляем пустую подборку: {0}",
                            album.Title);
                        vke.RemoveAlbum(album.ID);
                    }
                }

                int exportedCount = 0;
                using (var db = new ProductContext())
                {
                    if (chosenMode == 0)
                        foreach (Product p in db.Products)
                            if (ExportProductToVK(p, vke, db))
                                exportedCount++;

                    foreach (Product p in db.Products)
                        if (db.Translations.Any(t => t.titleEng == p.title))
                            if (ExportProductToVK(p, vke, db))
                                exportedCount++;
                }
                Logger.Logger.SuccessLog("Экспортировано {0} товаров", exportedCount);
            }
            else if (chosenMode == 2)
            {
                // забор переводов с ВК
                using (var db = new ProductContext())
                {

                    List<string> newTitles = vke.ProductList.Select(p => p.Title).
                        Where(title => !db.Products.Any(pl => pl.title.Replace("&", "") == title.Replace("and", ""))
                        && !db.Translations.Any(tr => tr.title == title)
                        ).ToList();

                    Logger.Logger.ErrorLog("Продукты в ВК, но не вошедшие в БД, заносим в БД: {0}",
                        newTitles.Count());
                    foreach (var diff in newTitles)
                    {
                        Logger.Logger.ErrorLog("{0}", diff);
                        foreach (var prod in vke.ProductList.Where(x => x.Title == diff))
                            if (!db.Translations.Any(t => prod.Title.Contains(t.title))
                                && !db.Products.Any(t => prod.Title.Contains(t.title)))
                            {
                                Translation t = new Translation();
                                t.title = prod.Title;
                                t.desc = prod.Description;
                                Logger.Logger.SuccessLog("Добавляем перевод {0}", prod.Title);
                                var prodTied = db.Products.FirstOrDefault(p => prod.Description.Contains(p.artikul));
                                if (prodTied != null)
                                {
                                    t.titleEng = prodTied.title;
                                    Logger.Logger.SuccessLog("Найден SKU для товара {0}", prodTied.title);
                                }

                                //prod.PhotoID;

                                db.Translations.Add(t);
                                db.SaveChanges();
                            }
                    }
                }
            }
            else if (chosenMode == 3)
            {
                DeleteUntranslatedProductsHavingTranslation(vke);
            }
            Console.WriteLine("Press smth");
            Console.ReadLine();
        }


        /// <summary>
        /// Удалить непереведенные дубли переводов в ВК
        /// </summary>
        public static void DeleteUntranslatedProductsHavingTranslation(ProductVKExporter vke)
        {
            Comparator cb = new Comparator();
            // из БД получаем список всех артикулов, названий на англ, названий на рус
            foreach (Product prodDB in db.Products)
            {
                // в вкшном массиве ищем товары, включающие этот артикул, находим рус товар и англ товар по переводу этого товара в БД        
                List<MarketEntity> goodsOfSameSKU = vke.ProductList.Where(p => p.Description.Contains(
                    string.Format("(артикул {0})",prodDB.artikul))).ToList();
                MarketEntity rusProd = new MarketEntity();
                MarketEntity engProd = new MarketEntity();
                bool rusFound = false, engFound = false;

                if (goodsOfSameSKU.Count > 1)
                {
                    foreach (MarketEntity sameSKUgood in goodsOfSameSKU)
                    {
                        Console.WriteLine("Artikul {0}, Title {1} ", prodDB.artikul, sameSKUgood.Title);
                        // ищем в бд переводы этого товара
                        List<Translation> transOfThisProd = db.GetTranslationsOfProduct(prodDB.title);
                        // среди переводов этого товара находим с таким же названием как товар в ВК
                        foreach (Translation tranOfThisProd in transOfThisProd)
                            //(t => t.title == sameSKUgood.Title);
                            if (tranOfThisProd.title == sameSKUgood.Title)
                            //if(transOfThisProd != null)
                            {
                                rusProd = sameSKUgood;
                                rusFound = true;
                    Logger.Logger.Trace("Artikul {0}, Title Rus {1}", prodDB.artikul, rusProd.Title);
                            }
                            else Logger.Logger.ErrorLog("{0} =/= {1}", tranOfThisProd.title, sameSKUgood.Title);
                        if(prodDB.title.Replace("&", "and") == sameSKUgood.Title.Replace("&", "and"))
                        {
                            engProd = sameSKUgood;
                            engFound = true;
                    Logger.Logger.Trace("Artikul {0}, Title Eng {1}", prodDB.artikul, engProd.Title);
                        }
                        else Logger.Logger.ErrorLog("{0} =/= {1}", prodDB.title, sameSKUgood.Title);
                        //if(transOfThisProd.Any(t => t.title == ))
                    }
                }
                if (rusFound && engFound)
                {
                    // если нашелся рус товар и англ товар, то удаляем англ товар
                    Logger.Logger.SuccessLog("Remove from VK {0}", engProd.Title);
                    vke.DeleteProduct(engProd.ID);
                }
            }
        }


        /// <summary>
        /// экспортировать товар из БД в ВК
        /// </summary>
        /// <param name="g"></param>
        /// <param name="vke"></param>
        /// <param name="db"></param>
        /// <returns></returns>
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
            // делаем цикл на случай неудачных попыток
            while (prodID == 0 && counter < maxTries)
                try
                {
                    // если есть перевод товара
                    Translation translation = db.Translations.FirstOrDefault(t => t.titleEng == g.title);
                    if (translation != null)
                    {
                        prodID = vke.ExportProduct(translation.title, translation.desc.Replace("&", " n "), g.price, g.imageFileName, g.artikul);
                        vke.AddProductToAlbum(g.title, prodID, ProductVKExporter.mainAlbumTitle, g.imageFileName);
                    }
                    else
                        prodID = vke.ExportProduct(g.title, g.desc.Replace("&", " n "), g.price, g.imageFileName, g.artikul);

                    foreach (LinkProductWithCategory link in g.Links)
                    {
                        // если есть перевод подборки, меняем название на перевод
                        Translation translationAlbum = db.Translations.FirstOrDefault(t => t.titleEng == link.category.title);

                        vke.AddProductToAlbum(g.title, prodID, 
                            (translationAlbum == null)?(link.category.title):(translationAlbum.title), 
                            g.imageFileName);
                    }

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

            // сохраняем картинку товара, т.к. это может быть из-за мелкой картинки
            if (counter >= maxTries)
            {
                File.Copy(vke.GetImageFilePath(g.imageFileName), vke.GetTooSmallImageFilePath(g.imageFileName), true);
            }
            locker = false;
            return result;
        }

    }
}

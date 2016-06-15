﻿using System;
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
            string[] modes = new string[] {"Полный импорт", "Импорт переводов", "Забрать переводы с ВК"};
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
                foreach (var album in vke.AlbumList)
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
            else if(chosenMode == 2)
            {
                // забор переводов с ВК
                using (var db = new ProductContext())
                {
                    List<string> newTitles = vke.ProductList.Select(p => p.Title).
                        Where(title => !db.Products.Any(pl => pl.title == title)
                        && !db.Translations.Any(tr => tr.title == title)
                        ).ToList();

                    Logger.Logger.ErrorLog("Продукты в ВК, но не вошедшие в БД, заносим в БД: {0}",
                        newTitles.Count());
                    foreach (var diff in newTitles)
                    {
                        Logger.Logger.ErrorLog("{0}", diff);
                        foreach (var prod in vke.ProductList.Where(x => x.Title == diff))
                            if (!db.Translations.Any(t => prod.Title.Contains(t.title)))
                            {
                                Translation t = new Translation();
                                t.title = prod.Title;
                                t.desc = prod.Description;
                                
                                //prod.PhotoID;

                                db.Translations.Add(t);
                                db.SaveChanges();
                            }
                    }
                }
            }
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

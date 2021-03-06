﻿using System;
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

using HtmlAgilityPack;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

using DeadSeaVKExport;
using DeadSeaCatalogueDAL;
using Logger;

namespace DeadSeaCosmeticsImport
{

    class DSImport
    {
        const string rootURL = "http://www.israel-catalog.com";
        const string rootURLdir = "dead-sea-cosmetics";
        const string cacheDir = "page_cache";
        const string resultsDir = "results";
        const string imagesDir = resultsDir + "\\images";
        const string resultFile = "_result.rus.html";
        const int TranslationsToPrint = 4;
        const int sleepingBeauty = 5000;

        static bool locker = false;
        static DateTime now = DateTime.Now;
        static int counter = 1;
        static int sleepTime = 100;
        //static List<Product> goodsList = new List<Product>();
        //static ProductContext db = new ProductContext();
        static ProductVKExporter vke;

        //static VkApi vk;

        public static void Export(ProductContext db , Product g)
        {

            //string resultFileName = string.Format("{1}\\results{0}.html", counter++, resultsDir);
            //File.Delete(resultFileName);

            string resultFileName = string.Format("{1}\\{2}", counter++, resultsDir, resultFile);
            string init = "";
            if (!File.Exists(resultFileName))
                init = "<html><body><table border = 1> ";

            using (StreamWriter swRes = new StreamWriter(resultFileName, true)) // DateTime.Now.ToString().Replace(":", "-"), true)))
            {
                swRes.WriteLine(init);
                //foreach (Good g in goodsList)
                //                    Product g = this;
                {
                    swRes.WriteLine("<tr><td>{0} <br> {6}</td><td>{1} <br> {7}</td><td>{2}</td><td>{3}</td><td>{4}</td><td><img src=images\\{5}></td></tr>",
                        g.Links[0].category, g.title, g.price, g.desc, g.details, g.imageFileName, g.Links[0].category.titleRus, g.titleRus);

                    /*
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
                            prodID = vke.ExportProduct(g.title, g.desc, g.Links[0].category.Name, g.price, g.imageFileName);
                        }
                        catch (Exception ex)
                        {
                            Logger.Logger.Trace(ex.Message);
                            Logger.Logger.Trace("Неудачная попытка... ждемс {0}", counter++);
                            Thread.Sleep(sleepTime);
                            if (counter > 10)
                            {
                                ErrorLog("Внимание внимание, что то пошло не так!");
                                //Console.ReadLine();
                            }
                        }
                    locker = false;
                    */
                    //vke.AddProductToAlbum(g.title, prodID, g.imageFileName, g.Links[0].category);
                    swRes.Flush();
                }
            }
        }        

        static void Main(string[] args)
        {
            Logger.Logger.Trace("START");

            //goodsList = new List<Product>();
            //goodsList = new List<Product>();

            string resultFileName = string.Format("{1}\\{2}", counter++, resultsDir, resultFile);
            File.Delete(resultFileName);

            //vk = Auth();


            //vke = new ProductVKExporter();

            Parsing(string.Format("{0}/{1}", rootURL, rootURLdir), 0);
            Console.ReadLine();
        }

        private static string MakeUpCacheFilePath(string siteURL, string title = "")
        {
            string url = siteURL.Replace(rootURL, "").Replace("/", "__").Replace("?", "__").Replace("&", "__");
            if (siteURL.Contains("yandex"))
                url = "yandex-" + title;
            string filePath = string.Format("{0}\\{1}", PrepareCacheDir(), url);
            return filePath;
        }

        private static string MakeUpScrapFilePath(string siteURL, string title = "")
        {
            string url = siteURL.Split(new string[] { "/" }, StringSplitOptions.None).Last() + ".html";
            
            string filePath = string.Format("{0}\\{1}", PrepareCacheDir(), url);
            return filePath;
        }

        private static string PrepareCacheDir(string dirname = "")
        {
            string dirName = (dirname=="")?("page_cache"):(dirname);
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
            return dirName;
        }

        private static string ReadPageFromCache(string siteURL, string title = "")
        {
            string filePathCache = MakeUpCacheFilePath(siteURL, title);
            string filePathScrap = MakeUpScrapFilePath(siteURL, title);
            if (File.Exists(filePathScrap))
                return File.ReadAllText(filePathScrap);
            if (File.Exists(filePathCache))
                return File.ReadAllText(filePathCache);
            return "";                
        }

        private static void WritePageToCache(string siteURL, string source, string title = "")
        {
            string filePath = MakeUpCacheFilePath(siteURL, title);
            for (int i = 0; i < 1; i++)
            {
                try {
                    File.WriteAllText(filePath, source);
                    return;
                }
                catch(Exception ex)
                {
                    Logger.Logger.Trace(string.Format("Writing HTML (URL={3}) (len={2}) try {0} error: {1}", i,  ex.Message, source.Length, siteURL));
                }
            }
        }

        private static async Task<byte[]> GetFile(string siteURL)
        {
            //Logger.Logger.Trace("sleeping with lock for");
            //while (locker)
            //{
            //    Console.Write(".");
            //    System.Threading.Thread.Sleep(sleepTime);
            //}
            //locker = true;

            //Logger.Logger.Trace("opening " + siteURL);
            Uri uri = new Uri(siteURL);

            HttpClientHandler handler = new HttpClientHandler
            {
                Credentials = new
                        System.Net.NetworkCredential("lolaokey", "cheburashka2017")
            };
            //handler.CookieContainer = new CookieContainer();
            //handler.CookieContainer.Add(uri, new Cookie("PAPVisitorId", "05c6d7dc79e7eae167a5bpHa8ZwCgfaL")); // Adding a Cookie

            HttpClient http = new HttpClient(handler);
            
            var response = await http.GetByteArrayAsync(siteURL);
            //locker = false;
            return response;
        }

        // получаем и разбираем
        private static async void Parsing(string siteURL, int depth, string category = "", string titleCurr = "", string tag = "")
        {
            if (siteURL != rootURL || depth == 0)
            {
                int rootTries = 0, maxRootTries = 5;
                bool successfullTry = false;
                for (; rootTries <= maxRootTries && !successfullTry; rootTries++)
                    try
                    {
                        Logger.Logger.Trace("sleeping with lock for");
                        int sleeping = 0;
                        while (locker && sleeping < sleepingBeauty)
                        {
                            Console.Write(".");
                            System.Threading.Thread.Sleep(sleepTime);
                            sleeping += sleepTime;
                        }
                        locker = true;

                        //Logger.Logger.Trace("checking cached " + siteURL);
                        String source = "";
                        //if(titleCurr == "") 
                        source = ReadPageFromCache(siteURL, tag + titleCurr);
                        if (source == "")
                        {
                           // try
                            {
                                var response = await GetFile(siteURL);
                                //var response = http.GetByteArrayAsync(siteURL);
                                source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                                source = WebUtility.HtmlDecode(source);
                                //if (titleCurr == "") 
                                WritePageToCache(siteURL, source, tag + titleCurr);
                            } 
                            //catch (Exception ex)
                            {
                               // Logger.Logger.ErrorLog(ex.Message);
                               // break;
                            }
                        }
                        HtmlDocument resultat = new HtmlDocument();
                        resultat.LoadHtml(source);

                        locker = false;

                        switch (depth)
                        {

                            case 0:
                                #region root parse
                                List<HtmlNode> categories = resultat.DocumentNode.Descendants().
                                    Where(x => (x.Name == "li" && x.Attributes["class"] != null &&
                                    x.Attributes["class"].Value.Contains("spee"))
                                    //|| x.Attributes["class"].Value.Contains("spee parent"))
                                    ).ToList();

                                foreach (var lis in categories)
                                {
                                    //var li = lis.Descendants("li").ToList();
                                    //foreach (var item in li)
                                    {
                                        //var link = item.Descendants("a").ToList()[0].GetAttributeValue("href", null);
                                        //var img = item.Descendants("img").ToList()[0].GetAttributeValue("src", null);
                                        //var title = item.Descendants("h5").ToList()[0].InnerText;
                                        var link = lis.FirstChild.GetAttributeValue("href", null);
                                        var catTitle = lis.InnerText;
                                        if (link.Contains("/dead-sea-cosmetics/"))
                                        {
                                            //Logger.Logger.Trace(link);
                                            Logger.Logger.Trace(catTitle);

                                            Parsing(rootURL + link, 1, catTitle);
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case 1:
                                #region goods

                                List<HtmlNode> goodsfid = resultat.DocumentNode.Descendants().
                                    Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.
                                    Contains("views-field-field-image-cache-fid"))).ToList();
                                //Contains("field-content"))).ToList();

                                foreach (var goodfid in goodsfid)
                                {
                                    List<HtmlNode> goods = goodfid.Descendants().
                                        Where(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.
                                        Contains("field-content"))).ToList();
                                    foreach (var good in goods)
                                    {
                                        var link = good.FirstChild.GetAttributeValue("href", null);
                                        //var title = good.InnerText;
                                        //Logger.Logger.Trace(link);
                                        //Logger.Logger.Trace(title);
                                        Parsing(rootURL + link, 2, category);
                                    }
                                }
                                #endregion

                                #region next page
                                HtmlNode page = resultat.DocumentNode.Descendants().
                                    FirstOrDefault(x => (x.Name == "li" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("pager-next")));
                                if (page != null)
                                {
                                    string pageLink = page.FirstChild.GetAttributeValue("href", null);
                                    //Logger.Logger.Trace("Going next page: " + pageLink);
                                    //Logger.Logger.Trace("Going next page: " + siteURL + "page");
                                    //string goodsURL = rootURL + pageLink);
                                    if (pageLink != "")
                                        Parsing(rootURL + pageLink, 1, category);
                                }
                                #endregion
                                break;
                            case 2:
                                //string resultFileName = string.Format("results\\result{0}.html", counter++);
                                //File.Delete(resultFileName);
                                //using (StreamWriter swRes = new StreamWriter(resultFileName, true)) // DateTime.Now.ToString().Replace(":", "-"), true)))
                                {
                                    #region product parse
                                    // image
                                    HtmlNode image = resultat.DocumentNode.Descendants().
                                        First(x => (x.Name == "li" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("productitem")));
                                    string imageLink = image.ChildNodes.First(x => x.Name == "div").FirstChild.GetAttributeValue("href", null)
                                        ?? image.ChildNodes.First(x => x.Name == "div").FirstChild.GetAttributeValue("src", null);
                                    //var title = good.InnerText;
                                    //if (imageLink != null)
                                    //{
                                        Logger.Logger.Trace("Image:");
                                        Logger.Logger.Trace(imageLink);
                                        string imageFileName = imageLink.Split('/').Last();
                                        string imageNewPath = string.Format("{0}\\{1}", PrepareCacheDir(imagesDir), imageFileName);
                                        if (!File.Exists(imageNewPath) && imageLink.Contains("http"))
                                        {

                                        //try
                                        {
                                            var response = await GetFile(rootURL + imageLink);
                                            File.WriteAllBytes(imageNewPath, response);
                                        }
                                        //catch (Exception ex)
                                        {
                                           // Logger.Logger.ErrorLog(ex.Message);
                                        }
                                        }
                                    //}

                                    // title
                                    HtmlNode titleDiv = resultat.DocumentNode.Descendants().
                                        First(x => (x.Name == "h1" && x.Attributes["itemprop"] != null && x.Attributes["itemprop"].Value == "name"));
                                    string title = titleDiv.InnerText;
                                    Logger.Logger.Trace("Product:");
                                    Logger.Logger.Trace(title);

                                    // description
                                    HtmlNode descDiv = resultat.DocumentNode.Descendants().
                                        FirstOrDefault(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value == "field-brief"));
                                    string desc = "";
                                    if (descDiv != null && descDiv.ChildNodes.Count > 0)
                                        desc = descDiv.FirstChild.InnerText;
                                    Logger.Logger.Trace("Desc:");
                                    //Logger.Logger.Trace(desc);

                                    // price
                                    HtmlNode priceDiv = resultat.DocumentNode.Descendants().
                                        First(x => (x.Name == "span" && x.Attributes["class"] != null && x.Attributes["class"].Value == "theprice"));
                                    string price = priceDiv.InnerText;
                                    Logger.Logger.Trace("Price");
                                    Logger.Logger.Trace(price);

                                    // artikul SKU
                                    HtmlNode skuDiv = resultat.DocumentNode.Descendants().
                                        First(x => (x.Name == "span" && x.Attributes["class"] != null && x.Attributes["class"].Value == "sku"));
                                    string sku = skuDiv.InnerText.Split(':')[1].Trim();
                                    Logger.Logger.Trace("sku");
                                    Logger.Logger.Trace(sku);

                                    // details
                                    HtmlNode detailsDiv = resultat.DocumentNode.Descendants().
                                        First(x => (x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value == "product-body"));
                                    Logger.Logger.Trace("Details:");
                                    string details = "";
                                    foreach (var detailPartDiv in detailsDiv.ChildNodes)
                                    {
                                        string detailPart = detailPartDiv.InnerText;
                                        details += detailPart;
                                        //Logger.Logger.Trace(detailPart);
                                    }
                                    // получили данные о товаре, заводим или сохраняем
                                    ProductContext.SaveProduct(sku, category, title, "", price, desc, details, imageFileName);
                                    //Parsing(YandexTranslateURL(
                                    //    "trnsl.1.1.20160420T200115Z.006bede5b131c604.4256886cd58598ea537df059cd532b6b141910cf",
                                    //    desc + ";;;" + details), 5, category, title);

                                    // пока прекращаем получать переводы яндекс, все равно не юзаем их
                                    /*
                                    Parsing(YandexTranslateURL(category), 6, category, title, "category-");

                                    Parsing(YandexTranslateURL(title), 7, category, title, "title-");

                                    Parsing(YandexTranslateURL(desc), 3, category, title, "desc-");

                                    Parsing(YandexTranslateURL(details), 4, category, title, "details-");
                                    */

                                    //swRes.WriteLine("{0};{1};{2};{3};{4};{5}", category, title, price, desc, details, imageFileName);
                                    //swRes.WriteLine("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td><img src=images\\{5}></td></tr>", category, title, price, desc, details, imageFileName);
                                    //swRes.Flush();
                                    #endregion
                                }
                                break;
                        }

                        if (depth >= 3 && depth <= 7)
                        {
                            HtmlNode transDiv = resultat.DocumentNode;
                            string translation = transDiv.InnerText;
                            //Logger.Logger.Trace(translation);
                            using (ProductContext db = new ProductContext())
                            {
                                Product g = GetProductByName(db, titleCurr);
                                switch (depth)
                                {
                                    case 5:
                                        g.descRus = translation.Split(new string[] { ";;;" }, StringSplitOptions.None)[0];
                                        g.detailsRus = translation.Split(new string[] { ";;;" }, StringSplitOptions.None)[1];
                                        g.translated++;
                                        break;
                                    case 3:
                                        g.descRus = getTextFromJson(translation);
                                        break;
                                    case 4:
                                        g.detailsRus = getTextFromJson(translation);
                                        break;
                                    case 6:
                                        db.Categories.FirstOrDefault(c => c.title == category).titleRus = getTextFromJson(translation);
                                        break;
                                    case 7:
                                        g.titleRus = getTextFromJson(translation);
                                        break;
                                }

                                g.translated++;
                                if (g.translated == TranslationsToPrint)
                                    Export(db, g);
                                db.SaveChanges();
                            }
                        }
                        //goodsList.Remove(g);
                        //goodsList.Products.Add(g);

                        Logger.Logger.Trace("finished with " + siteURL);
                        //foreach(string element in toftitle)                 Logger.Logger.Trace(element.)

                        successfullTry = true;
                    }
                    catch (Exception ex)
                    {
                        //ConsoleColor defcol = Console.ForegroundColor;
                        //Console.ForegroundColor = ConsoleColor.Red;
                        Logger.Logger.ErrorLog(string.Format("Запрос {0} привел к ошибке {1}", siteURL, ex.ToString()));
                        //Console.ForegroundColor = defcol;
                        
                        locker = false;
                        if (rootTries >= maxRootTries )
                            //throw new Exception("Ошибка", ex);
                            Logger.Logger.ErrorLog("Ошибка " + ex.Message);
                    }
                    finally
                    {
                    }
            }
        }

        private static Product GetProductByName(ProductContext db, string titleCurr)
        {
            return db.Products.FirstOrDefault(x => x.title == titleCurr);
        }


        private static string YandexTranslateURL(string text)
        {
            return string.Format("https://translate.yandex.net/api/v1.5/tr.json/translate?key={0}&text={1}&lang=en-ru",
    "trnsl.1.1.20160420T200115Z.006bede5b131c604.4256886cd58598ea537df059cd532b6b141910cf",
    text.Replace("&", " and ").Replace("#", "N"));
        }

        public static string getTextFromJson(string json)
        {
            Regex reg = new Regex(@"\[(.*)\]");

            return reg.Match(json, 0).Value.Replace("[", "").Replace("]", "");

            //string 
        }

    }
}


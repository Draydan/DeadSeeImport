using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadSeaCatalogueDAL;
using Logger;

// утилита для вытаскивания связей переводов товаров с категориями из гуглодока
namespace DeadSeaKeyWordCategoryLinker
{
    class CatLinker
    {
        static void Main(string[] args)
        {
            using (ProductContext db = new ProductContext())
            {
                foreach (Translation tran in db.Translations.Where(
                    t => t.keyWords != "" && t.isOurCategory))
                {
                    Category cat = new Category();
                    Console.WriteLine("если нету категории {0}", tran.title);
                    if (!db.Categories.Any(ca => ca.titleRus == tran.title))
                    {
                        Console.WriteLine("заводим категорию {0}", tran.title);
                        cat.isOurCategory = true;
                        cat.titleRus = tran.title;                        
                        db.Categories.Add(cat);
                    }
                    else
                        cat = db.Categories.First(ca => ca.titleRus == tran.title);
                    cat.title = tran.titleEng;
                    //List<LinkProductWithCategory> links = cat.Links;
                    //List<LinkProductWithCategory> linksAll = db.Links.ToList();

                    //links = db.Links.Where(l => l.category.titleRus == tran.title).ToList();
                    Console.WriteLine("удаляем связи категории");
                    db.Links.RemoveRange(db.Links.Where(l => l.category.titleRus == tran.title));
                    //cat.Links.Clear();
                    //foreach (var linkToDel in links)                        db.Links.Remove(linkToDel);
                    //linksAll = db.Links.ToList();
                }
                db.SaveChanges();
                Console.WriteLine("берем каждый товар");
                foreach (Product prod in db.Products.Where(p => p.supplier.ID == 1))
                {
                    
                    // берем каждую Нашу категорию
                    foreach (Translation tranCat in db.Translations.Where(
                        t => t.keyWords != "" && t.isOurCategory))
                    {
                        
                        if (prod.supplier.ID == 2)
                        {
                            //Logger.Logger.Trace(prod.title);
                            //Logger.Logger.Trace(tran.title);
                        }
                        Category cat = db.Categories.First(ca => ca.titleRus == tranCat.title);
                        List<string> keyWords = new List<string>();
                        if (tranCat.keyWords != null)
                            keyWords = tranCat.keyWords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        List<string> antiKeyWords = new List<string>();
                        if(tranCat.antiKeyWords != null)
                            antiKeyWords = tranCat.antiKeyWords.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        // для каждого ключевого слова
                        //foreach (string kw in keyWords)
                        //{

                        string det = "";// prod.details;
                        string des = prod.desc;
                        string tit = prod.title;
                        if (prod.title == "Mogador Nurturing Eye Cream, Argan Oil")
                        { }
                        if (//prod.title == "Pomegranate Firming Face and Neck Cream with Dead Sea Minerals SPF 15" || 
                            prod.title == "Mogador Nurturing Eye Cream, Argan Oil")
                            //&& (cat.title == "Grease skin" || tranCat.title.ToLower().Contains("парфюм")))
                        {
                            LogDescriptionContainingKeyword(keyWords, tit);
                            LogDescriptionContainingKeyword(keyWords, det);
                            LogDescriptionContainingKeyword(keyWords, des);
                            LogDescriptionContainingKeyword(antiKeyWords, tit);
                            LogDescriptionContainingKeyword(antiKeyWords, det);
                            LogDescriptionContainingKeyword(antiKeyWords, des);
                        }
                        // если не содержит кс то пропускаем это кс
                        if ((!antiKeyWords.Any(kw => det.Contains(kw))
                            && !antiKeyWords.Any(kw => des.Contains(kw))
                            && !antiKeyWords.Any(kw => tit.Contains(kw)))

                        && (keyWords.Any(kw => det.Contains(kw))
                             || keyWords.Any(kw => des.Contains(kw))
                             || keyWords.Any(kw => tit.Contains(kw))))
                        {
                            if( ! db.Links.Any(li => li.category.ID == cat.ID && li.product.ID == prod.ID))
                            { 

                                LinkProductWithCategory link = new LinkProductWithCategory();
                                link.category = cat;
                                link.product = prod;
                                db.Links.Add(link);
                                //if (prod.artikul == "15778")
                                {
                                    //Logger.Logger.Trace("Product {0} added to {1}", prod.title, cat.titleRus);

                                    /*
                                    Logger.Logger.Trace("keywords");
                                    foreach (string kw in keyWords)
                                    {
                                        if (prod.title.Contains(kw))
                                            Logger.Logger.Trace("Product {0} contains {1}", prod.title, kw);
                                        if (prod.details.Contains(kw))
                                            Logger.Logger.Trace("details {0} contains {1}", prod.details, kw);
                                        if (prod.desc.Contains(kw))
                                            Logger.Logger.Trace("desc {0} contains {1}", prod.desc, kw);
                                    }
                                    Logger.Logger.Trace("anti keywords");
                                    foreach (string kw in antiKeyWords)
                                    {
                                        if (prod.title.Contains(kw))
                                            Logger.Logger.Trace("Product {0} contains {1}", prod.title, kw);
                                        if (prod.details.Contains(kw))
                                            Logger.Logger.Trace("details {0} contains {1}", prod.details, kw);
                                        if (prod.desc.Contains(kw))
                                            Logger.Logger.Trace("desc {0} contains {1}", prod.desc, kw);
                                    }
                                    Logger.Logger.Trace("Desc {0}", prod.desc);
                                    Logger.Logger.Trace("Details {0}", prod.details);
                                    Logger.Logger.Trace("keyWords {0}", keyWords);
                                    Logger.Logger.Trace("antiKeyWords {0}", antiKeyWords);
                                    */

                                }
                            }
                            //break;
                        }
                        else
                        {
                            if (prod.Links.Any(l => (((l.category.titleRus != null) && l.category.titleRus == cat.titleRus) || 
                            l.category.title == cat.title)))
                            {
                                Logger.Logger.Trace("Product {0} will be removed from {1}", prod.title, (cat.titleRus)??(cat.title) );
                                //Category catLinked = prod.Links.First(l => l.category.titleRus == cat.titleRus).category;
                                db.Links.RemoveRange(db.Links.Where(li => (li.category.titleRus == cat.titleRus || 
                                    li.category.title == cat.title) && li.product.title == prod.title));
                            }
                        }
                        
                    }
                }
                    db.SaveChanges();
                    Console.WriteLine("press");
                    Console.ReadLine();
            }
        }

        static void LogDescriptionContainingKeyword(List<string> keywords, string description)
        {
            foreach (string kw in keywords)
                if (description.Contains(kw))
                {
                    Logger.Logger.SuccessLog(kw);
                    Logger.Logger.Trace("{0} contained in '{1}'", kw, description);
                }
        }
    }
}

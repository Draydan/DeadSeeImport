using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadSeaCatalogueDAL;
using Logger;

namespace DeadSeaKeyWordCategoryLinker
{
    class Program
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
                    //List<LinkProductWithCategory> links = cat.Links;
                    //List<LinkProductWithCategory> linksAll = db.Links.ToList();

                    //links = db.Links.Where(l => l.category.titleRus == tran.title).ToList();
                    db.Links.RemoveRange(db.Links.Where(l => l.category.titleRus == tran.title));
                    //cat.Links.Clear();
                    //foreach (var linkToDel in links)                        db.Links.Remove(linkToDel);
                    //linksAll = db.Links.ToList();
                }
                db.SaveChanges();
                Console.WriteLine("берем каждый товар");
                foreach (Product prod in db.Products)
                {
                    // берем каждую Нашу категорию
                    foreach (Translation tran in db.Translations.Where(
                        t => t.keyWords != "" && t.isOurCategory))
                    {
                        Category cat = db.Categories.First(ca => ca.titleRus == tran.title);
                        
                        List<string> keyWords = tran.keyWords.Split(',').ToList();
                        List<string> antiKeyWords = new List<string>();
                        if(tran.antiKeyWords != null)
                            antiKeyWords = tran.antiKeyWords.Split(',').ToList();
                        // для каждого ключевого слова
                        //foreach (string kw in keyWords)
                        //{


                        // если не содержит кс то пропускаем это кс
                        if ((!antiKeyWords.Any(kw => prod.details.Contains(kw))
                            && !antiKeyWords.Any(kw => prod.desc.Contains(kw)))
                        && (keyWords.Any(akw => prod.details.Contains(akw))
                             || keyWords.Any(kw => prod.desc.Contains(kw))))
                        {

                            LinkProductWithCategory link = new LinkProductWithCategory();
                            link.category = cat;
                            link.product = prod;
                            db.Links.Add(link);
                            Logger.Logger.Trace("Product {0} added to {1}", prod.title, cat.titleRus);
                            //break;
                        }
                        else
                        {
                            if (prod.Links.Any(l => l.category.titleRus == cat.titleRus))
                            {
                                //Category catLinked = prod.Links.First(l => l.category.titleRus == cat.titleRus).category;
                                db.Links.RemoveRange(db.Links.Where(li => li.category.titleRus == cat.titleRus && li.product.title == prod.title));
                            }
                        }
                        
                    }
                    //Console.WriteLine("press");
                    //Console.ReadLine();
                }
                    db.SaveChanges();
            }
        }
    }
}

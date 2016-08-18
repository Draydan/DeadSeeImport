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
                    Console.WriteLine("ЕСЛИ НЕТУ КАТЕГОРИИ {0}, ЗАВОДИМ", tran.title);
                    if (!db.Categories.Any(ca => ca.titleRus == tran.title))
                    {
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
                    cat.Links.Clear();
                    //foreach (var linkToDel in links)                        db.Links.Remove(linkToDel);
                    //linksAll = db.Links.ToList();
                    db.SaveChanges();
                }
                Console.WriteLine("берем каждый товар");
                foreach (Product prod in db.Products)
                {
                    foreach (Translation tran in db.Translations.Where(
                        t => t.keyWords != "" && t.isOurCategory))
                    {
                        Category cat = db.Categories.First(ca => ca.titleRus == tran.title);

                        
                        List<string> keyWords = tran.keyWords.Split(',').ToList();
                        foreach (string kw in keyWords)
                        {
                            if (!prod.details.Contains(kw) && !prod.desc.Contains(kw)) continue;

                            LinkProductWithCategory link = new LinkProductWithCategory();
                            link.category = cat;
                            link.product = prod;
                            db.Links.Add(link);
                            Logger.Logger.Trace("Product {0} added to {1}", prod.title, cat.titleRus);
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

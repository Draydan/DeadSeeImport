using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadSeaCatalogueDAL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("trying");
            using (var db = new ProductContext())
            {
                //Console.WriteLine("opened, adding");
                //db.Categories.Add(new Category { title = "test", titleRus = "тест" });
                //db.Products.Add(new Product(db, "", "test", "test prod", "", "", "", "")); //{ category = db.Categories.First(x => x.Name == "test"), title = "test prod"});
                //Console.WriteLine("Saving");
                //db.SaveChanges();

                /*               
                               Console.WriteLine("products:");
                               foreach (Product g in db.Products)
                               {
                                   Console.WriteLine(g.title);
                               }

                               //db.Translations.Add(new Translation { title = "test" });

                               Console.WriteLine("translations:");
                               foreach (Translation g in db.Translations)
                               {
                                   Console.WriteLine(g.title);
                               }
                               db.SaveChanges();

                               */

                // ищем несцепленные переводы, схожие с автопереводами товаров, чтобы сцепить
                Console.WriteLine("Unlinked translations:");
                FuzzyHelper.Comparator comp = new FuzzyHelper.Comparator();

                foreach (Translation g in db.Translations.Where(t => t.titleEng == null))
                {
                    Console.WriteLine("Ручной перевод: {0}", g.title);
                    string bestCompare = "";
                    int maxCompare = 0; //= db.Products.Max(mp => comp.FuzzyStringCompare_2side(mp.titleRus, g.title));

                    //bestCompare = db.Products.Select(p => p.titleRus).FirstOrDefault(tr => comp.FuzzyStringCompare_2side(tr, g.title) == maxCompare);

                    comp.FindBestComparison(g.title, db.Products.Select(p => p.titleRus).ToList(), out bestCompare, out maxCompare);
                    /*
                    foreach(string prodTitle in db.Products.Select(p => p.titleRus))
                    {
                        string fixTitle = prodTitle.Replace("Dead Sea Cosmetics", "Косметика Мертвого Моря").
                            Replace("Dead Sea", "Мертвого Моря");
                        int compare = comp.FuzzyStringCompare_2side(fixTitle, g.title);
                        if (maxCompare < compare)
                        {
                            maxCompare = compare;
                            bestCompare = prodTitle;
                            Console.WriteLine("Up: {0} = ({1} =??= {2})", maxCompare, prodTitle, g.title);
                        }
                    }
                    */
                    Console.WriteLine("Авто перевод: {0}", bestCompare);

                }

                // ищем и удаляем переводы, кот. на самом деле англ.названия товаров
                foreach(Translation t in db.Translations)
                {
                    if (!Translation.HasRussianLetters(t.title))
                    {
                        Console.WriteLine("removing {0}", t.title);
                        db.Translations.Remove(t);
                    }
                }
                db.SaveChanges();


                Console.WriteLine("closing, press");
                Console.ReadLine();
            }
        }
    }
}

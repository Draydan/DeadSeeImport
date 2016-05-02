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
                Console.WriteLine("opened, adding");
                db.Categories.Add(new Category { Name = "test", NameRus = "тест" });
                db.Products.Add(new Product(db, "", "test", "test prod", "", "", "", "")); //{ category = db.Categories.First(x => x.Name == "test"), title = "test prod"});
                Console.WriteLine("Saving");
                db.SaveChanges();
                Console.WriteLine("closing, press");
                Console.ReadLine();
            }
        }
    }
}

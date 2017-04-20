using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace DeadSeaCatalogueDAL
{
    public class ProductContext : DbContext

    {
        public ProductContext():base()
        { }                    

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LinkProductWithCategory> Links { get; set; }

        public DbSet<Translation> Translations { get; set; }

        public List<Translation> GetTranslationsOfProduct(string productTitle)
        {
            return Translations.Where(t => t.titleEng == productTitle).ToList();
        }
        public Product GetProductByTitle(string productTitle)
        {
            return Products.FirstOrDefault(p => p.title == productTitle);
        }
    }
}

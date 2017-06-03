using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;

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

        public static void SaveProduct(string sku, string category, string title, string price, string pricefull, string desc, string details, string imageFileName, bool IsPriceExtrapolated = false)
        {
            using (ProductContext db = new ProductContext())
            {
                // если в товарах более 1 с таким артикулом или именем, то надо зачистить и завести заново
                if (db.Products.Where(x => x.artikul == sku || x.title == title).Count() > 1)
                {
                    db.Links.RemoveRange(
                        db.Links.Where(
                            l => db.Products.Where(
                                p => p.artikul == sku || p.title == title).
                                    Contains(l.product)));
                    db.Products.RemoveRange(db.Products.Where(x => x.artikul == sku || x.title == title));
                    db.SaveChanges();
                }
                Product g = db.Products.FirstOrDefault(x => x.artikul == sku || x.title == title);
                if (g == null)
                {
                    g = new Product(db, sku, category, title, price, pricefull, desc, details, imageFileName);
                    db.Products.Add(g);
                }
                else
                    g.Edit(db, sku, category, title, price, pricefull, desc, details, imageFileName);
                g.priceIsFromSiteNotExtrapolated = ! IsPriceExtrapolated;

                int tries = 0;
                int maxtries = 10;
                while (tries < maxtries)
                {
                    try
                    {
                        db.SaveChanges();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (tries++ >= maxtries) throw ex;
                    }
                }
            }
        }
    }
}

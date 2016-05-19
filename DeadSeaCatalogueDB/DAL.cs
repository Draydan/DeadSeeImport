using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace DeadSeaCatalogueDAL
{
    public class Category
    {
        [Key]
        public long ID {get; set;}
        public string title {get; set;}
        public string titleRus {get; set;}
        public virtual List<LinkProductWithCategory> Links {get; set;}
        public Category()
        { }
        public Category(string title_)
        {
            title = title_;
            Links = new List<LinkProductWithCategory>();
        }
    }

    public class LinkProductWithCategory
    {
        [Key]
        public long ID { get; set; }

        public virtual Category category
        {
            get; set;
        }

        public virtual Product product
        {
            get; set;
        }
    }

    public class Product
    {
        public virtual List<LinkProductWithCategory> Links { get; set; }
        [Key]
        public long ID {get; set;}
        public string artikul { get; set; }
        public string title {get; set;}
        public string titleRus {get; set;}
        public string price {get; set;}
        public string desc {get; set;}
        public string details {get; set;}
        public string descRus {get; set;}
        public string detailsRus {get; set;}
        public string imageFileName {get; set;}
        public int translated {get; set;}

        public Product()
        { }

        /// <summary>
        /// заводим продукт в БД
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sku"></param>
        /// <param name="cat"></param>
        /// <param name="tit"></param>
        /// <param name="pr"></param>
        /// <param name="dsc"></param>
        /// <param name="det"></param>
        /// <param name="img"></param>
        public Product(ProductContext db, string sku, string cat, string tit, string pr, string dsc, string det, string img)
        {
            Links = new List<LinkProductWithCategory>();
            Edit(db, sku, cat, tit, pr, dsc, det, img);
            translated = 0;
        }

        public void Edit(ProductContext db, string sku, string cat, string tit, string pr, string dsc, string det, string img)
        {
            //category = cate;
            LinkProductWithCategory link = new LinkProductWithCategory();
            link.category = db.Categories.FirstOrDefault(x => x.title == cat);
            if (link.category == null)
            {
                link.category = new Category(cat);// { title = cat };
                link.category.Links.Add(link);
                db.Categories.Add(link.category);
            }
            if (link.product == null)
                link.product = this;
            Links.Add(link);
            db.Links.Add(link);
            //db.SaveChanges();

            artikul = sku;
            title = tit;
            price = pr;
            desc = dsc;
            details = det;
            imageFileName = img;
        }
    }

    public class Translation
    {        
        [Key]
        public long ID { get; set; }
        public string titleEng { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string details { get; set; }
        public string ingridients { get; set; }
    }
        public class ProductContext : DbContext
    {
        public ProductContext():base()
        { }                    

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LinkProductWithCategory> Links { get; set; }

        public DbSet<Translation> Translations { get; set; }
    }
}

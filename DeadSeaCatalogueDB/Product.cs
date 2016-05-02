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
        public string Name {get; set;}
        public string NameRus {get; set;}
        public virtual List<Product> Products {get; set;}
    }

    public class Product
    {
        public virtual Category category
        {
            get; set;
        }

        [Key]
        public long ID {get; set;}
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
        public Product(ProductContext db, string cat, string tit, string pr, string dsc, string det, string img)
        {
            //category = cate;
            category = db.Categories.FirstOrDefault(x => x.Name == cat);
            if(category == null)
            {
                category = new Category { Name = cat };
                db.Categories.Add(category);
                db.SaveChanges();
            }
            title = cat;
            title = tit;
            price = pr;
            desc = dsc;
            details = det;
            imageFileName = img;
            translated = 0;
        }
        

    }

    public class ProductContext : DbContext
    {
        public ProductContext():base()
        { }                    

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}

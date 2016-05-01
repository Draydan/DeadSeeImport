using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DeadSeaCatalogueDB
{
    public class Category
    {
        long ID;
        public string Name;
        public string NameRus;
        public virtual List<Product> Products {get; set;}
    }

    public class Product
    {
        public virtual Category category
        {
            get; set;
        }

        public long ID;
        public string title;
        public string titleRus;
        public string price;
        public string desc;
        public string details;
        public string descRus;
        public string detailsRus;
        public string imageFileName;
        public int translated;

        public Product(string cat, string t, string p, string d, string det, string im)
        {
            //category = cate;
            title = cat;
            title = t;
            price = p;
            desc = d;
            details = det;
            imageFileName = im;
            translated = 0;
        }

        public class ProductContext : DbContext
        {
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
        }

    }
    }

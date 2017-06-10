using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Globalization;

namespace DeadSeaCatalogueDAL
{

    public class Product
    {
        public virtual List<LinkProductWithCategory> Links { get; set; }
        [Key]
        public long ID { get; set; }
        public string artikul { get; set; }
        public string title { get; set; }
        public string titleRus { get; set; }
        public string price { get; set; }
        public string priceFull { get; set; }
        public string desc { get; set; }
        public string details { get; set; }
        public string descRus { get; set; }
        public string detailsRus { get; set; }
        public string imageFileName { get; set; }
        public int translated { get; set; }
        public bool priceIsFromSiteNotExtrapolated { get; set; }
        public string wpUrl { get; set; }
        public string wpImageUrl { get; set; }


        public virtual Supplier supplier
        {
            get; set;
        }

        public Product()
        { }

        public float numericPriceRub
        {
            get
            {
                return numericPrice * 1.1f;
            }
        }

        public float numericPrice
        {
            get
            {                
                return float.Parse(price.Replace("$", "").Replace(".", ","));
            }
        }

        public float numericPriceFull
        {
            get
            {
                return float.Parse(priceFull.Replace("$", ""));
            }
        }

        public static float numericBaks(string baks)
        {
            // Set current thread culture to en-US.
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            return float.Parse(baks.Replace("$", ""));
        }

        public Category OurCategory(ProductContext db)
        {
            //return  this.Links.Select(li => li.category).Where(cat => cat.isOurCategory).First();
            foreach (Category cat in db.Categories)
                foreach (LinkProductWithCategory link in this.Links)
                    if (cat.isOurCategory && cat.Links.Any(li => li.product.ID == link.product.ID))
                        return cat;
            return null;
        }
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
        public Product(ProductContext db, string sku, string cat, string tit, string pr, string prfull, string dsc, string det, string img)
        {
            Links = new List<LinkProductWithCategory>();
            Edit(db, sku, cat, tit, pr, prfull, dsc, det, img);
            translated = 0;
        }

        /// <summary>
        /// Меняем данные об этом товаре
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sku"></param>
        /// <param name="cat"></param>
        /// <param name="tit"></param>
        /// <param name="pr"></param>
        /// <param name="prfull"></param>
        /// <param name="dsc"></param>
        /// <param name="det"></param>
        /// <param name="img"></param>
        public void Edit(ProductContext db, string sku, string cat, string tit, string pr, string prfull, string dsc, string det, string img)
        {
            // добавляем ссылку на эту категорию
            //category = cate;
            LinkProductWithCategory link = new LinkProductWithCategory();
            link.category = db.Categories.FirstOrDefault(x => x.title == cat);
            // создаем категорию если нету признаков ошибки
            if (link.category == null && cat != "" && cat != null && !cat.Contains(";;"))
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
            if(pr != "")
                price = pr;
            if (prfull != "")
                priceFull = prfull;
            desc = dsc;
            details = det;
            if(imageFileName != "")
                imageFileName = img;
        }
    }
}

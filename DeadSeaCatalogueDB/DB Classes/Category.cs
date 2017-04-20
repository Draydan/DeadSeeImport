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
        public long ID { get; set; }
        public string title { get; set; }
        public string titleRus { get; set; }
        public virtual List<LinkProductWithCategory> Links { get; set; }
        public bool isOurCategory { get; set; }

        public Category()
        { }
        public Category(string title_)
        {
            title = title_;
            Links = new List<LinkProductWithCategory>();
        }
    }
}

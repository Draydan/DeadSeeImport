using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Globalization;

namespace DeadSeaCatalogueDAL
{
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
}

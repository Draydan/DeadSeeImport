using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DeadSeaCatalogueDAL
{
    public  class Supplier
    {
        [Key]
        public long ID { get; set; }

        public string title { get; set; }

        public virtual List<Product> Products { get; set; }

    }
}

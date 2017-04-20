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

    public class Translation
    {
        [Key]
        public long ID { get; set; }
        public string titleEng { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string details { get; set; }
        public string ingridients { get; set; }
        public bool isOurCategory { get; set; }
        public string keyWords { get; set; }
        public string antiKeyWords { get; set; }

        public static bool HasRussianLetters(string t)
        {
            for (int i = 0; i < t.Length; i++)
            {
                string s = t.Substring(i, 1);
                if ("абвгдеёжзийклмнопрсутфхцчшщъыьэюя".Contains(s.ToLower()))
                    return true;
            }
            return false;
        }

    }
}

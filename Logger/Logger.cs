using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class Logger
    {
        public static void ErrorLog(string text, params object[] args)
        {
            ConsoleColor defcol = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Trace(text, args);
            Console.ForegroundColor = defcol;
        }

        public static void Trace(string text, params object[] args)
        {
            Console.WriteLine(text);
            using (StreamWriter sw = new StreamWriter("log.txt", true))
            {
                sw.WriteLine(DateTime.Now + " : " + text, args);
            }
        }
    }
}

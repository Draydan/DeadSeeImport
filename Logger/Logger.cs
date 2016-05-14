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
        public static void SuccessLog(string text, params object[] args)
        {
            ConsoleColor defcol = Console.ForegroundColor;
            if (defcol == ConsoleColor.Green) defcol = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Green;
            Trace(text, args);
            Console.ForegroundColor = defcol;
        }
        public static void ErrorLog(string text, params object[] args)
        {
            ConsoleColor defcol = Console.ForegroundColor;
            if (defcol == ConsoleColor.Red) defcol = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Red;
            Trace(text, args);
            Console.ForegroundColor = defcol;
        }

        public static void Trace(string text, params object[] args)
        {
            Console.WriteLine(text, args);
            try
            {
                using (StreamWriter sw = new StreamWriter("log.txt", true))
                {
                    sw.WriteLine(DateTime.Now + " : " + text, args);
                }
            }
            catch (Exception ex)
            {
                ConsoleColor defcol = Console.ForegroundColor;
                if (defcol == ConsoleColor.Red) defcol = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Red;
                ErrorLog(text);
                Console.ForegroundColor = defcol;
            }
        }
    }
}

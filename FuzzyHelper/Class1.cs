using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;

namespace FuzzyHelper
{
  
    public class Comparator
    {

        public int FuzzyStringCompare(string etalon, string subject)
        {
            int functionReturnValue = 0;
            
            if (Strings.Len(etalon) == 0 | Strings.Len(subject) == 0 & Strings.Len(etalon) != Strings.Len(subject))
            {
                functionReturnValue = 0;
            }
            else {
                etalon = Strings.Replace(etalon, " ", "");
                subject = Strings.Replace(subject, " ", "");
                int i = 0;
                int j = 0;
                int lastFoundPos = 0;
                lastFoundPos = 0;
                int res1 = 0;
                int res2 = 0;
                //сравниваем побуквенно эталон с субъектом и наоборот
                // ищем буквы эталона
                for (i = 1; i <= Strings.Len(etalon); i++)
                {
                    for (j = lastFoundPos + 1; j <= Strings.Len(subject); j++)
                    {
                        if (Strings.LCase(Strings.Mid(etalon, i, 1)) == Strings.LCase(Strings.Mid(subject, j, 1)))
                        {
                            //++ math.inc(res1)
                            res1 = res1 + 1;
                            lastFoundPos = j;
                            //'tracetranslit Mid(etalon,i,1) & "(" & i & ")" & "=" & Mid(subject, j, 1) & "(" & j & ")" & "( res = " & res1 & ")"
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
                res1 = res1 * 100 / Strings.Len(etalon);

                // возвращаем среднее от результата двух сравнений
                //FuzzyStringCompare = (res1+res2)/2
                double firstLet = 0;
                double firstLetterCoef = 0;
                firstLet = 0;
                if ((Strings.Left(etalon, 1) == Strings.Left(subject, 1)))
                    firstLet = 100;


                int etalonCoef = 0;
                int subjectCoef = 0;
                int lenCoef = 0;
                etalonCoef = 2;
                subjectCoef = 0;
                lenCoef = 0;
                firstLetterCoef = 0;
                functionReturnValue = 
                    (int)((firstLet * firstLetterCoef + res1 * etalonCoef + 100 * (1 - Math.Abs(Strings.Len(etalon) - Strings.Len(subject))
                    / Strings.Len(etalon)) * lenCoef) / (etalonCoef + subjectCoef + lenCoef + firstLetterCoef));
                //FuzzyStringCompare = res1*res2/100
            }
            return functionReturnValue;
        }


        public double AmountOfLettersOfWordInOtherWord(string etalon, string subject)
        {

            const int maxDiff = 3;
            int i = 0;
            int j = 0;
            int lastFoundPos = 0;
            lastFoundPos = 0;
            double res1 = 0;
            double res2 = 0;

            for (i = 1; i <= Strings.Len(etalon); i++)
            {
                for (j = lastFoundPos + 1; j <= Strings.Len(subject); j++)
                {
                    if (j > lastFoundPos + 1 + maxDiff)
                        break; // TODO: might not be correct. Was : Exit For

                    if (Strings.LCase(Strings.Mid(etalon, i, 1)) == Strings.LCase(Strings.Mid(subject, j, 1)))
                    {
                        //++ math.inc(res1)
                        res1 = res1 + 1;
                        lastFoundPos = j;
                        if (Strings.InStr(i + 1, etalon, Strings.Mid(etalon, i, 1)) > 0)
                        {
                            lastFoundPos = i;
                        }
                        if ((etalon == "Аораханскаяобласть" | subject == "Аораханскаяобласть") & (etalon == "Астраханскаяобласть" | subject == "Астраханскаяобласть"))
                        {
                            //Trace Mid(etalon,i,1) & "(" & i & ")" & "=" & Mid(subject, j, 1) & "(" & j & ")" & "( res = " & res1 & ")"
                        }
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            return res1 * 100 / Strings.Len(etalon);
        }


        public int FuzzyStringCompare_2side(string etalon, string subject)
        {
            int functionReturnValue = 0;
            if (Strings.Len(etalon) == 0 | Strings.Len(subject) == 0 & Strings.Len(etalon) != Strings.Len(subject))
            {
                functionReturnValue = 0;
            }
            else {
                etalon = Strings.Replace(etalon, " ", "");
                subject = Strings.Replace(subject, " ", "");
                int i = 0;
                int j = 0;
                int lastFoundPos = 0;
                lastFoundPos = 0;
                double res1 = 0;
                double res2 = 0;

                //сравниваем побуквенно эталон с субъектом и наоборот
                // ищем буквы эталона

                res1 = AmountOfLettersOfWordInOtherWord(etalon, subject);
                res2 = AmountOfLettersOfWordInOtherWord(subject, etalon);


                // возвращаем среднее от результата двух сравнений
                //FuzzyStringCompare = (res1+res2)/2
                double firstLet = 0;
                double firstLetterCoef = 0;
                firstLet = 0;
                if ((Strings.Left(etalon, 1) == Strings.Left(subject, 1)))
                    firstLet = 100;


                int etalonCoef = 0;
                int subjectCoef = 0;
                int lenCoef = 0;
                etalonCoef = 1;
                subjectCoef = 1;
                lenCoef = 0;
                firstLetterCoef = 0;
                functionReturnValue = (int)((firstLet * firstLetterCoef + res1 * etalonCoef + res2 * subjectCoef + 
                    100 * (1 - Math.Abs(Strings.Len(etalon) - Strings.Len(subject)) / Strings.Len(etalon)) * lenCoef) / 
                    (etalonCoef + subjectCoef + lenCoef + firstLetterCoef));

            }
            return functionReturnValue;
        }

        public int FuzzyPhraseCompare_2side(string etalon, string obj)
        {
            string[] etalWords = null;
            string[] objWords = null;
            string bestCompare = "";
            int maxCompare = 0;
            int sumCompare = 0;

            etalWords = Strings.Split(etalon, " ");
            objWords = Strings.Split(obj, " ");
            for (int ei = 0; ei < etalWords.Count(); ei++)
            {
                FindBestComparison(etalWords[ei], objWords.ToList(), out bestCompare, out maxCompare);
                sumCompare += maxCompare;
            }
            return sumCompare / etalWords.Count();
        }

        public void FindBestComparison(string tested, List<string> searched, out string bestCompare, out int maxCompare)
        {
            bestCompare = "";
            maxCompare = 0; //= db.Products.Max(mp => comp.FuzzyStringCompare_2side(mp.titleRus, g.title));
            //bestCompare = db.Products.Select(p => p.titleRus).FirstOrDefault(tr => comp.FuzzyStringCompare_2side(tr, g.title) == maxCompare);

            foreach (string prodTitle in searched)
            {
                string fixTitle = prodTitle.Replace("Dead Sea Cosmetics", "Косметика Мертвого Моря").
                    Replace("Dead Sea", "Мертвого Моря");
                int compare = FuzzyStringCompare_2side(fixTitle, tested);
                if (maxCompare < compare)
                {
                    maxCompare = compare;
                    bestCompare = prodTitle;
                    Console.WriteLine("Up: {0} = ({1} =??= {2})", maxCompare, prodTitle, tested);
                }
            }
        }
    }

    //=======================================================
    //Service provided by Telerik (www.telerik.com)
    //Conversion powered by NRefactory.
    //Twitter: @telerik
    //Facebook: facebook.com/telerik
    //=======================================================
}

﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using Microsoft.Win32;

using DeadSeaCatalogueDAL;

namespace DeadSeaTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string iniFileNameImportUrls = "PostUrlImportPaths.ini";
        const string iniFileNameExportYM = "YaMarketExportPaths.ini";
        const string iniFileNameImportRoboted2IK = "ImportRobotedIKToDB.ini";
        const string iniFileNameImportKontrakt = "KontraktCatalogPaths.ini";


        public MainWindow()
        {
            InitializeComponent();
        }


        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void WritePathToIni(string pathToCSV, string pathToIni)
        {
            using (StreamWriter sw = new StreamWriter(pathToIni, append: true))
            {
                sw.WriteLine(pathToCSV);
            }
        }
        private void ReadPathFromIni(System.Windows.Controls.ComboBox cb, string pathToIni)
        {            
            if(File.Exists(pathToIni))
            using (StreamReader sr = new StreamReader(pathToIni))
            {
                while (!sr.EndOfStream)
                {
                    cb.Items.Add(sr.ReadLine());
                }
            }
            if (cb.Items.Count > 0)
                cb.Text = cb.Items[cb.Items.Count - 1].ToString();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                cbPath.Text = openFileDialog.FileName;
                WritePathToIni(cbPath.Text, iniFileNameImportUrls);
            }
        }

        /// <summary>
        /// Импорт ссылок на товары из файла ЦВС
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            string urlFilePath = cbPath.Text;

            using (ProductContext db = new ProductContext())
            {
                using (StreamReader sr = new StreamReader(urlFilePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] cells = line.Split(';');
                        string url = cells[4];
                        string sku = cells[3];
                        string image = cells[5];
                        lbLogUrls.Items.Insert(0, url);
                        Product p = db.Products.FirstOrDefault(x => x.artikul == sku);
                        if (p != null)
                        {
                            p.wpUrl = //"http://izrael-cosmetics.ru/product/" + 
                                url.Replace("http://израильскаякосметика.рф", "http://izrael-cosmetics.ru");
                            p.wpImageUrl = image;
                        }
                    }
                }
                db.SaveChanges();
            }            
        }

        private void TabItemURLImport_Initialized(object sender, EventArgs e)
        {
            ReadPathFromIni(cbPath, iniFileNameImportUrls);
        }

        private void TabItemExportYM_Initialized(object sender, EventArgs e)
        {
            ReadPathFromIni(cbExportYMFile, iniFileNameExportYM);
        }

        private void bBrowseExportFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = false;
            openFileDialog.AddExtension = true;
            
            if (openFileDialog.ShowDialog() == true)
            {
                cbExportYMFile.Text = openFileDialog.FileName;
                WritePathToIni(cbExportYMFile.Text, iniFileNameExportYM);
            }
        }

        private void bExport_Click(object sender, RoutedEventArgs e)
        {
            string eymFilePath = cbExportYMFile.Text;

            using (ProductContext db = new ProductContext())
            {
                using (StreamWriter sw = new StreamWriter(eymFilePath))
                {
                    sw.WriteLine("id;available;url;price;currencyId;category;picture;name;description");

                    foreach (Product p in db.Products.Where(x => x.wpUrl != null && x.wpImageUrl != null))
                    {   
                        Translation t = db.Translations.FirstOrDefault(tr => tr.titleEng == p.title);
                        if (t != null)
                        {
                            Category cat = p.OurCategory(db);

#warning Нужно проверить товары без нашей категории!!! И товары без урла :(

                            if(cat != null)
                                sw.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                            p.artikul, "true", p.wpUrl, p.numericPriceRub, "RUR",
                            cat.titleRus.Replace(";", ",").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""),
                            p.wpImageUrl, t.title.Replace(";", ",").Replace("\r\n", "").Replace("\n", "").Replace("\r", ""), 
                            t.desc.Replace(";", ",").Replace("\r\n", "").Replace("\n", "").Replace("\r", "")
                            //"-"
                            );
                        }
                    }
                }
            }
        }

        private void bBrowseIKRobot_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();            

            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cbRobotPath.Text = browser.SelectedPath; // prints path
                WritePathToIni(cbRobotPath.Text, iniFileNameImportRoboted2IK);
            }
        }

        /// <summary>
        /// экспорт товаров из выкачанной роботом инфы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bExport2DB_Click(object sender, RoutedEventArgs e)
        {

            WritePathToIni(cbRobotPath.Text, iniFileNameImportRoboted2IK);
            string robotPath = cbRobotPath.Text;
            //string[] fileNames = {"" };

            using (ProductContext db = new ProductContext())
            {
                DirectoryInfo dir = new DirectoryInfo(robotPath);
                string currTitle = "";
                string currSku = "";
                int totalCount = dir.GetDirectories().Count();
                int okCount = 0;
                foreach (DirectoryInfo skuDir in dir.GetDirectories())
                {
                    try {
                        string skuDirPath = skuDir.FullName;
                        string sku = skuDir.Name;
                        currSku = sku;
                        string categories = File.ReadAllText(skuDirPath + "\\categories");
                        string title = File.ReadAllText(skuDirPath + "\\name");
                        currTitle = title;
                        string description = File.ReadAllText(skuDirPath + "\\description");
                        string details = File.ReadAllText(skuDirPath + "\\details");
                        string fullprice = File.ReadAllText(skuDirPath + "\\fullprice");
                        string ourPriceFileName = skuDirPath + "\\ourprice";
                        
                        // опт.цена реально с сайта или будет экстраполяция?
                        bool IsPriceExtrapolated = true;
                        string ourprice = fullprice;
                        if (File.Exists(ourPriceFileName))
                        {
                            IsPriceExtrapolated = false;
                            ourprice = File.ReadAllText(ourPriceFileName);
                        }

                        string imageFileName = skuDir.GetFiles().FirstOrDefault(file => new string[] { ".jpg", ".png", ".gif" }.Contains(file.Extension)).Name;
                        lbLogRobot.Items.Add(string.Format("Adding {0} with sku {1} and prices {2}\\{3} to category {4}",
                            title, sku, ourprice, fullprice, categories));

                        //string category = categories.Split(';;')[0];
                        // получили данные о товаре, заводим или сохраняем
                        foreach(string category in categories.Split(new string[] { ";;" }, StringSplitOptions.None))
                            ProductContext.SaveProduct(sku, category, title, ourprice, fullprice, description, details, imageFileName);
                        okCount++;
                    }
                    catch(Exception ex)
                    {
                        lbLogRobot.Items.Add(string.Format("Error when adding {1}\\{0} :", currTitle, currSku, ex.ToString()));
                    }
                }
                lbLogRobot.Items.Add(string.Format("Products added: {0} of {1}", okCount, totalCount ));
            }
        }

        private void TabItemRobot_Initialized(object sender, EventArgs e)
        {
            ReadPathFromIni(cbRobotPath, iniFileNameImportRoboted2IK);
        }

        private void bBrowseKontrakt_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();

            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                cbKontraktPath.Text = browser.SelectedPath; // prints path
                WritePathToIni(cbKontraktPath.Text, iniFileNameImportRoboted2IK);
            }
        }

        private void bImportKontrakt_Click(object sender, RoutedEventArgs e)
        {

            WritePathToIni(cbKontraktPath.Text, iniFileNameImportKontrakt);
            string kontraktPath = cbKontraktPath.Text;
            //string[] fileNames = {"" };

            using (ProductContext db = new ProductContext())
            {
                DirectoryInfo dir = new DirectoryInfo(kontraktPath);
                string currTitle = "";
                string currSku = "";
                int totalCount = dir.GetDirectories().Count();
                int okCount = 0;
                foreach (FileInfo csv in dir.GetFiles())
                {
                    StreamReader priceFile = File.OpenText(csv.FullName);
                    bool startedData = false;

                    string categories = "";

                    while (!priceFile.EndOfStream)
                        try
                        {
                            string line = priceFile.ReadLine();
                            if (line.Contains("Наименование"))
                                startedData = true;
                            if (!startedData)
                                continue;
                            string[] cells = line.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            if (cells.Count() == 2)
                            {
                                categories = cells[1] + ";;" + cells[2];
                                continue;
                            }

                            string skuDirPath;
                            string sku = cells[3];
                            currSku = sku;
                            string title = cells[2];
                            currTitle = title;
                            string barcode = cells[6];
                            string description = cells[10];
                            string details = cells[11];
                            string fullprice = cells[8];
                            string ourprice = cells[9];

                            bool IsPriceExtrapolated = false;

                            string imageFileName = sku + ".jpg";
                            textBlockKontraktLog.Text += (string.Format("Adding {0} with sku {1} and prices {2}\\{3} to category {4}",
                                title, sku, ourprice, fullprice, categories));

                            //string category = categories.Split(';;')[0];
                            // получили данные о товаре, заводим или сохраняем
                            foreach (string category in categories.Split(new string[] { ";;" }, StringSplitOptions.None))
                                ProductContext.SaveProduct(sku, category, title, ourprice, fullprice, description, details, imageFileName);
                            okCount++;
                        }
                        catch (Exception ex)
                        {
                            textBlockKontraktLog.Text += string.Format("Error when adding {1}\\{0} :", currTitle, currSku, ex.ToString()) + "\n";
                        }
                }
                lbLogRobot.Items.Add(string.Format("Products added: {0} of {1}", okCount, totalCount));
            }
        }

        private void TabItemKontrakt_Initialized(object sender, EventArgs e)
        {
            ReadPathFromIni(cbKontraktPath, iniFileNameImportKontrakt);
        }
    }
}

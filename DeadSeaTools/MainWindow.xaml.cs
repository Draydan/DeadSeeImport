using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private void ReadPathFromIni(ComboBox cb, string pathToIni)
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
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
                        lbLog.Items.Insert(0, url);
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

        }
    }
}

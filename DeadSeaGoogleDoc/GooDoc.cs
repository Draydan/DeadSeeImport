using Google.Apis.Auth.OAuth2;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.Spreadsheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DeadSeaCatalogueDAL;


namespace DeadSeaGoogleDoc
{
    class GooDoc
    {
        static void Main(string[] args)
        {
            //SpreadSheetRead();
            SpreadSheetWork();
            Console.WriteLine("enter");
            Console.ReadLine();
        }

        public static void SpreadSheetRead()
        {
            //Authenticate:

            SpreadsheetsService myService = new SpreadsheetsService(null);
            myService.setUserCredentials("yokolnov.logstream", "klamsi81");
            //Get a list of spreadsheets:

            SpreadsheetQuery query = new SpreadsheetQuery();
            query.Uri = new Uri(@"https://docs.google.com/spreadsheets/d/19qX8B8skvYMRcmgMQhsek2ZHi1fdiyf1LC7wVm7D4L4/edit#gid=0");
            SpreadsheetFeed feed = myService.Query(query);

            Console.WriteLine("Your spreadsheets: ");
            foreach (SpreadsheetEntry entry in feed.Entries)
            {
                Console.WriteLine(entry.Title.Text);

                //Given a SpreadsheetEntry you've already retrieved, you can get a list of all worksheets in this spreadsheet as follows:

                AtomLink link = entry.Links.FindService(GDataSpreadsheetsNameTable.WorksheetRel, null);

                WorksheetQuery wsquery = new WorksheetQuery(link.HRef.ToString());
                WorksheetFeed wsfeed = myService.Query(wsquery);

                foreach (WorksheetEntry worksheet in wsfeed.Entries)
                {
                    Console.WriteLine(worksheet.Title.Text);

                    //And get a cell based feed:
                    AtomLink cellFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

                    CellQuery cquery = new CellQuery(cellFeedLink.HRef.ToString());
                    CellFeed cfeed = myService.Query(cquery);

                    Console.WriteLine("Cells in this worksheet:");
                    foreach (CellEntry curCell in feed.Entries)
                    {
                        Console.WriteLine("Row {0}, column {1}: {2}", curCell.Cell.Row,
                            curCell.Cell.Column, curCell.Cell.Value);
                    }
                }
            }
        }
        public static void SpreadSheetWork()
        {
            var certificate = new X509Certificate2(@"GoogleDocAccessProject-b63329148447.p12", "notasecret", X509KeyStorageFlags.Exportable);

            const string user = "yokolnov-goo-doc-1@appspot.gserviceaccount.com";

            var serviceAccountCredentialInitializer = new ServiceAccountCredential.Initializer(user)
            {
                Scopes = new[] { "https://spreadsheets.google.com/feeds" }
            }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                throw new InvalidOperationException("Access token request failed.");

            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + credential.Token.AccessToken);

            var service = new SpreadsheetsService("GoogleDocAccessApp") { RequestFactory = requestFactory };

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            if (feed.Entries.Count == 0)
            {
                Console.WriteLine("There are no sheets");
            }

            // Iterate through all of the spreadsheets returned
            foreach (SpreadsheetEntry sheet in feed.Entries)
            {
                // Print the title of this spreadsheet to the screen
                Console.WriteLine(sheet.Title.Text);

                // Make a request to the API to fetch information about all
                // worksheets in the spreadsheet.
                WorksheetFeed wsFeed = sheet.Worksheets;

                // Iterate through each worksheet in the spreadsheet.
                foreach (WorksheetEntry entry in wsFeed.Entries)
                {
                    // Get the worksheet's title, row count, and column count.
                    string title = entry.Title.Text;
                    var rowCount = entry.Rows;
                    var colCount = entry.Cols;
                    #region "перевод выбранного"
                    if (title.Contains("перевод выбранного"))
                    {
                        // Print the fetched information to the screen for this worksheet.
                        Console.WriteLine(title + "- rows:" + rowCount + " cols: " + colCount);

                        //And get a cell based feed:
                        AtomLink cellFeedLink = entry.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

                        CellQuery cquery = new CellQuery(cellFeedLink.HRef.ToString());
                        CellFeed cfeed = service.Query(cquery);

                        Console.WriteLine("Cells in this worksheet:");
                        string[,] cells = new string[rowCount, colCount];
                        foreach (CellEntry curCell in cfeed.Entries)
                        //for (int ri = 0; ri < rowCount; ri++)
                        //    for (int ci = 0; ci < colCount; ci++)
                            {
                                //CellEntry curCell = cfeed.Entries.FirstOrDefault(x => x.);
                                //Console.WriteLine("Row {0}, column {1}: {2}", curCell.Cell.Row, curCell.Cell.Column, curCell.Cell.Value);
                                cells[curCell.Cell.Row - 1 , curCell.Cell.Column - 1] = curCell.Cell.Value;
                            }

                        using (var db = new ProductContext())
                        {
                            for (int ri = 0; ri < rowCount; ri++)
                                //for (int ci = 0; ci < colCount; ci++)
                                if( ! string.IsNullOrEmpty(cells[ri, 0])
                                    && !string.IsNullOrEmpty(cells[ri, 1])
                                    && !string.IsNullOrEmpty(cells[ri, 2])
                                    && !string.IsNullOrEmpty(cells[ri, 3]))
                                {
                                    string titleEng = cells[ri, 1];
                                    if (db.Translations.Any(t => t.titleEng == titleEng))
                                        db.Translations.RemoveRange(db.Translations.Where(t => t.titleEng == titleEng));
                                    db.Translations.Add(new Translation
                                    {
                                        titleEng = cells[ri, 1],
                                        title = cells[ri, 2],
                                        desc = cells[ri, 3]
                                    });
                            Console.WriteLine("added {0}" , cells[ri, 1]);
                                }
                            db.SaveChanges();
                        }
                        
                        // Create a local representation of the new worksheet.
                        /*
                        WorksheetEntry worksheet = new WorksheetEntry();
                        worksheet.Title.Text = "New Worksheet";
                        worksheet.Cols = 10;
                        worksheet.Rows = 20;

                        // Send the local representation of the worksheet to the API for
                        // creation.  The URL to use here is the worksheet feed URL of our
                        // spreadsheet.
                        WorksheetFeed NewwsFeed = sheet.Worksheets;
                        service.Insert(NewwsFeed, worksheet);
                        */
                    }
                    #endregion

                    #region категории
                    if (title.Contains("Категории"))
                    {
                        // Print the fetched information to the screen for this worksheet.
                        Console.WriteLine(title + "- rows:" + rowCount + " cols: " + colCount);

                        //And get a cell based feed:
                        AtomLink cellFeedLink = entry.Links.FindService(GDataSpreadsheetsNameTable.CellRel, null);

                        CellQuery cquery = new CellQuery(cellFeedLink.HRef.ToString());
                        CellFeed cfeed = service.Query(cquery);

                        Console.WriteLine("Cells in this worksheet:");
                        string[,] cells = new string[rowCount, colCount];
                        foreach (CellEntry curCell in cfeed.Entries)
                        //for (int ri = 0; ri < rowCount; ri++)
                        //    for (int ci = 0; ci < colCount; ci++)
                        {
                            //CellEntry curCell = cfeed.Entries.FirstOrDefault(x => x.);
                            Console.WriteLine("Row {0}, column {1}: {2}", curCell.Cell.Row, curCell.Cell.Column, curCell.Cell.Value);
                            cells[curCell.Cell.Row - 1, curCell.Cell.Column - 1] = curCell.Cell.Value;
                        }

                        using (var db = new ProductContext())
                        {
                            for (int ri = 0; ri < rowCount; ri++)
                                //for (int ci = 0; ci < colCount; ci++)
                                if (!string.IsNullOrEmpty(cells[ri, 0])
                                    && !string.IsNullOrEmpty(cells[ri, 1])
                                    && !string.IsNullOrEmpty(cells[ri, 2])
                                    )
                                {
                                    string titleEng = cells[ri, 0];
                                    if (db.Translations.Any(t => t.titleEng == titleEng))
                                        db.Translations.RemoveRange(db.Translations.Where(t => t.titleEng == titleEng));
                                    db.Translations.Add(new Translation
                                    {
                                        titleEng = cells[ri, 0],
                                        title = cells[ri, 1],
                                        isOurCategory = (cells[ri, 2] == "Наша категория" || cells[ri, 2] == "Наша категория 2") ,
                                        keyWords = cells[ri, 3],
                                        antiKeyWords = cells[ri, 4]
                                    });
                                    Console.WriteLine("added {0}", cells[ri, 1]);
                                }
                            db.SaveChanges();
                        }
                    }
                    #endregion 
                }
            }            
        }
    }
}

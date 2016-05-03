using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

namespace DeadSeaVKExport
{
    public class MarketEntity
    {
        public long ID;
        public string Title;
        public bool isUpdated;
        public long? PhotoID;

        public MarketEntity(long id, string _title, long? photoId, bool isupdated = false)
        {
            ID = id;
            Title = _title;
            PhotoID = photoId;
            isUpdated = isUpdated;
        }
    }

    public class ProductVKExporter
    {
        private const float kursBaksa = 65;

        private VkApi vk;
        private long GroupID;
        private string imageDir = @"e:\Work\DeadSeaCosmeticsImport\DeadSeaCosmeticsImport\bin\Debug\results\images\";

        List<MarketEntity> AlbumList;
        List<MarketEntity> ProductList;

        public ProductVKExporter()
        {
            vk = Auth();

            var dsg = vk.Utils.ResolveScreenName("izrael_cosmetics");
            //Console.WriteLine(dsg.Id.Value);
            GroupID = dsg.Id.Value;

            LoadAlbumIDs();
            LoadProductsIDs();
        }

        void CleanPrevGoods()
        {
            throw new NotImplementedException();
        }

        void LoadAlbumIDs()
        {
            AlbumList = new List<MarketEntity>();
            var albums = vk.Markets.GetAlbums(-GroupID);
            foreach (var a in albums)
            {
                AlbumList.Add(new MarketEntity(a.Id.Value, a.Title,
                    (a.Photo != null) ? (a.Photo.Id.Value) : (0)));
                Console.WriteLine(a.Title);
            }
        }

        void LoadProductsIDs()
        {
            ProductList = new List<MarketEntity>();
            var goods = vk.Markets.Get(-GroupID);
            foreach (var a in goods)
            {                
                ProductList.Add(new MarketEntity(a.Id.Value, a.Title, 
                    (a.Photos.Count>0)?(a.Photos[0].Id.Value):(0)));
                Console.WriteLine(a.Title);
            }
        }

        public string GetImageFilePath(string imageFileName)
        {
            return imageDir + imageFileName;
        }

        public void AddProductToAlbum(string titleProduct, long ProductID, string titleAlbum, string imageFileName)
        {
            string imageFilePath = GetImageFilePath(imageFileName);
            //if (AlbumList.Where(x => x.Title == titleAlbum).Count() >= 2)
            
            // если в этот заход подборку еще не апдейтили, то удаляем все ее копии
            foreach (var alb in AlbumList.Where(x => x.Title == titleAlbum && !x.isUpdated))
            {
                Console.WriteLine("удаляем альбом {0} {1}", alb.ID, alb.Title);
                vk.Markets.DeleteAlbum(-GroupID, alb.ID);
            }
            AlbumList.RemoveAll(x => x.Title == titleAlbum && !x.isUpdated);
            //AlbumList = new List<MarketEntity>();
            //long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
            //vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });

            //else
            // если подборки еще нету, заводим
            if (AlbumList.Where(x => x.Title == titleAlbum).Count() == 0)
            {
                long photoID = UploadImage(imageFilePath);
                long AlbumID = vk.Markets.AddAlbum(-GroupID, titleAlbum, photoID);
                Console.WriteLine("добавляем в заведенный альбом {0} {1}", AlbumID, titleAlbum);                
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
                MarketEntity alb = new MarketEntity(AlbumID, titleAlbum, photoID);
                alb.isUpdated = true;
                AlbumList.Add(alb);
            }
            else
            // если подборка уже есть, заводим в нее товар
            {
                long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
                Console.WriteLine("добавляем в готовый альбом {0} {1}", AlbumID, titleAlbum);
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
            }
        }

        private decimal ConverPrice(string sprice)
        {
            decimal rez = (decimal)Math.Round(float.Parse(sprice.Replace("$", "").Replace(".", ",")) * kursBaksa, 0);
            return rez;
        }

        public long ExportProduct(string title, string desc, /*string titleCategory, */ string sprice, string imageFileName)
        {
            string imageFilePath = GetImageFilePath(imageFileName);
            if (desc.Length <= 10)
                desc = string.Format("This is Sparta! And also {0} for a pidgy pipl price of {1}", title, ConverPrice(sprice));

            int prodCount = ProductList.Where(x => x.Title == title).Count();
            //если есть 1 копия то редактируем ее
            //если более 1 то удаляем все копии этого товара
            if(prodCount > 1)
                foreach (var p in ProductList.Where(x => x.Title == title))
                    vk.Markets.Delete(-GroupID, p.ID);

            if (prodCount == 0)
            {
                long photoID = UploadImage(imageFilePath);
                long ProdID = vk.Markets.Add(new MarketProductParams
                {
                    OwnerId = -GroupID,
                    CategoryId = 702,
                    MainPhotoId = photoID,
                    Deleted = false,
                    Name = title,
                    Description = desc,
                    Price = ConverPrice(sprice)
                });
                //AddProductToAlbum(title, ProdID, titleCategory, imageFilePath);
                ProductList.Add(new MarketEntity(ProdID, title, photoID, true));
                return ProdID;
            }
            if (prodCount == 1)
            {
                long ProdID = ProductList.First(x => x.Title == title).ID;
                long? mainPhotoID = ProductList.First(x => x.Title == title).PhotoID;
                if (mainPhotoID == 0)
                    mainPhotoID = UploadImage(imageFilePath);
                vk.Markets.Edit(new MarketProductParams
                {
                    OwnerId = -GroupID,
                    ItemId = ProdID,
                    MainPhotoId = (long)mainPhotoID,
                    CategoryId = 702,
                    Deleted = false,
                    Name = title,
                    Description = desc,
                    Price = ConverPrice(sprice)
                });
                //AddProductToAlbum(title, ProdID, titleCategory, imageFilePath);
                return ProdID;
            }
            return 0;
        }

        private long UploadImage(string imageFilePath)
        {
            // Получить адрес сервера для загрузки.
            var uploadServer = vk.Photo.GetMarketUploadServer(GroupID, true);
            // Загрузить фотографию.
            var wc = new WebClient();
            //Console.WriteLine("uploadServer.UploadUrl=" + uploadServer.UploadUrl);
            Console.WriteLine("uploading {0}", imageFilePath);
            var responseImg = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, imageFilePath));
            // Сохранить загруженную фотографию
            var photo = vk.Photo.SaveMarketPhoto(GroupID, responseImg);
            return photo.FirstOrDefault().Id.Value;
        }

        VkApi Auth()
        {
            #region settings
            ulong appID = 5432233;                      // ID приложения
            string email = "yuokol@yandex.ru";         // email или телефон
            Console.WriteLine("VK password:");            
            string pass = Console.ReadLine();               // пароль для авторизации
            //for (int si = 0; si < 6; si++)                 pass += Console.ReadKey();
            Console.WriteLine("checking...");
            Settings scope = Settings.All;      // Приложение имеет доступ к маркету и всему

            #endregion

            for (int li = 0; li <= 100; li++)
                Console.WriteLine();

            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = appID,
                Login = email,
                Password = pass,
                Settings = scope
            });

            return vk;
        }

    }
}


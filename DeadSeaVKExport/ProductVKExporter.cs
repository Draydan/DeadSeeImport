using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.Model;

using Logger;
using System.Collections.ObjectModel;
using System.Threading;

namespace DeadSeaVKExport
{
    public class MarketEntity
    {
        public long ID;
        public string Title;
        public bool isUpdated;
        public long? PhotoID;
        public string Description;

        public MarketEntity(long id, string _title, string desc, long? photoId, bool isupdated = false)
        {
            ID = id;
            Title = _title;
            PhotoID = photoId;
            isUpdated = isupdated;
            Description = desc;
        }
    }

    public class ProductVKExporter
    {
        private const float kursBaksa = 60;
        private const int sleepTimeMS = 400;

        public const string mainAlbumTitle = "Переведенные";

        private VkApi vk;
        private long GroupID;
        private string imageDir = @"e:\Work\DeadSeaCosmeticsImport\DeadSeaCosmeticsImport\bin\Debug\results\images\";
        private string imageTooSmallDir = @"e:\Work\DeadSeaCosmeticsImport\DeadSeaCosmeticsImport\bin\Debug\results\images\toosmall\";

        public List<MarketEntity> AlbumList;
        public List<MarketEntity> ProductList;

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

        public List<Market> GetAllGoods(long? albumId = null)
        {
            const int goodsCountStep = 200;
            //ReadOnlyCollection<VkNet.Model.Market> allGoods = new ReadOnlyCollection<VkNet.Model.Market>();
            List<Market> allGoods = new List<Market>();
            //var goods = vk.Markets.Get(-GroupID, null, goodsCountStep);
            //allGoods.AddRange(goods);
            
            int offset = goodsCountStep;
            //for(int gi=goodsCountStep; goods.Count == goodsCountStep; gi += goodsCountStep)
            bool countCheck = true;
            for (int gi = 0; countCheck; gi += goodsCountStep)
            {
                Thread.Sleep(400);
                var goods = vk.Markets.Get(-GroupID, albumId, goodsCountStep, gi);
                allGoods.AddRange(goods);
                countCheck = (goods.Count == goodsCountStep);
            }
            return allGoods;
        }

        public void RemoveAlbum(long albumId)
        {
            vk.Markets.DeleteAlbum(-GroupID, albumId);
        }

        //public int GetAlbum(long albumId)
        //{
        //    return vk.Markets.GetAlbumById(-GroupID, new List<long>(  albumId));            
        //}

        void LoadAlbumIDs()
        {
            AlbumList = new List<MarketEntity>();
            var albums = vk.Markets.GetAlbums(-GroupID);
            foreach (var a in albums)
            {
                AlbumList.Add(new MarketEntity(a.Id.Value, a.Title, "",
                    (a.Photo != null) ? (a.Photo.Id.Value) : (0))                    
                    );
                Console.WriteLine(a.Title);
            }
        }

        void LoadProductsIDs()
        {
            ProductList = new List<MarketEntity>();
            var goods = GetAllGoods();
            foreach (var a in goods)
            {                
                ProductList.Add(new MarketEntity(a.Id.Value, a.Title, a.Description,
                    (a.Photos.Count>0)?(a.Photos[0].Id.Value):(0))                    
                    );
                Console.WriteLine("{0} : {1}", a.Title, 
                    ProductList.Where(x => x.Title == a.Title).Count());
            }
        }

        public string GetImageFilePath(string imageFileName)
        {
            return imageDir + imageFileName;
        }

        public string GetTooSmallImageFilePath(string imageFileName)
        {
            return imageTooSmallDir + imageFileName;
        }
        public void AddProductToAlbum(string titleProduct, long ProductID, string titleAlbum, string imageFileName)
        {
            string imageFilePath = GetImageFilePath(imageFileName);
            //if (AlbumList.Where(x => x.Title == titleAlbum).Count() >= 2)
            
            // если в этот заход подборку еще не апдейтили, то удаляем все ее копии

            /* // отменяем апокалипсис
            foreach (var alb in AlbumList.Where(x => x.Title == titleAlbum && !x.isUpdated))
            {
                Logger.Logger.SuccessLog("удаляем альбом {0} {1}", alb.ID, alb.Title);
                vk.Markets.DeleteAlbum(-GroupID, alb.ID);
            }
            AlbumList.RemoveAll(x => x.Title == titleAlbum && !x.isUpdated);
            */

            //AlbumList = new List<MarketEntity>();
            //long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
            //vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });

            //else
            // если подборки еще нету, заводим
            if (AlbumList.Where(x => x.Title == titleAlbum).Count() == 0)
            {
                long photoID = UploadImage(imageFilePath);
                bool isMainAlbum = (titleAlbum == mainAlbumTitle);
                if(isMainAlbum)
                {
                }
                long AlbumID = vk.Markets.AddAlbum(-GroupID, titleAlbum, photoID, isMainAlbum);
                Logger.Logger.SuccessLog("Добавлен альбом {0}", titleAlbum);
                Logger.Logger.SuccessLog("добавляем в заведенный альбом {0} {1}", AlbumID, titleAlbum);                
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
                MarketEntity alb = new MarketEntity(AlbumID, titleAlbum, "", photoID);
                alb.isUpdated = true;
                AlbumList.Add(alb);
            }
            else
            // если подборка уже есть, заводим в нее товар
            {
                long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
                Logger.Logger.SuccessLog("добавляем в готовый альбом {0} {1}", AlbumID, titleAlbum);
            }
        }

        private decimal ConverPrice(string sprice)
        {
            decimal rez = (decimal)Math.Round(float.Parse(sprice.Replace("$", "").Replace(".", ",")) * kursBaksa, 0);
            return rez;
        }
        public void DeleteProduct(long pId)
        {
            vk.Markets.Delete(-GroupID, pId);
            Thread.Sleep(sleepTimeMS);
        }
        public long ExportProduct(string title, string desc, /*string titleCategory, */ string sprice, string imageFileName)
        {
            Console.WriteLine("обрабатываем товар {0}", title);
            string imageFilePath = GetImageFilePath(imageFileName);
            if (desc.Length <= 10)
                desc = string.Format("This is Sparta! And also {0} for a pidgy pipl price of {1}", title, ConverPrice(sprice));
            string titleFixed = title.Replace("&", " and ").Replace("  ", " ");

            int prodCount = ProductList.Where(x => x.Title == title).Count();
            //если есть 1 копия то редактируем ее
            //если более 1 то удаляем все копии этого товара
            if (prodCount > 1)
            {
                Console.WriteLine("удаляем все {0} копий", prodCount);
                foreach (var p in ProductList.Where(x => x.Title == title))
                    vk.Markets.Delete(-GroupID, p.ID);
                ProductList.RemoveAll(x => x.Title == title);
                prodCount = 0;
            }
            if (prodCount == 0)
            {
                Console.WriteLine("добавляем");
                long photoID = UploadImage(imageFilePath);
                long ProdID = vk.Markets.Add(new MarketProductParams
                {
                    OwnerId = -GroupID,
                    CategoryId = 702,
                    MainPhotoId = photoID,
                    Deleted = false,
                    Name = titleFixed,
                    Description = desc,
                    Price = ConverPrice(sprice)
                });
                //AddProductToAlbum(title, ProdID, titleCategory, imageFilePath);
                ProductList.Add(new MarketEntity(ProdID, title, desc, photoID, true));
                Logger.Logger.SuccessLog("Добавлен товар {0}", title);
                return ProdID;
            }
            if (prodCount == 1)
            {
                Console.WriteLine("редактируем: {0} // {1}", title, desc);
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
                Logger.Logger.SuccessLog("Изменен товар {0}", title);
                return ProdID;
            }
            Logger.Logger.ErrorLog("Возвращен 0 для товара {0}", title);
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


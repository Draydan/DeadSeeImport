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

        public MarketEntity(long id, string _title)
        {
            ID = id;
            Title = _title;
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
                AlbumList.Add(new MarketEntity(a.Id.Value, a.Title));
                Console.WriteLine(a.Title);
            }
        }

        void LoadProductsIDs()
        {
            ProductList = new List<MarketEntity>();
            var goods = vk.Markets.Get(-GroupID);
            foreach (var a in goods)
            {
                ProductList.Add(new MarketEntity(a.Id.Value, a.Title));
                Console.WriteLine(a.Title);
            }
        }
        public void AddProductToAlbum(string titleProduct, long ProductID, string titleAlbum, long ImageID)
        {
            if (AlbumList.Where(x => x.Title == titleAlbum).Count() >= 2)
            {
                foreach (var alb in AlbumList.Where(x => x.Title == titleAlbum))
                {
                    vk.Markets.DeleteAlbum(-GroupID, alb.ID);
                }
                //long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
                //vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
            }
            //else
            {
                long AlbumID = vk.Markets.AddAlbum(-GroupID, titleAlbum, ImageID);
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
                AlbumList.Add(new MarketEntity(AlbumID, titleAlbum));
            }
        }

        private decimal ConverPrice(string sprice)
        {
            decimal rez = (decimal)Math.Round(float.Parse(sprice.Replace("$", "").Replace(".", ",")) * kursBaksa, 0);
            return rez;
        }

        public long ExportProduct(string title, string desc, string titleCategory, string sprice, string imageFileName)
        {
            // удаляем все копии этого товара
            //foreach (var p in ProductList.Where(x => x.Title == title))                 vk.Markets.Delete(-GroupID, p.ID);

            // Получить адрес сервера для загрузки.
            var uploadServer = vk.Photo.GetMarketUploadServer(GroupID, true);
            // Загрузить фотографию.
            var wc = new WebClient();
            Console.WriteLine("uploadServer.UploadUrl=" + uploadServer.UploadUrl);
            string imageFilePath = imageDir + imageFileName;
            Console.WriteLine("uploading {0}", imageFilePath);
            var responseImg = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, imageFilePath));
            // Сохранить загруженную фотографию
            var photo = vk.Photo.SaveMarketPhoto(GroupID, responseImg);
            long photoID = photo.FirstOrDefault().Id.Value;
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
            AddProductToAlbum(title, ProdID, titleCategory, photoID);
            return ProdID;
        }        

        VkApi Auth()
        {
            #region settings
            ulong appID = 5432233;                      // ID приложения
            string email = "yuokol@yandex.ru";         // email или телефон
            string pass = "laif81";               // пароль для авторизации
            Settings scope = Settings.All;      // Приложение имеет доступ к маркет 

            #endregion

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


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
    public class Album
    {
        public long ID;
        public string Title;

        public Album(long id, string _title)
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

        List<Album> AlbumList;

        public ProductVKExporter()
        {
            vk = Auth();

            var dsg = vk.Utils.ResolveScreenName("izrael_cosmetics");
            //Console.WriteLine(dsg.Id.Value);
            GroupID = dsg.Id.Value;

            LoadAlbumIDs();
        }

        void CleanPrevGoods()
        {
            throw new NotImplementedException();
        }

        void LoadAlbumIDs()
        {
            AlbumList = new List<Album>();
            var albums = vk.Markets.GetAlbums(-GroupID);
            foreach (var a in albums)
                AlbumList.Add(new Album(a.Id.Value, a.Title));
        }

        public void AddProductToAlbum(string titleProduct, long ProductID, string imageFilePath, string titleAlbum)
        {
            if (AlbumList.Any(x => x.Title == titleAlbum))
            {
                long AlbumID = AlbumList.First(x => x.Title == titleAlbum).ID;
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
            }
            else
            {
                long AlbumID = vk.Markets.AddAlbum(-GroupID, titleAlbum);
                vk.Markets.AddToAlbum(-GroupID, ProductID, new[] { AlbumID });
                AlbumList.Add(new Album(AlbumID, titleAlbum));
            }
        }

        private decimal ConverPrice(string sprice)
        {
            decimal rez = (decimal)Math.Round(float.Parse(sprice.Replace("$", "").Replace(".", ",")) * kursBaksa, 0);
            return rez;
        }

        public long ExportProduct(string title, string desc, string sprice, string imageFileName)
        {
            // Получить адрес сервера для загрузки.
            var uploadServer = vk.Photo.GetMarketUploadServer(GroupID, true);
            // Загрузить фотографию.
            var wc = new WebClient();
            var responseImg = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, imageDir + imageFileName));
            // Сохранить загруженную фотографию
            var photo = vk.Photo.SaveMarketPhoto(GroupID, responseImg);
            long ProdID = vk.Markets.Add(new MarketProductParams
            {
                OwnerId = -GroupID,
                CategoryId = 702,
                MainPhotoId = photo.FirstOrDefault().Id.Value,
                Deleted = false,
                Name = title,
                Description = desc,
                Price = ConverPrice(sprice)
            });
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

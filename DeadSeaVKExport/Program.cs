using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;
using VkNet.Properties;
using System.IO;

namespace DeadSeaVKExport
{
    class Program
    {
        static void Main(string[] args)
        {
            var vk = Auth();
            var param = new GroupsGetParams();
            param.Filter = GroupsFilters.Moderator;
            param.Fields = GroupsFields.All;
            //var groups = vk.Groups.Get(param);
            //foreach(var group in groups)                 Console.WriteLine("{0} {1} {2}", group.Id, group.Name, group.ScreenName);
            var dsg = vk.Utils.ResolveScreenName("izrael_cosmetics");
            Console.WriteLine(dsg.Id.Value);

            var owner = vk.Utils.ResolveScreenName("lolaok");

            var goods = vk.Markets.Get(-dsg.Id.Value);
            foreach (var g in goods) Console.WriteLine(g.Title);

            var albums = vk.Markets.GetAlbums(-dsg.Id.Value);
            foreach (var a in albums) Console.WriteLine(a.Title);

            //var cats = vk.Markets.GetCategories(1000, 0);
            //using (StreamWriter sw = new StreamWriter("categories.txt"))
            //{
            //    foreach (var cat in cats)  sw.WriteLine(cat.Id + " " + cat.Name + " " + cat.Section.Name);
            //}

            //var deadSeaGroup =
            //.First(x => x.Name.Contains("Мертвого Моря"));
            //Console.WriteLine(deadSeaGroup.Name);

            // грузим фото товара
            //vk.Photo.GetMarketUploadServer(dsg.Id.Value, )
            ExportProduct(vk, dsg.Id.Value);
            // заводим товар
            // заводим подборку 
            // добавляем в нее товар


            Console.WriteLine("Press smth");
            Console.ReadLine();
        }

        static void ExportProduct(VkApi vk, long groupId)
        {
            decimal price = (decimal)((795 * 70)/100);
            string imageDir = @"e:\Work\DeadSeaCosmeticsImport\DeadSeaCosmeticsImport\bin\Debug\results\images\";
            // Получить адрес сервера для загрузки.
            var uploadServer = vk.Photo.GetMarketUploadServer(groupId, true);
            // Загрузить фотографию.
            var wc = new WebClient();
            var responseImg = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, imageDir + @"canaan-mineral-mud-soap-dead-sea-cosmetics.gif"));
            // Сохранить загруженную фотографию
            var photo = vk.Photo.SaveMarketPhoto(groupId, responseImg);
            vk.Markets.Add(new MarketProductParams
            {
                OwnerId = -groupId,
                CategoryId = 702,
                MainPhotoId = photo.FirstOrDefault().Id.Value,
                Deleted = false,
                Name = "Canaan Mineral Mud Soap, Dead Sea Cosmetics",
                Description = "Canaan has a variety of amazing dead sea products, but who knew that plain old bar soap could be made into such a pleasant experience? Canaan's Mineral Mud Soap combines the essential's for cleaning the skin with a variety of ingredients that make the experience much more than just that.",
                Price = price
            });
        }

        static VkApi Auth()
        {
            #region settings
            ulong appID = 5432233;                      // ID приложения
            string email = "yuokol@yandex.ru";         // email или телефон
            string pass = "";               // пароль для авторизации
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

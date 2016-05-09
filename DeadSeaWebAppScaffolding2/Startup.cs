using Microsoft.Owin;
using Owin;
using DeadSeaCatalogueDAL;

[assembly: OwinStartupAttribute(typeof(DeadSeaWebAppScaffolding2.Startup))]
namespace DeadSeaWebAppScaffolding2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eMoneyApi.Startup))]
namespace eMoneyApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

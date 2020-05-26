using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WHITELABEL.Web.Startup))]
namespace WHITELABEL.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

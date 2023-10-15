using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSite7.Startup))]
namespace WebSite7
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

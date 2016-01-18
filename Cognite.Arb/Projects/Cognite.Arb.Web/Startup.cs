using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cognite.Arb.Web.Startup))]
namespace Cognite.Arb.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

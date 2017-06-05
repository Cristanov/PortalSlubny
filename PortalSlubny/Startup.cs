using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PortalSlubny.Startup))]
namespace PortalSlubny
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

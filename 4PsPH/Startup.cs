using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_4PsPH.Startup))]
namespace _4PsPH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

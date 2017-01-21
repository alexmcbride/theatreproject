using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheatreProject.Startup))]
namespace TheatreProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

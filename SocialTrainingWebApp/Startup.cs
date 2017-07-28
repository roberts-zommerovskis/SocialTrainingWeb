using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialTrainingWebApp.Startup))]
namespace SocialTrainingWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

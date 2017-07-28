using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialTrainingWEB.Startup))]
namespace SocialTrainingWEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;
using Api;

[assembly: OwinStartup(typeof(Startup))]

namespace Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}

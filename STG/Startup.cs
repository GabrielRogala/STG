using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(STG.Startup))]
namespace STG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

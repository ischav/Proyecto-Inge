using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProyectoInge1.Startup))]
namespace ProyectoInge1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

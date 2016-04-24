using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;

namespace ObiOne
{
    /// <summary>
    /// 
    public class Startup
    {
        //public static IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            //var path = appEnv.ApplicationBasePath;
            AppPath= appEnv.ApplicationBasePath;
            Configuration = new ConfigurationBuilder().AddJsonFile($"{AppPath}/appsettings.json").AddEnvironmentVariables().Build();

        }

        /// <summary>
        /// アプリケーションのルートパス
        /// </summary>
        public static string AppPath { get; set; }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(Microsoft.Framework.DependencyInjection.IServiceCollection services)
        {
            services.AddMvc();

        }
        public static IConfiguration Configuration { get; set; }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}

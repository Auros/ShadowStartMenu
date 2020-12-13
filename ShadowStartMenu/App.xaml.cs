using Serilog.Core;
using System.Windows;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ShadowStartMenu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IHostBuilder setup = CreateHostBuilder(e.Args);
            IHost host = setup.Build();

            Main main = (host.Services.GetService(typeof(Main)) as Main)!;
            main.Show();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            Initialization initializer = new Initialization();
            IHostBuilder builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices((env, services) =>
            {
                services.AddTransient<Main>();
                initializer.Configure(services);
                Logger logger = initializer.Logging(env.HostingEnvironment.ContentRootPath);
                services.AddSingleton<ILoggerFactory>(sp => new SerilogLoggerFactory(logger, true));
                logger.Information("Initializing Shadow Start Menu");
            });
            return builder;
        }
    }
}
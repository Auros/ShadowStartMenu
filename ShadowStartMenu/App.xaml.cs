using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

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
            });
            return builder;
        }
    }
}
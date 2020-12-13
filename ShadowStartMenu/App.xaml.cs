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

            Log log = (host.Services.GetService(typeof(Log)) as Log)!;
            Main main = (host.Services.GetService(typeof(Main)) as Main)!;
            main.Show();
            log.Show();

            main.Focus();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            Initialization initializer = new Initialization();
            IHostBuilder builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices((env, services) =>
            {
                services.AddSingleton<Log>();
                services.AddTransient<Main>();
                initializer.Configure(services);
                services.AddSingleton<LoggerProviderCollection>();
                services.AddSingleton<ILoggerFactory>(sp =>
                {
                    initializer.Logging(env.HostingEnvironment.ContentRootPath, sp);
                    LoggerProviderCollection loggerProviderCollection = sp.GetRequiredService<LoggerProviderCollection>();
                    SerilogLoggerFactory serilogLoggerFactory = new SerilogLoggerFactory(dispose: true, providerCollection: loggerProviderCollection);
                    foreach (var provider in sp.GetServices<ILoggerProvider>())
                    {
                        serilogLoggerFactory.AddProvider(provider);
                    }
                    return serilogLoggerFactory;
                });
            });
            return builder;
        }
    }
}
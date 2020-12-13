using System.Windows;
using ShadowStartMenu.Menu;
using Microsoft.Extensions.Logging;

namespace ShadowStartMenu
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private readonly ILogger<Main> _logger;
        private readonly IMenuSource _menuSource;

        public Main(ILogger<Main> logger, IMenuSource menuSource)
        {
            _logger = logger;
            InitializeComponent();
            _menuSource = menuSource;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _logger.LogInformation("Initializing Window");
        }
    }
}
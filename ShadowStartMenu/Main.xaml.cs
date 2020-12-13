using System.Windows;
using Microsoft.Extensions.Logging;

namespace ShadowStartMenu
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private readonly ILogger<Main> _logger;

        public Main(ILogger<Main> logger)
        {
            _logger = logger;
            InitializeComponent();
        }
    }
}
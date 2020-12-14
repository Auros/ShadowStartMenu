using System.Windows;
using System.Windows.Media;
using ShadowStartMenu.Menu;
using DIcon = System.Drawing.Icon;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace ShadowStartMenu
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private readonly ILogger<Main> _logger;
        private readonly IMenuSource _menuSource;
        private readonly ObservableCollection<AppCell> _cells = new ObservableCollection<AppCell>();

        public Main(ILogger<Main> logger, IMenuSource menuSource)
        {
            _logger = logger;
            InitializeComponent();
            _menuSource = menuSource;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _logger.LogInformation("Initializing Window");

            AppGridView.ItemsSource = _cells;

            foreach (var app in _menuSource.Apps)
            {
                _cells.Add(new AppCell(app, IconFromFilePath(app.Path)));
            }
            TitleText.Text = "...";
        }

        public static ImageSource IconFromFilePath(string filePath)
        {
            DIcon result = System.Drawing.SystemIcons.Application;
            try
            {
                result = DIcon.ExtractAssociatedIcon(filePath)!;
            }
            catch (System.Exception) { }
            return result.ToImageSource();
        }

        private record AppCell(IApp App, ImageSource Icon);

        private void AppGridView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                AppCell cell = (e.AddedItems[0] as AppCell)!;
                TitleText.Text = cell.App.Name;
            }
            else
            {
                TitleText.Text = "...";
            }
        }
    }
}
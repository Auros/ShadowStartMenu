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

        private AppCell? _activeAppCell;

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

        private static ImageSource IconFromFilePath(string filePath)
        {
            DIcon result = System.Drawing.SystemIcons.Application;
            try
            {
                result = DIcon.ExtractAssociatedIcon(filePath)!;
            }
            catch (System.Exception) { }
            return result.ToImageSource();
        }

        private void AppGridView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DeleteButton.Visibility = Visibility.Visible;
                AppCell cell = (e.AddedItems[0] as AppCell)!;
                TitleText.Text = cell.App.Name;
                _activeAppCell = cell;
            }
            else
            {
                DeleteButton.Visibility = Visibility.Hidden;
                TitleText.Text = "...";
                _activeAppCell = null;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeAppCell != null)
            {
                _menuSource.Remove(_activeAppCell.App);
                _cells.Remove(_activeAppCell);
            }
        }

        private record AppCell(IApp App, ImageSource Icon);
    }
}
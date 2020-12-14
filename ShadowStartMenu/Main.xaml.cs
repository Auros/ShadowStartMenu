using System.IO;
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

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MainGrid.Opacity = 1f;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                AppCell? cell = null;
                IApp[] newApps = new IApp[files.Length];
                for (int i = 0; i < newApps.Length; i++)
                {
                    FileInfo fileInfo = new FileInfo(files[i]);
                    var app = new UmbraMenuSource.App
                    {
                        Name = Path.GetFileNameWithoutExtension(fileInfo.Name),
                        Path = fileInfo.FullName
                    };
                    newApps[i] = app;
                    _menuSource.Add(app, ShortcutType.File, nameof(ShadowStartMenu));
                    cell = new AppCell(app, IconFromFilePath(app.Path));
                    _cells.Add(cell);
                }
                if (cell != null)
                {
                    AppGridView.SelectedValue = cell;
                }
            }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MainGrid.Opacity = 0.1f;
            }
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                MainGrid.Opacity = 1f;
            }
        }
        private record AppCell(IApp App, ImageSource Icon);
    }
}
using Serilog.Core;
using Serilog.Events;
using System.Windows;
using System.Collections.ObjectModel;

namespace ShadowStartMenu
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window, ILogEventSink
    {
        private readonly ObservableCollection<LogItem> _logItems = new ObservableCollection<LogItem>();

        public Log()
        {
            InitializeComponent();
            LogTable.ItemsSource = _logItems;
        }

        public void Emit(LogEvent logEvent)
        {
            _logItems.Add(new LogItem($"{logEvent.Properties["LooseSource"].ToString().Replace("\"", "")} {logEvent.RenderMessage()}"));
        }

        private record LogItem(string Log);
    }
}
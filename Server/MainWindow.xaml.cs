using BulletinBoard.Server;
using System.Net;
using System.Windows;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BoardServer _boardServer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _boardServer = new BoardServer(IPAddress.Parse("127.0.0.1"), 11000);
            _boardServer.Log += PrintLog;
            _boardServer.StartServer();
        }

        private void PrintLog(string message)
        {
            OutputTextBox.Dispatcher.Invoke(() => OutputTextBox.Text += message);
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _boardServer?.StopServer();
            _boardServer = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletinBoard.Server;

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

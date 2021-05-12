using System.IO;
using System.Linq;
using BulletinBoard.Server;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using BulletinBoard.Model;

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
            _boardServer?.StartServer();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _boardServer?.StopServer();
        }

        private void OutputTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            if (textBox.Text.Count(x => x == '\n') > LogLength.Value)
            {
                textBox.Text = textBox.Text.Remove(0, textBox.Text.IndexOf('\n')+1);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _boardServer = new BoardServer(IPAddress.Parse("127.0.0.1"), 11000);
            _boardServer.Log += PrintLog;
        }

        private void PrintLog(string message)
        {
            OutputTextBox.Dispatcher.Invoke(() => OutputTextBox.Text += message);
        }
    }
}
using BulletinBoard.Server;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private BoardServer server;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            server.StartServer();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void PrintLog(string message)
        {
            OutputTextBox.Dispatcher.Invoke(() => OutputTextBox.Text += message);
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (server == null)
                server = new BoardServer(IPAddress.Parse("127.0.0.1"), 11000);
            server.Log += PrintLog;
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
                textBox.Text = textBox.Text.Remove(0, textBox.Text.IndexOf('\n') + 1);
            }
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (server != null)
                server.StopServer();
        }
    }
}
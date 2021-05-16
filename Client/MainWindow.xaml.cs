using BulletinBoard.Client;
using BulletinBoard.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Client
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

        private BoardClient client;

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            client = new BoardClient(IPAddress.Parse("127.0.0.1"), 11000);
            client.StartClient();
            Task.Run(CheckConnection);
            Content = new LoginPage(client, this);
        }

        /// <summary>
        /// Background connection check
        /// </summary>
        private void CheckConnection()
        {
            while (true)
            {
                if (!client.Connected)
                    client.StartClient();
            }
        }

        /// <summary>
        /// Stopping the client after closing the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            if (client == null)
                return;

            client.StopClient();
        }
    }
}
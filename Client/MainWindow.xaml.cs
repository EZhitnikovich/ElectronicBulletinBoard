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
        private ObservableCollection<Bulletin> bulletins;

        public MainWindow()
        {
            InitializeComponent();
            bulletins = new ObservableCollection<Bulletin>();
            BulletinsList.ItemsSource = bulletins;
        }

        private BoardClient client;

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            client = new BoardClient(IPAddress.Parse("127.0.0.1"), 11000);
            client.StartClient();
            Task.Run(CheckConnection);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var bltns = GetBulletins();
            RefillBulletins(bltns);
        }

        private void RefillBulletins(List<Bulletin> bltns)
        {
            if (bltns == null)
                return;

            bulletins.Clear();

            foreach (var bulletin in bltns)
            {
                bulletins.Add(bulletin);
            }
        }

        private List<Bulletin> GetBulletins()
        {
            if (client == null)
                return null;

            return client.GetAllBulletins();
        }

        private void CheckConnection()
        {
            while (true)
            {
                if (!client.Connected)
                    client.StartClient();
            }
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            if (client == null)
                return;

            client.StopClient();
        }
    }
}
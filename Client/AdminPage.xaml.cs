using System.Collections.Generic;
using System.Collections.ObjectModel;
using BulletinBoard.Client;
using BulletinBoard.Model;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly ObservableCollection<Bulletin> _bulletins;

        protected AdminPage()
        {
            InitializeComponent();
            _bulletins = new ObservableCollection<Bulletin>();
            BulletinsList.ItemsSource = _bulletins;
        }

        private BoardClient _client;
        private Window _mainWindow;

        public AdminPage(BoardClient client, Window window) : this()
        {
            _client = client;
            _mainWindow = window;
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (TitleTextBox.Text == string.Empty)
            {
                WarningLabel.Content = "Title is empty";
                return;
            }

            if (ContentTextBox.Text == string.Empty)
            {
                WarningLabel.Content = "Content is empty";
                return;
            }

            if (ImagePathTextBox.Text == string.Empty)
            {
                WarningLabel.Content = "Image path is empty";
                return;
            }

            if (!_client.Connected)
                return;

            var bulletin = new Bulletin(ImagePathTextBox.Text, ContentTextBox.Text, TitleTextBox.Text);

            _client.AddBulletin(bulletin);
            _bulletins.Insert(0, bulletin);
            WarningLabel.Content = string.Empty;
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            _client.DeautorizeAdministrator();
            _mainWindow.Content = new LoginPage(_client, _mainWindow);
        }

        private void RefillBulletins(List<Bulletin> bltns)
        {
            if (bltns == null)
                return;

            _bulletins.Clear();

            foreach (var bulletin in bltns)
            {
                _bulletins.Add(bulletin);
            }
        }

        private List<Bulletin> GetBulletins()
        {
            if (_client == null)
                return null;

            return _client.GetAllBulletins();
        }

        private void AdminPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefillBulletins(GetBulletins());
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var bulletin = (Bulletin)((Button)sender).DataContext;

            _bulletins.Remove(bulletin);

            _client.DelBulletin(bulletin);
        }
    }
}
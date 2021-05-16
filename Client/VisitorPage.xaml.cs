using BulletinBoard.Client;
using BulletinBoard.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for VisitorPage.xaml
    /// </summary>
    public partial class VisitorPage : Page
    {
        private readonly ObservableCollection<Bulletin> _bulletins;

        protected VisitorPage()
        {
            InitializeComponent();
            _bulletins = new ObservableCollection<Bulletin>();
            BulletinsList.ItemsSource = _bulletins;
        }

        private BoardClient _client;
        private Window _mainWindow;

        public VisitorPage(BoardClient client, Window mainWindow) : this()
        {
            _client = client;
            _mainWindow = mainWindow;
        }

        private void VisitorPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            RefillBulletins(GetBulletins());
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

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.Content = new LoginPage(_client, _mainWindow);
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefillBulletins(GetBulletins());
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefillBulletins(GetBulletins().Where(x => x.Title.Contains(TitleSearchTextBox.Text) && x.Date >= PreDate.SelectedDate && x.Date <= AfterDate.SelectedDate).ToList());
        }
    }
}
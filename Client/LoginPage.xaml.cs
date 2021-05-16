using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletinBoard.Client;
using BulletinBoard.Model;

namespace Client
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private BoardClient _client;
        private Window _mainWindow;
        
        public LoginPage(BoardClient client, Window mainWindow):this()
        {
            _client = client;
            _mainWindow = mainWindow;
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_client.Connected)
            {
                WarningLabel.Content = "Client is not connected.";
                return;
            }

            if (LoginTextBox.Text == string.Empty)
            {
                WarningLabel.Content = "Login is empty";
                return;
            }

            if (PasswordTextBox.Text == string.Empty)
            {
                WarningLabel.Content = "Password is empty";
                return;
            }

            User admin = new User(LoginTextBox.Text, PasswordTextBox.Text);

            if (_client.AuthorizeAdministrator(admin))
            {
                _mainWindow.Content = new AdminPage(_client, _mainWindow);
            }
            else
            {
                WarningLabel.Content = "Not administrator";
            }
        }

        private void VisitorButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_client.Connected)
            {
                WarningLabel.Content = "Client is not connected.";
                return;
            }

            _mainWindow.Content = new VisitorPage(_client, _mainWindow);
        }
    }
}

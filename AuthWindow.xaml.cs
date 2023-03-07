using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TextNormalizer.Models;

namespace TextNormalizer
{
    public partial class AuthWindow : Window
    {
        private bool _access = false;

        public AuthWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextbox.Text;
            string password = PasswordTextbox.Password;
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Необходимо заполнить все поля!");
                return;
            }
            Crypter crypter = new();

            string encryptedLogin = crypter.Encrypt(login);
            string encryptedPassword = crypter.Encrypt(password);

            using (TextNormalizerDbContext db = new())
            {
                User? userDb = db.Users.FirstOrDefault(x => x.Login == encryptedLogin);
                if (userDb != null)
                {
                    _access = userDb.Login == encryptedLogin && userDb.Password == encryptedPassword;
                    if (_access == false)
                    {
                        MessageBox.Show("Логин и/или пароль введены неверно!");
                    }
                    else
                    {
                        Auth.CurrentUser = userDb;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином не найден!");
                    return;
                }
            }

        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_access == false)
            {
                App.Current.Shutdown();
            }
        }
    }
}

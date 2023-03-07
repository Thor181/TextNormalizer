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
using System.Security.Cryptography;
using TextNormalizer.Models;

namespace TextNormalizer
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextbox.Text;
            string password = PasswordTextbox.Password;
            bool passwordConfirm = password == PasswordConfirmTextbox.Password;
            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(password) && string.IsNullOrWhiteSpace(PasswordConfirmTextbox.Password))
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                return;
            }
            if (passwordConfirm == false)
            {
                MessageBox.Show("Введенные пароли не совпадают!");
                return;
            }
            Crypter crypter = new();

            using (TextNormalizerDbContext db = new())
            {
                if (db.Users.Any(x => x.Login == login))
                {
                    MessageBox.Show("Данный логин занят!");
                    return;
                }

                try
                {
                    db.Users.Add(new User()
                    {
                        Login = crypter.Encrypt(login),
                        Password = crypter.Decrypt(password),
                        RoleId = 2 //user
                    });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Что-то пошло не так...\n{ex.Message}");
                    return;
                }
                MessageBox.Show("Регистрация прошла успешно!");
                this.Close();
            }
        }
    }
}

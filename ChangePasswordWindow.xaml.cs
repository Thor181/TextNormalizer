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
    public partial class ChangePasswordWindow : Window
    {
        private InnerUser _currentUser;
        public ChangePasswordWindow(InnerUser user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = NewPasswordTextbox.Password;
            if (newPassword != NewPasswordConfirmTextbox.Password)
            {
                MessageBox.Show("Введенные пароли не совпадают!");
                return;
            }
            if (string.IsNullOrWhiteSpace(newPassword) 
                || string.IsNullOrWhiteSpace(NewPasswordConfirmTextbox.Password) 
                || string.IsNullOrWhiteSpace(OldPasswordBox.Password))
            {
                MessageBox.Show("Необходимо заполнить поле для пароля!");
                return;
            }
            _currentUser.Password = newPassword;
            string encryptedOldPassword = new Crypter().Encrypt(OldPasswordBox.Password);
            string encryptedNewPassword = new Crypter().Encrypt(newPassword);
            TextNormalizerDbContext db = new();
            User? editableUser = db.Users.FirstOrDefault(x => x.Id == _currentUser.Id);
            if (editableUser != null)
            {
                if (editableUser.Password != encryptedOldPassword)
                {
                    MessageBox.Show("Старый пароль введен неверно!");
                    return;
                }
                editableUser.Password = encryptedNewPassword;
                try
                {
                    db.Update(editableUser);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Что-то пошло не так...\n{ex.Message}\n{ex.InnerException}");
                    return;
                }
                MessageBox.Show("Пароль упешно изменен!");
                DialogResult = true;
                this.Close();
            }
        }
    }
}

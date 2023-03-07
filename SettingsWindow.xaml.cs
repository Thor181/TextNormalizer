using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class SettingsWindow : Window
    {
        private readonly TextNormalizerDbContext db;

        public SettingsWindow()
        {
            InitializeComponent();
            db = new();
            InitDataGrid();
        }

        private void InitDataGrid()
        {
            DataGridUsers.ItemsSource = db.Users.Select(x => new Models.InnerUser(x.Id, x.Login, x.Password, x.RoleId, x.Role.RoleName)).ToList();
        }

        private void ChangeRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать пользователя!");
                return;
            }

            InnerUser? user = DataGridUsers.SelectedItem as InnerUser;
            if (user != null)
            {
                user.RoleId = user.RoleId == 1 ? 2 : 1;
                User? editableUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
                if (editableUser != null)
                {
                    editableUser.RoleId = editableUser.RoleId == 1 ? 2 : 1;
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
                    MessageBox.Show("Роль успешно изменена!");
                    InitDataGrid();
                }
            }
        }

        private void ChangePasswordUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать пользователя!");
                return;
            }
            InnerUser? user = DataGridUsers.SelectedItem as InnerUser;
            if (user != null)
            {
                ChangePasswordWindow changePassWindow = new ChangePasswordWindow(user);
                if (changePassWindow.ShowDialog() == true)
                {
                    InitDataGrid();
                }
            } 
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUsers.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать пользователя!");
                return;
            }
            InnerUser? user = DataGridUsers.SelectedItem as InnerUser;

            if (MessageBox.Show("Вы уверены?", "Удалить пользователя", MessageBoxButton.YesNo) == MessageBoxResult.Yes && user != null)
            {
                User? deletableUser = db.Users.FirstOrDefault(x => x.Id == user.Id);
                if (deletableUser != null)
                {
                    try
                    {
                        db.Users.Remove(deletableUser);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Что-то пошло не так...\n{ex.Message}\n{ex.InnerException}");
                        return;
                    }
                    InitDataGrid();
                    MessageBox.Show("Пользователь успешно удален!");
                }
            }
        }
    }
}

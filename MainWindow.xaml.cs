using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace TextNormalizer
{
    public partial class MainWindow : Window
    {
        private Normalizer _normalizer;

        public MainWindow()
        {
            InitializeComponent();
            new AuthWindow().ShowDialog();
            if (Auth.CurrentUser?.RoleId == 1)
            {
                SettingsButton.Visibility = Visibility.Visible;
                SettingsUserButton.Visibility = Visibility.Hidden;
            }
            else
            {
                SettingsButton.Visibility = Visibility.Hidden;
                SettingsUserButton.Visibility = Visibility.Visible;
            }
            _normalizer = new Normalizer();
            Normalizer.itemsObs.CollectionChanged += UpdateUI;
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            _normalizer.LoadFile();
        }

        private void DeleteDuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            _normalizer.DeleteDuplicate();
            UpdateUI(null, null!);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxStrings.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать строку!");
                return;
            }
            (int index, string item) editableItem = (ListBoxStrings.SelectedIndex, (string)ListBoxStrings.SelectedItem);

            if (editableItem.item == null) return;
            _normalizer.EditString(editableItem);

        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Normalizer.itemsObs.Add(new SecondWindow().ShowDialog());

        }

        private void UpdateUI(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ListBoxStrings.ItemsSource = Normalizer.itemsObs;

        }
        struct InnerItem
        {
            public string Name { get; set; }
            public int Index { get; set; }
        }
        private void NormalizeButton_Click(object sender, RoutedEventArgs e)
        {
            _normalizer.Normalize();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            _normalizer.ExportToExcel();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void SettingsUserButton_Click(object sender, RoutedEventArgs e)
        {
            Models.InnerUser? user = new Models.TextNormalizerDbContext().Users.Where(x => x.Id == Auth.CurrentUser!.Id).Select(x => new Models.InnerUser(
                x.Id,
                x.Login,
                x.Password,
                x.RoleId,
                x.Role.RoleName)).FirstOrDefault();
            if (user != null)
            {
                new ChangePasswordWindow(user).ShowDialog();
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            string about = File.ReadAllText("About.txt");
            MessageBox.Show(about, "О программе");
        }
    }
}

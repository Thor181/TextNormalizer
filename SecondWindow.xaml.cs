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

namespace TextNormalizer
{
    public partial class SecondWindow : Window
    {
        private string? _editString;
        public SecondWindow()
        {
            InitializeComponent();
        }

        public SecondWindow(string editableString)
        {
            InitializeComponent();
            NameStringTextbox.Text = editableString;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameStringTextbox.Text)) return;
            _editString = new Normalizer().NormalizeString(NameStringTextbox.Text);
            this.Close();
        }
        /// <returns>Возвращает отредактированную строку</returns>
        internal new string ShowDialog()
        {
            base.ShowDialog();
            return _editString!;
        }
    }
}

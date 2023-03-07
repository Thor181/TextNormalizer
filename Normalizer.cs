using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Excel = Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Microsoft.Win32;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.IO;

namespace TextNormalizer
{
    internal class Normalizer
    {
        internal static Application application = null!;
        private Excel.Workbook workbook = null!;
        private Excel.Worksheet worksheet = null!;
        private Excel.Worksheet worksheetNorm = null!;

        private List<string> strList = new List<string>();
        internal static ObservableCollection<string> itemsObs = new ObservableCollection<string>();


        internal Normalizer()
        {
            application = new Application { DisplayAlerts = false };
        }

        internal void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Файл Excel|*.xls; *.xlsx|Все файлы|*.*" };
            if (openFileDialog.ShowDialog() == false || openFileDialog.FileName == "") return;
            string filePath = openFileDialog.FileName;
            workbook = application.Workbooks.Open(filePath);
            worksheet = workbook.Worksheets[1];
            var strings = worksheet.Range["A1", "A1048576"].Value;
            strList.Clear();
            itemsObs.Clear();
            foreach (var item in strings)
            {
                if (item != null)
                {
                    strList.Add(item);
                    continue;
                }
                break;
            }
            strList.ForEach(x => itemsObs.Add(x));
        }

        internal void ExportToExcel()
        {
            foreach (Excel.Worksheet item in workbook.Worksheets)
            {
                if (item.Name == "Нормализ. текст")
                {
                    item.Delete();
                }
            }
            worksheetNorm = workbook.Worksheets.Add();
            worksheetNorm.Name = "Нормализ. текст";


            int i = 1;
            var list = itemsObs;
            foreach (var item in list)
            {
                worksheetNorm.Cells[i++, 1].Value = item;
            }
            workbook.Save();

            MessageBox.Show("Экспорт успешно завершен!");

        }

        internal void DeleteDuplicate()
        {
            if (workbook == null)
            {
                MessageBox.Show("Необходимо загрузить файл!");
                return;
            }

            List<string> withoutDuplicates = new HashSet<string>(itemsObs).ToList();
            List<string> duplicates = itemsObs.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            itemsObs = new ObservableCollection<string>(withoutDuplicates);

            
            foreach (Excel.Worksheet item in workbook.Worksheets)
            {
                if (item.Name == "Удаленные повторы")
                {
                    item.Delete();
                }
            }
            Excel.Worksheet worksheetDuplicates = workbook.Worksheets.Add();

            worksheetDuplicates.Name = "Удаленные повторы";
            int i = 1;
            foreach (var item in duplicates)
            {
                worksheetDuplicates.Cells[i++, 1].Value = item;
            }
            workbook.Save();
            
            MessageBox.Show("Повторы успешно удалены!");
        }


        internal void EditString((int index, string editableString) editableItem)
        {
            string editedString = new SecondWindow(editableItem.editableString).ShowDialog();
            itemsObs[editableItem.index] = editedString;
        }

        private void ShowDuplicates(IEnumerable<string> collection)
        {
            if (collection.Count() == 0)
            {
                MessageBox.Show("Повторы не найдены!");
                return;
            }
            string duplicatesString = "";
            foreach (var item in collection)
            {
                duplicatesString += $"{item}; ";
            }

            MessageBox.Show(duplicatesString, "Удаленные повторы");
        }

        internal void Normalize()
        {
            if (workbook == null)
            {
                MessageBox.Show("Необходимо загрузить файл!");
                return;
            }

            List<string> tempCollection = new List<string>();
            foreach (var item in itemsObs)
            {
                tempCollection.Add(NormalizeString(item));


            }
            itemsObs.Clear();
            tempCollection.ForEach(x => itemsObs.Add(x));
            MessageBox.Show("Нормализация успешно завершена!");
        }
        
        internal string NormalizeString(string str)
        {
            str = DeleteWhiteSpace(str);
            //str = ReplaceNumberAsLetter(str);
            str = DeleteSymbols(str);
            str = ToUpperFirstLetter(str);
            str = ReductionLenght(str);
            return str;
        }
        private string DeleteWhiteSpace(string str)
        {
            return Regex.Replace(str, @"\s+", " ");
        }

        private string ReplaceNumberAsLetter(string str)
        {
            var strArray = str.Split(' ');
            var editedStrArray = new List<string>();
            foreach (var item in strArray)
            {
                if (double.TryParse(item, out double asNumber) == true && asNumber * 0 == 0)
                {
                    editedStrArray.Add(item);
                    continue;
                }
                if (Regex.IsMatch(item, @"[А-Яа-я]+"))
                {
                    editedStrArray.Add(item.Replace('0','о')
                                           .Replace('3', 'з')
                                           .Replace('4', 'ч')
                                           .Replace('6', 'б')
                                           .Replace('7', 'т')
                                           .Replace('8', 'в')
                                           .Replace('9', 'д'));
                }
                else if (Regex.IsMatch(item, @"[A-Za-z]+"))
                {
                    editedStrArray.Add(item.Replace('1', 'l')
                                           .Replace('2', 'z')
                                           .Replace('3', 'e')
                                           .Replace('4', 'a')
                                           .Replace('5', 's')
                                           .Replace('6', 'b')
                                           .Replace('7', 't')
                                           .Replace('8', 'b')
                                           .Replace('9', 'g')
                                           .Replace('0', 'o'));
                    
                }
            }
            if (editedStrArray.Count == 1)
            {
                return editedStrArray[0].ToString()!;
            }
            else if (editedStrArray.Count == 0)
            {
                return str;
            }
            return string.Join(" ", editedStrArray);

        }

        private string DeleteSymbols(string str)
        {
            var strArray = str.Split(' ');
            var editedStrArray = new List<string>();
            foreach (var item in strArray)
            {
                var itemStr = item;
                if (Regex.IsMatch(item, @"\w\."))
                {
                    itemStr = itemStr.Replace(".", "");
                }
                editedStrArray.Add(itemStr.Replace('ё', 'е')
                                       .Replace("^", "")
                                       .Replace("%", "")
                                       .Replace("$", "")
                                       .Replace("#", "")
                                       .Replace("@", "")
                                       .Replace("?", "")
                                       .Replace("!", "")
                                       .Replace(":", "")
                                       .Replace(";", "")
                                       .Replace(",", "")
                                       .Replace("&", "")
                                       .Replace("~", "")
                                       .Replace("`", ""));
                
            }
            
            return string.Join(" ", editedStrArray);
        }

        private string ToUpperFirstLetter(string str)
        {
            if (Regex.IsMatch(str, @"[0-9]+"))
            {
                return str;
            }
            var charArray = str.ToLower().ToCharArray();
            charArray[0] = charArray[0].ToString().ToUpper().ToCharArray()[0];
            return string.Join("", charArray);
        }

        private string ReductionLenght(string str)
        {
            if (str.Length > 40)
            {
                List<KeyValuePair<string, string>> listDictionary = new List<KeyValuePair<string, string>>();
                string[] dictionary = File.ReadAllText($"{Environment.CurrentDirectory}/DictionaryReduction.txt").Replace("\r\n", "").Split(';');
                
                foreach (var item in dictionary)
                {
                    if (item != "")
                    {
                        var splitedItem = item.Split(':');
                        listDictionary.Add(new KeyValuePair<string, string>(splitedItem[0], splitedItem[1]));
                    }
                }
                string[] strArray = str.Split(' ');
                foreach (var item in listDictionary)
                {
                    str = str.Replace(item.Key, item.Value);
                    return str;
                }
                return string.Join(" ", strArray);
            }
            return str;
            
        }

        

    }
}

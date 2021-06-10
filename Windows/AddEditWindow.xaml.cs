using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
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
using CarServiceProg.EF;
using Microsoft.Win32;

namespace CarServiceProg
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        Client client;
        bool isAdd = false;
        string photoPath;
        ObservableCollection<Tag> useTags = new ObservableCollection<Tag>();
        ObservableCollection<Tag> unUseTags = new ObservableCollection<Tag>();
        public AddEditWindow(Client cl = null)
        {
            InitializeComponent();

            client = cl;
            if (cl is null)
            {
                client = new Client();
                isAdd = true;
                IDTextBlock.Visibility = Visibility.Collapsed;
                IDTextBox.Visibility = Visibility.Collapsed;
            }

            InputDataGrid.DataContext = client;

            GenderComboBox.ItemsSource = DB.Context.Gender.ToList();
            GenderComboBox.DisplayMemberPath = "Name";

            foreach (var tag in client.Tag)
            {
                useTags.Add(tag);
            }

            var idsTag = client.Tag.Select(r => r.ID);
            var tmpTags = DB.Context.Tag.Where(r => !idsTag.Contains(r.ID))
                .ToList();
            foreach (var tag in tmpTags)
            {
                unUseTags.Add(tag);
            }

            UseTagsListView.ItemsSource = useTags;
            UnUseTagsListView.ItemsSource = unUseTags;
        }

        private void ChangeImgButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image|*.jpg;*.png;*.jpeg";

            var result = fileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var file = new FileInfo(fileDialog.FileName);
                var prjDir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent;
                photoPath = "Clients\\" + file.Name;
                File.Copy(fileDialog.FileName, "..\\..\\" + photoPath, true);
                ClientPhoto.Source = new BitmapImage(new Uri(prjDir.FullName + "\\" + photoPath));
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            var textBoxesInfo = this.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.FieldType == typeof(TextBox))
                .ToList();
            foreach (var tbInfo in textBoxesInfo)
            {
                var tb = (TextBox)tbInfo.GetValue(this);
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }
            }

            var fioTbs = new List<TextBox>()
            {
                FirstNameTextBox,
                LastNameTextBox,
                PatronymicTextBox
            };

            foreach (var tb in fioTbs)
            {
                if(!tb.Text.Select(s => char.IsLetter(s) || s == ' ' || s == '-')
                    .Aggregate((b1,b2)=> b1 && b2))
                {
                    MessageBox.Show("Поля ФИО могут содержать в себе только буквы и следующие символы: пробел и дефис.");
                    return;
                }

                if(tb.Text.Length > 50)
                {
                    MessageBox.Show("Поля фамилии, имени и отчества не могут быть длиннее 50 символов.");
                    return;
                }
            }

            try
            {
                MailAddress mailAddress = new MailAddress(EmailTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Неверная почта");
                return;
            }

            if(!PhoneTextBox.Text.Select(s => char.IsDigit(s) || s == '-' || s == '+' ||
            s == '(' || s == ')' || s == ' ').Aggregate((b1,b2) => b1 && b2))
            {
                MessageBox.Show("Поле телефона может содержать только цифры и следующие символы: плюс, минус, открывающая и закрывающая круглые скобки, знак пробела.");
                return;
            }

            var result = MessageBox.Show("Вы точно хотите сохранить?", "Сохранение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            client.PhotoPath = photoPath;
            if(isAdd)
            {
                DB.Context.Client.Add(client);
            }
            DB.Context.SaveChanges();
            this.Close();
        }

        private void AddTagButtonClick(object sender, RoutedEventArgs e)
        {
            if(UnUseTagsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите теги для добавления");
                return;
            }

            foreach (Tag tag in UnUseTagsListView.SelectedItems)
            {
                useTags.Add(tag);
                client.Tag.Add(tag);
            }

            foreach (var tag in useTags)
            {
                unUseTags.Remove(tag);
            }
        }

        private void DelTagButtonClick(object sender, RoutedEventArgs e)
        {
            if (UseTagsListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите теги для удаления");
                return;
            }

            foreach (Tag tag in UseTagsListView.SelectedItems)
            {
                unUseTags.Add(tag);
                client.Tag.Remove(tag);
            }

            foreach (var tag in unUseTags)
            {
                useTags.Remove(tag);
            }
        }
    }
}

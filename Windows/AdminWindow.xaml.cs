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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CarServiceProg.EF;

namespace CarServiceProg
{
    using Sortings = Dictionary<string, Func<List<Client>, IEnumerable<Client>>>;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        int maxPageCount;
        int pageNumber = 0;
        List<Client> clients;
        List<string> CountOfRecordsOnPage = new List<string>()
        {
            "все",
            "10",
            "50",
            "200"
        };

        Sortings sortings = new Sortings()
        {
            {"по Id", l => l.OrderBy(r => r.ID) },
            {"фамилии (в алфавитном порядке)", l => l.OrderBy(r => r.LastName) },
            {"дате последнего посещения (от новых к старым)", l => l.OrderByDescending(r => r.LastVisit) },
            {"количеству посещений (от большего к меньшему)", l => l.OrderByDescending(r => r.VisitsCount) }
        };
        public AdminWindow()
        {
            InitializeComponent();
            CountOfRecordsComboBox.ItemsSource = CountOfRecordsOnPage;
            CountOfRecordsComboBox.SelectedIndex = 0;

            var genders = DB.Context.Gender.Select(r => r.Name).ToList();
            genders.Insert(0, "все");
            GenderComboBox.ItemsSource = genders;
            GenderComboBox.SelectedIndex = 0;

            SortingComboBox.ItemsSource = sortings.Keys;
            SortingComboBox.SelectedIndex = 0;

            UpdateClients();
        }

        private void UpdateClients(bool updatePage = false)
        {
            DB.Context = new Context();
            if (updatePage)
                pageNumber = 0;


            clients = DB.Context.Client.ToList();

            if ((string)GenderComboBox.SelectedItem != "все")
            {
                clients = clients
                    .Where(r => r.Gender.Name == (string)GenderComboBox.SelectedItem)
                    .ToList();
            }

            if (BirthDayCB.IsChecked.HasValue && BirthDayCB.IsChecked.Value)
                clients = clients
                    .Where(r => r.Birthday.HasValue && r.Birthday.Value.Month == DateTime.Now.Month)
                    .ToList();

            if(!string.IsNullOrWhiteSpace( SearchTextBox.Text))
            {
                var input = SearchTextBox.Text;
                clients = clients
                    .Where(r => r.FirstName.Contains(input) ||
                    r.LastName.Contains(input) ||
                    r.Patronymic.Contains(input) ||
                    r.Email.Contains(input) ||
                    r.Phone.Contains(input)
                    ).ToList();

            }

            int filteredCount = clients.Count;

            if (SortingComboBox.SelectedItem != null)
                clients = sortings[(string)SortingComboBox.SelectedItem](clients).ToList();

            if ((string)CountOfRecordsComboBox.SelectedItem != "все")
            {
                int n = int.Parse((string)CountOfRecordsComboBox.SelectedItem);
                maxPageCount = (int)Math.Ceiling((double)filteredCount / n);
                clients = clients.Skip(pageNumber * n).Take(n).ToList();
            }
            CountOfRecordsTextBlock.Text = $"{clients.Count} из {filteredCount}";
            DataListView.ItemsSource = clients;
        }

        private void UpdateTB(object sender, EventArgs e)
        {
            UpdateClients();
        }

        private void UpdateTBWithPage(object sender, EventArgs e)
        {
            UpdateClients(true);
        }

        private void PrevButtonClick(object sender, RoutedEventArgs e)
        {
            if (pageNumber - 1 >= 0)
                pageNumber--;
            UpdateClients();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            if (pageNumber + 1 < maxPageCount)
                pageNumber++;
            UpdateClients();
        }

        private void DelButtonClick(object sender, RoutedEventArgs e)
        {
            if(DataListView.SelectedItem == null)
            {
                MessageBox.Show("Надо выбрать запись для удаления!");
                return;
            }
            var selcted = (Client)DataListView.SelectedItem;

            if(selcted.ClientService.Count > 0)
            {
                MessageBox.Show("у клиента есть информация о посещениях, нельзя удалит");
                return;
            }

            var result = MessageBox.Show("Вы точно хотите удалить?","Удаление",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;

            foreach (var tag in selcted.Tag)
            {
                selcted.Tag.Remove(tag);
            }

            DB.Context.SaveChanges();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            new AddEditWindow().ShowDialog();
            UpdateClients();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataListView.SelectedItem == null)
            {
                MessageBox.Show("Надо выбрать запись!");
                return;
            }
            var selcted = (Client)DataListView.SelectedItem;
            new AddEditWindow(selcted).ShowDialog();
            UpdateClients();
        }

        private void VisitsButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataListView.SelectedItem == null)
            {
                MessageBox.Show("Надо выбрать запись!");
                return;
            }
            var selcted = (Client)DataListView.SelectedItem;

            if (selcted.ClientService.Count == 0)
            {
                MessageBox.Show("У пользователя 0 записей");
                return;
            }

            new VisitsWindow(selcted).ShowDialog();
        }
    }
}

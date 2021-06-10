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
using CarServiceProg.EF;

namespace CarServiceProg
{
    /// <summary>
    /// Interaction logic for VisitsWindow.xaml
    /// </summary>
    public partial class VisitsWindow : Window
    {
        Client client;
        public VisitsWindow(Client cl)
        {
            InitializeComponent();
            VisitsListView.ItemsSource = cl.ClientService;
            client = cl;
        }

        private void VisitClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                var selected = ((ClientService)VisitsListView.SelectedItem);
                if (selected.CountOfDocs == 0)
                {
                    MessageBox.Show("нет прикреплённых документов");
                    return;
                }
                new ListOfDocsWindow(selected.DocumentByService.ToList()).ShowDialog();
            }
        }
    }
}

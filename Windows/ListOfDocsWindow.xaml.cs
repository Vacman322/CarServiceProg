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
    /// Interaction logic for ListOfDocsWindow.xaml
    /// </summary>
    public partial class ListOfDocsWindow : Window
    {
        public ListOfDocsWindow(List<DocumentByService> documents)
        {
            InitializeComponent();
            DocsList.ItemsSource = documents;
        }
    }
}

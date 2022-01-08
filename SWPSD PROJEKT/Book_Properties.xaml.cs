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

namespace SWPSD_PROJEKT
{
    /// <summary>
    /// Logika interakcji dla klasy Book_Properties.xaml
    /// </summary>
    public partial class Book_Properties : Page
    {
        Book book;
        public Book_Properties(Book book)
        {
            InitializeComponent();
               this.book = book;
            booktitle.Text = this.book.title;
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}

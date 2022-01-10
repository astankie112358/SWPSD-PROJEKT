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
        List<Book> bookstoborrow;
        int clientid;
        public Book_Properties(Book book, List<Book> bookstoborrow, int clientid)
        {
            InitializeComponent();
               this.book = book;
            booktitle.Text = this.book.title;
            this.bookstoborrow= bookstoborrow;
            this.clientid = clientid;
            this.cantadd.Visibility = System.Windows.Visibility.Hidden;
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void getbookbutton_Click(object sender, RoutedEventArgs e)
        {
            if ((Book)bookstoborrow.Find(item => item.title == book.title) == null && book.free == true)
            {
                bookstoborrow.Add(this.book);
                this.NavigationService.Navigate(new BooksWindowPage(bookstoborrow, clientid));
            }
            else
                this.cantadd.Visibility = System.Windows.Visibility.Visible;
        }
    }
}

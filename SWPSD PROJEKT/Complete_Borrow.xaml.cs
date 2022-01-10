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
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SWPSD_PROJEKT
{
    /// <summary>
    /// Logika interakcji dla klasy Complete_Borrow.xaml
    /// </summary>
    public partial class Complete_Borrow : Page
    {
        int id_client;
        List<Book> bookstoborrow;
        public Complete_Borrow(int idclient, List<Book> bookstoborrow)
        {
            InitializeComponent();
            this.id_client = idclient;
            this.bookstoborrow = bookstoborrow;
            foreach(Book book in bookstoborrow)
            {
                this.booksinorder.Items.Add(book);
            }

        }

        private void completeborrow_Click(object sender, RoutedEventArgs e)
        {
            DBConnection conn = new DBConnection();
            conn.IsConnect();
            string query = "insert into biblioteka.wypozyczenie(id_czytelnik) values(@id)";
            MySqlCommand com = new MySqlCommand(query, conn.Connection);
            com.Parameters.AddWithValue("@id", this.id_client);
            com.ExecuteNonQuery();
            query = "SELECT* FROM biblioteka.wypozyczenie ORDER BY idwypozyczenie DESC LIMIT 0, 1";
            var cmd = new MySqlCommand(query, conn.Connection);
            var reader = cmd.ExecuteReader();
            int id_order=0;
            while (reader.Read())
            {
                id_order=Int32.Parse(reader.GetString(0));
            }
            reader.Close();
            foreach (Book book in bookstoborrow)
            {
                string query1 = "insert into biblioteka.pozycja_wypozyczenie(id_ksiazka,id_wypozyczenie,data_wypozyczenia,data_zwrotu) values(@book,@order,now(),null)";
                MySqlCommand com1 = new MySqlCommand(query1, conn.Connection);
                com1.Parameters.AddWithValue("@book", book.id);
                com1.Parameters.AddWithValue("@order", id_order);
                com1.ExecuteNonQuery();
                
            }
            conn.Close();
            this.NavigationService.Navigate(new MainMenuPage());
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}

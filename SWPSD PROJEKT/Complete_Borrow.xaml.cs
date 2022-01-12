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
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;

namespace SWPSD_PROJEKT
{

    public partial class Complete_Borrow : Page
    {
        int id_client;
        List<Book> bookstoborrow;
        public SpeechSynthesizer pTTS = new SpeechSynthesizer();
        public SpeechRecognitionEngine pSRE = new SpeechRecognitionEngine();
        public Complete_Borrow(int idclient, List<Book> bookstoborrow)
        {
            InitializeComponent();
            buildgrammar();
            this.id_client = idclient;
            this.bookstoborrow = bookstoborrow;
            foreach(Book book in bookstoborrow)
            {
                this.booksinorder.Items.Add(book);
            }

        }
        public void complete_borrow()
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
            int id_order = 0;
            while (reader.Read())
            {
                id_order = Int32.Parse(reader.GetString(0));
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
            pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
            this.NavigationService.Navigate(new MainMenuPage());
        }
        private void completeborrow_Click(object sender, RoutedEventArgs e)
        {
            complete_borrow();
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
            this.NavigationService.Navigate(new BooksWindowPage(bookstoborrow, id_client));
        }
        public void buildgrammar()
        {
            try
            {
                pTTS.SetOutputToDefaultAudioDevice();
                pTTS.Speak("Twoje zamówienia");
                CultureInfo ci = new CultureInfo("pl-PL");
                pSRE = new SpeechRecognitionEngine(ci);

                pSRE.SetInputToDefaultAudioDevice();

                pSRE.SpeechRecognized += PSRE_SpeechRecognized;

                GrammarBuilder buildGrammarSystem = new GrammarBuilder();
                Choices choice = new Choices();
                choice.Add("Wypożycz");
                choice.Add("Wróć");
                buildGrammarSystem.Append(choice);
                Grammar calc = new Grammar(buildGrammarSystem);
                pSRE.LoadGrammarAsync(calc);
                pSRE.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        private void PSRE_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            string comments;
            float confidence = e.Result.Confidence;
            if (confidence > 0.20)
            {
                if (txt.IndexOf("Wróć") >= 0)
                {
                    pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
                    this.NavigationService.Navigate(new BooksWindowPage(bookstoborrow, id_client));
                }
                else if(txt.IndexOf("Wypożycz") >= 0)
                {
                    complete_borrow();
                }
            }

        }
    }
}

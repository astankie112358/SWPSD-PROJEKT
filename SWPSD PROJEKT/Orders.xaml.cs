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
    public class Order
    {
        public int orderid { get; set; }
        public int bookid { get; set; }
        public string bookname { get; set; }
        public string orderdate { get; set; }

        public string backdate { get; set; }

        public Order(int orderid, int bookid, string bookname, string orderdate, string backdate)
        {
            this.orderid = orderid;
            this.bookid = bookid;
            this.bookname = bookname;
            this.orderdate = orderdate;
            this.backdate = backdate;
        }
    }
    public partial class Orders : Page
    {
        public SpeechSynthesizer pTTS = new SpeechSynthesizer();
        public SpeechRecognitionEngine pSRE= new SpeechRecognitionEngine();
        int user_id;
        List<Order> orders=new List<Order>();
        public Orders(int user_id)
        {
            DBConnection conn = new DBConnection();
            this.user_id = user_id;
            InitializeComponent();
            whichtoshow.Items.Add("Nie zwrócone");
            whichtoshow.Items.Add("Zwrócone");
            whichtoshow.SelectedIndex = 0;

            conn.IsConnect();
            string query = "select powwyp.idpozycja_wypozyczenie, ks.idksiazka, ks.tytul, powwyp.data_wypozyczenia, powwyp.data_zwrotu  from biblioteka.pozycja_wypozyczenie as powwyp inner join biblioteka.wypozyczenie as wyp on wyp.idwypozyczenie = powwyp.id_wypozyczenie inner join biblioteka.ksiazka as ks on ks.idksiazka = powwyp.id_ksiazka inner join biblioteka.czytelnik as cz on cz.idczytelnik = wyp.id_czytelnik where idczytelnik = " + this.user_id;
            var cmd = new MySqlCommand(query, conn.Connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string a=reader.GetString(0);
                try
                {
                    Order o = new Order(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetString(4));
                    orders.Add(new Order(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
                }
                catch (Exception ex)
                {
                    Order o = new Order(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), "");
                    orders.Add(new Order(Int32.Parse(reader.GetString(0)), Int32.Parse(reader.GetString(1)), reader.GetString(2), reader.GetString(3), ""));
                }
            }
            foreach (Order order in orders)
            {
                if(order.backdate=="")
                {
                    this.orderlist.Items.Add(order);
                }
            }
            reader.Close();
            if (orderlist.Items.Count > 0)
                orderlist.SelectedIndex = 0;
            buildgrammar();

        }

        private void whichtoshow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.whichtoshow.SelectedIndex == 0)
            {
                this.orderlist.Items.Clear();
                this.finishborrow.Visibility = Visibility.Visible;
                foreach (Order order in orders)
                {
                    if (order.backdate == "")
                    {
                        this.orderlist.Items.Add(order);
                    }
                }
            }
            else
            {
                this.orderlist.Items.Clear();
                this.finishborrow.Visibility = Visibility.Hidden;
                foreach (Order order in orders)
                {
                    if (order.backdate != "")
                    {
                        this.orderlist.Items.Add(order);
                    }
                }
                
            }
            if (orderlist.Items.Count > 0)
            {
                orderlist.SelectedIndex = 0;
            }
        }
        public void returnbook()
        {
            Order thisorder = ((Order)this.orderlist.SelectedItem);
            DBConnection conn = new DBConnection();
            conn.IsConnect();
            string query = "update biblioteka.pozycja_wypozyczenie set data_zwrotu=now() where idpozycja_wypozyczenie = " + thisorder.orderid;
            MySqlCommand com = new MySqlCommand(query, conn.Connection);
            com.ExecuteNonQuery();
            pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
            this.NavigationService.Navigate(new Orders(this.user_id));
        }
        private void finishborrow_Click(object sender, RoutedEventArgs e)
        {
            returnbook();
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
            this.NavigationService.Navigate(new MainMenuPage());
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
                choice.Add("Dół");
                choice.Add("Góra");
                choice.Add("Wróć");
                choice.Add("Zwróć książkę");
                choice.Add("Pokaż oddane");
                choice.Add("Pokaż nieoddane");
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
                if (txt.IndexOf("Dół") >= 0)
                {
                    if (orderlist.SelectedIndex > 0)
                        orderlist.SelectedIndex = orderlist.SelectedIndex + 1;
                }
                else if (txt.IndexOf("Góra") >= 0)
                {
                    if (orderlist.SelectedIndex > 0)
                        orderlist.SelectedIndex = orderlist.SelectedIndex - 1;
                }
                else if (txt.IndexOf("Wróć") >= 0)
                {
                    pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
                    this.NavigationService.Navigate(new MainMenuPage());
                }
                else if (txt.IndexOf("Zwróć książkę") >= 0)
                {
                    returnbook();
                }
                else if (txt.IndexOf("Pokaż oddane") >= 0)
                {
                    this.whichtoshow.SelectedIndex = 1;
                }
                else if (txt.IndexOf("Pokaż nieoddane") >= 0)
                {
                    this.whichtoshow.SelectedIndex = 0;
                }


            }

        }
    }
}

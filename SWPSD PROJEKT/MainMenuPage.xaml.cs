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
    /// <summary>
    /// Logika interakcji dla klasy MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        DBConnection conn=new DBConnection();
        public static SpeechSynthesizer pTTS = new SpeechSynthesizer();
        public static SpeechRecognitionEngine pSRE;
        public MainMenuPage()
        {
            InitializeComponent();
            buildgrammar();
        }
        public void navigatetobooksview()
        {

            this.conn.IsConnect();
            if (getuserid.Text != "")
            {
                string query = "select * from biblioteka.czytelnik where idczytelnik = " + getuserid.Text;
                var cmd = new MySqlCommand(query, conn.Connection);
                var reader = cmd.ExecuteReader();
                int userid;


                if (reader.HasRows)
                {
                    userid = Int32.Parse(getuserid.Text);
                    this.NavigationService.Navigate(new BooksWindowPage(null, userid));
                }
                reader.Close();
            }
        }

        public void navigatetoorders()
        {
            this.conn.IsConnect();
            if (getuserid.Text != "")
            {
                string query = "select * from biblioteka.czytelnik where idczytelnik = " + getuserid.Text;
                var cmd = new MySqlCommand(query, conn.Connection);
                var reader = cmd.ExecuteReader();
                int userid;


                if (reader.HasRows)
                {
                    userid = Int32.Parse(getuserid.Text);
                    this.NavigationService.Navigate(new Orders(userid));
                }
                reader.Close();
            }
        }

        private void showbooksbutton_Click(object sender, RoutedEventArgs e)
        {
            
            navigatetobooksview();
        }
        public void buildgrammar()
        {
            try
            {
                pTTS.SetOutputToDefaultAudioDevice();
                pTTS.Speak("Witamy w bibliotece");
                CultureInfo ci = new CultureInfo("pl-PL");
                pSRE = new SpeechRecognitionEngine(ci);

                pSRE.SetInputToDefaultAudioDevice();

                pSRE.SpeechRecognized += PSRE_SpeechRecognized;

                GrammarBuilder buildGrammarSystem = new GrammarBuilder();
                Choices choice = new Choices();
                choice.Add("Wypożycz książkę");
                choice.Add("Zwróć książkę");
                string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                choice.Add(numbers);
                choice.Add("Usuń");
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
                if (txt.IndexOf("Wypożycz książkę") >= 0)
                {
                    navigatetobooksview();
                }
                else if (txt.IndexOf("Zwróć książkę") >= 0)
                {
                    Console.WriteLine("");

                }
                else if (txt.IndexOf("Usuń") >= 0)
                {
                    if(getuserid.Text.Length>0)
                    getuserid.Text = getuserid.Text.Remove(getuserid.Text.Length - 1,1);

                }
                else
                {
                    getuserid.Text= getuserid.Text+txt;
                }


            }

        }

        private void borrows_button_Click(object sender, RoutedEventArgs e)
        {
            navigatetoorders();
        }
    }
}





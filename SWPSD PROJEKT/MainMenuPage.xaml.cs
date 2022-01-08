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
        DBConnection conn;
        public static SpeechSynthesizer pTTS = new SpeechSynthesizer();
        public static SpeechRecognitionEngine pSRE;
        public MainMenuPage()
        {
            InitializeComponent();
            buildgrammar();
        }
        public void navigatetobooksview()
        {
            this.NavigationService.Navigate(new BooksWindowPage());
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


            }

        }
    }
}





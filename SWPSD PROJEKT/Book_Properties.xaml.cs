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
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
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
        SpeechSynthesizer pTTS = new SpeechSynthesizer();
        SpeechRecognitionEngine pSRE = new SpeechRecognitionEngine();
        public Book_Properties(Book book, List<Book> bookstoborrow, int clientid)
        {
            InitializeComponent();
            buildgrammar();
            this.book = book;
            booktitle.Text = this.book.title;
            this.bookstoborrow = bookstoborrow;
            this.clientid = clientid;
            this.cantadd.Visibility = System.Windows.Visibility.Hidden;
        }
        public void back()
        {
            pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
            this.NavigationService.Navigate(new BooksWindowPage(bookstoborrow, clientid));
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            back();
        }
        private void addbook(){
            if ((Book)bookstoborrow.Find(item => item.title == book.title) == null && book.free == true)
            {
                bookstoborrow.Add(this.book);
                pTTS.Speak("Dodano książkę");
                pSRE.SpeechRecognized -= PSRE_SpeechRecognized;
                this.NavigationService.Navigate(new BooksWindowPage(bookstoborrow, clientid));

            }
            else
            {
                this.cantadd.Visibility = System.Windows.Visibility.Visible;
                pTTS.Speak("Nie można dodać książki");
            }
        }
        private void getbookbutton_Click(object sender, RoutedEventArgs e)
        {
            addbook();
        }
        public void buildgrammar()
        {
            try
            {
                pTTS.SetOutputToDefaultAudioDevice();
                CultureInfo ci = new CultureInfo("pl-PL");
                pSRE = new SpeechRecognitionEngine(ci);

                pSRE.SetInputToDefaultAudioDevice();

                pSRE.SpeechRecognized += PSRE_SpeechRecognized;

                GrammarBuilder buildGrammarSystem = new GrammarBuilder();
                Choices choice = new Choices();
                choice.Add("Wróć");
                choice.Add("Wypożycz");
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
                    back();
                }
                else if(txt.IndexOf("Wypożycz") >= 0)
                {
                    addbook();

                }

            }

        }
    }
}

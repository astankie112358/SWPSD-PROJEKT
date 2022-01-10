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

    public partial class BooksWindowPage : Page
    {
        DBConnection conn;
        public List<Book> books = new List<Book>();
        public static SpeechSynthesizer pTTS = new SpeechSynthesizer();
        public static SpeechRecognitionEngine pSRE;
        public List<Book> bookstoborrow;
        int idclient;
        public BooksWindowPage(List<Book> bookstoborrow, int idclient)
        {
            conn = new DBConnection();
            this.idclient= idclient;
            
            InitializeComponent();
            this.nobooks.Visibility = System.Windows.Visibility.Hidden;
            if (bookstoborrow != null)
                this.bookstoborrow = bookstoborrow;
            else
                this.bookstoborrow=new List<Book>();
            this.conn.IsConnect();
            string query = "select ks.idksiazka, ks.tytul, gt.nazwa, concat(aut.imie, \" \", aut.nazwisko), ks.wolna from biblioteka.ksiazka as ks inner join biblioteka.autor as aut on aut.idautor = ks.autor inner join biblioteka.ksiazka_gatunek kg on kg.id_ksiazka = ks.idksiazka inner join biblioteka.gatunek as gt on gt.idgatunek = kg.id_gatunek order by id_ksiazka";
            var cmd = new MySqlCommand(query, conn.Connection);
            var reader=cmd.ExecuteReader();
            selection_box.Items.Add("Tytuły");
            selection_box.Items.Add("Autorzy");
            selection_box.Items.Add("Gatunki");
            selection_box.SelectedIndex = 0;
                        while (reader.Read())
            {
                int isfree = Int32.Parse(reader.GetString(4));
                bool free=false;
                    if (isfree > 0)
                {
                    free=true;
                }
                Book book = new Book(Int32.Parse(reader.GetString(0)), reader.GetString(1), reader.GetString(3), null, free);
                bool exist = false;
                foreach(Book booksearch in books)
                {
                    if (book.id==booksearch.id)
                    {
                        exist = true;
                        booksearch.types.Add(reader.GetString(2)); 
                        break;
                    }
                }
                if (!exist)
                {
                    book.types.Add(reader.GetString(2));
                    books.Add(book);
                }
            }
            conn.Close();
            foreach (Book book in books)
            {
                foreach(string type in book.types)
                {
                    book.typesview= book.typesview+type+" ";
                }
                booklist.Items.Add(book);
            }
            booklist.SelectedIndex = 0;

            buildgrammar();
        }

        private void mainmenubutton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainMenuPage());
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void changesort()
        {
            if (selection_box.SelectedValue.ToString() == "Tytuły")
            {
                booklist.Items.Clear();
                books = books.OrderBy(o => o.title).ToList();
                foreach (Book book in books)
                {
                    booklist.Items.Add(book);
                }


            }
            else if (selection_box.SelectedValue.ToString() == "Autorzy")
            {
                booklist.Items.Clear();
                books = books.OrderBy(o => o.author).ToList();
                foreach (Book book in books)
                {
                    booklist.Items.Add(book);
                }

            }
            else
            {
                if (selection_box.SelectedValue.ToString() == "Gatunek")
                {
                    booklist.Items.Clear();
                    books = books.OrderBy(o => o.typesview).ToList();
                    foreach (Book book in books)
                    {
                        booklist.Items.Add(book);
                    }

                }
            }
        }

        private void selection_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changesort();
        }
        private void scrollup()
        {
            if (booklist.SelectedIndex > 0)
                booklist.SelectedIndex = booklist.SelectedIndex - 1;
        }
        private void scrolldown()
        {
            if (booklist.SelectedIndex < books.Count)
                booklist.SelectedIndex = booklist.SelectedIndex + 1;
        }
        private void upscroll_Click(object sender, RoutedEventArgs e)
        {
            scrollup();
        }

        private void downscroll_Click(object sender, RoutedEventArgs e)
        {
            scrolldown();
        }

        public bool comparetext(Book book)
        {
            if(selection_box.SelectedValue.ToString() == "Tytuły")
            {
                if (book.title.IndexOf(searchbar.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
                else
                    return false;
            }
            else if (selection_box.SelectedValue.ToString() == "Autorzy")
            {
                if (book.author.IndexOf(searchbar.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
                else
                    return false;
            }
            else
            {
                foreach(string gatunek in book.types)
                {
                    if(gatunek.IndexOf(searchbar.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }

        }
        private void searchbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            booklist.Items.Clear();
            List<Book> booksaftersearch= books.FindAll(comparetext);
            foreach(Book book in booksaftersearch)
            {
                booklist.Items.Add(book);
            }
            booklist.SelectedIndex = 0;

        }
        private void go_into_book_details()
        {
            this.NavigationService.Navigate(new Book_Properties(booklist.SelectedItem as Book, this.bookstoborrow,this.idclient));
        }

        private void go_into_details_Click(object sender, RoutedEventArgs e)
        {
            go_into_book_details();
        }

        public void buildgrammar()
        {
            try
            {
                pTTS.SetOutputToDefaultAudioDevice();
                pTTS.Speak("Wybierz książkę");
                CultureInfo ci = new CultureInfo("pl-PL");
                pSRE = new SpeechRecognitionEngine(ci);
                pSRE.SetInputToDefaultAudioDevice();
                pSRE.SpeechRecognized += PSRE_SpeechRecognized;

                GrammarBuilder buildGrammarSystem = new GrammarBuilder();
                Choices choice1 = new Choices();
                choice1.Add("Wybierz");
                choice1.Add("Góra");
                choice1.Add("Dół");
                choice1.Add("Usuń");
                choice1.Add("Wróć");
                buildGrammarSystem.Append(choice1);
                Grammar basic = new Grammar(buildGrammarSystem);
                pSRE.LoadGrammarAsync(basic);

                GrammarBuilder buildGrammarSystem1 = new GrammarBuilder();
                Choices choice3 = new Choices();
                choice3.Add("Wskaż");
                buildGrammarSystem1.Append(choice3);
                Choices choice4 = new Choices();
                foreach(Book book in booklist.Items)
                {
                    choice4.Add(book.title);
                }
                buildGrammarSystem1.Append(choice4);
                Grammar selectbook = new Grammar(buildGrammarSystem1);
                pSRE.LoadGrammarAsync(selectbook);

                GrammarBuilder buildGrammarSystem2 = new GrammarBuilder();
                Choices choice5 = new Choices();
                choice5.Add("Wyszukaj");
                string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s", "t", "y","w","z","ż","ź" };
                Choices choice6 = new Choices();
                choice6.Add(letters);
                buildGrammarSystem2.Append(choice5);
                buildGrammarSystem2.Append(choice6);
                Grammar search = new Grammar(buildGrammarSystem2);
                pSRE.LoadGrammarAsync(search);

                GrammarBuilder buildGrammarSystem3 = new GrammarBuilder();
                Choices choice7 = new Choices();
                choice7.Add("Sortuj po");
                buildGrammarSystem3.Append(choice7);
                Choices choice8 = new Choices();
                choice8.Add("Tytuły");
                choice8.Add("Autorzy");
                choice8.Add("Gatunki");
                buildGrammarSystem3.Append(choice8);
                Grammar sortby = new Grammar(buildGrammarSystem3);
                pSRE.LoadGrammarAsync(sortby);
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
                if (txt.IndexOf("Wybierz") >= 0)
                {
                    go_into_book_details();
                }
                else if (txt.IndexOf("Góra") >= 0)
                {
                    scrollup();
                }
                else if (txt.IndexOf("Dół") >= 0)
                {
                    scrolldown();
                }
                else if(txt.IndexOf("Wyszukaj") >= 0)
                {
                    string letter = txt.Substring(9);
                    this.searchbar.Text = this.searchbar.Text+letter;
                }
                else if (txt.IndexOf("Usuń") >= 0)
                { 
                    if(this.searchbar.Text.Length>0)
                    this.searchbar.Text = this.searchbar.Text.Remove(this.searchbar.Text.Length - 1,1);
                }
                else if (txt.IndexOf("Wróć") >= 0)
                {
                    this.NavigationService.GoBack();
                }
                else if (txt.IndexOf("Sortuj po") >= 0)
                {
                    this.selection_box.SelectedValue = txt.Substring(10);
                }
                else if (txt.IndexOf("Wskaż") >= 0)
                {
                    string bookname=txt.Substring(6);
                    int i = 0;
                    foreach(Book book in booklist.Items)
                    {
                        if (book.title == bookname)
                        {
                            booklist.SelectedIndex = i;
                        }
                        i++;
                    }
                }

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.bookstoborrow.Count > 0)
            this.NavigationService.Navigate(new Complete_Borrow(idclient,bookstoborrow));
            else
                this.nobooks.Visibility = System.Windows.Visibility.Visible;
        }
    }

}

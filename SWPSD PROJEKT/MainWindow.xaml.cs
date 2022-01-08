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

    public partial class MainWindow : Window
    {
        DBConnection conn;
        public MainWindow()
        {
            InitializeComponent(); 
            PageFrame.Navigate(new MainMenuPage());
            conn = new DBConnection();
            Console.WriteLine("aaa");

        }
        


    }
    public class DBConnection
    {
        public DBConnection()
        {
            server = "127.0.0.1";
            databaseName = "biblioteka";
            userName = "root";
            password = "nuka2008";
            port = "3306";
        }

        public string server { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }

        public string port { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(this.databaseName))
                    return false;
                string connstring = string.Format("Server={0};Port={1} ;database={2}; UID={3}; password={4}", server, port, databaseName, userName, password);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
            }
            return true;
        }

        public void Close()
        {
            Connection.Close();
        }
    }
}

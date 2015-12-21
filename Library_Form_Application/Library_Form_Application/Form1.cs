using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Connection;
using Oracle.DataAccess.Client;


namespace Library_Form_Application
{
    public partial class Form1 : Form
    {
        enum adaptersIndexes
        {
            studentsIndex,
            booksIndex,
            cardsIndex,
            penalizationsIndex,
            debtsIndex,
            loansIndex,
            returnsIndex
        }

        public Form1()
        {
            InitializeComponent();
        }

        private Connection.Connection _connection;
        private OracleConnection _oracleConn;
        private Entity[] _adapters;

        public bool InitOracleDbConnection()
        {
            bool bRet = true;
            try
            {
                _connection = new Connection.Connection();
                _oracleConn = _connection.GetConnectionObject();
                bRet = _connection.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bRet;
        }

        public bool Login()
        {
            bool bRet = _connection.Login("user1", "user1");
            String message;
            message = (bRet == true ? "Utilizator logat" : "Utilizator nelogat");
            MessageBox.Show(message);
            return bRet;
        }

        private void InitAdapters()
        {
            _adapters = new Entity[7];
            _adapters[(int)adaptersIndexes.booksIndex] = new Books(_oracleConn);
            
          
        }

        private void OnLoad(object sender, EventArgs e)
        {
            bool isInitializedOK = InitOracleDbConnection();
            if ( true == isInitializedOK)
            {
                Login();
                InitAdapters();
            }
            
        }

        private void LoadAllBooks(object sender, EventArgs e)
        {
            
            String[] books = { "Book1", "Book2" };
           // String[] books = booksAdapter.GetAllBooks();
            foreach (String book in books)
            {
                listBoxBooks.Items.Add(book);
            }
        }

        private void AddBook(object sender, EventArgs e)
        {
            try
            {
                String title = BooksAddTitleTb.Text;
                String author = BooksAddAuthorTb.Text;
                String publisher = BooksAddPublisherTb.Text;
                String type = BooksAddTypeTb.Text;
                int totalStock = Int32.Parse(BooksAddTotalStockTb.Text);
                int avalaibleStock = Int32.Parse(BooksAddAvalaibleStockTb.Text);
                String[] lines = BooksAddPublicationDateTb.Text.Split("-./".ToCharArray());
                PubDate date = new PubDate(lines[0], lines[1], lines[2]);

                Books booksAdapter = (Books)_adapters[(int)adaptersIndexes.booksIndex];
                bool ret = booksAdapter.AddBook(title, author, date, publisher, totalStock, avalaibleStock, type);
                String message = (ret ? "Book added !" : "Book not added : Fill all fields !");
                MessageBox.Show(message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

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
        public Form1()
        {
            InitializeComponent();
        }

        private Connection.Connection connection;
        private OracleConnection oracleConn;
        Books.Books booksAdapter;
        int bookIndex;

        public bool InitOracleDbConnection()
        {
            bool bRet = true;
            try
            {
                connection = new Connection.Connection();
                bRet = connection.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bRet;
        }

        public bool Login()
        {
            bool bRet = connection.Login("user1", "user1");
            String message;
            message = (bRet == true ? "Utilizator logat" : "Utilizator nelogat");
            MessageBox.Show(message);
            return bRet;
        }

        private void InitAdapters()
        {
            booksAdapter = new Books.Books(connection.GetConnectionObject());
            bookIndex = booksAdapter.GetAllBooks().Length;
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
            
           // String[] books = { "Book1", "Book2" };
            String[] books = booksAdapter.GetAllBooks();
            foreach (String book in books)
            {
                listBoxBooks.Items.Add(book);
            }
        }

        private void AddBook(object sender, EventArgs e)
        {
            try
            {
                int bookId = ++bookIndex;
                String title = BooksAddTitleTb.Text;
                String author = BooksAddAuthorTb.Text;
                String publisher = BooksAddPublisherTb.Text;
                String type = BooksAddTypeTb.Text;
                int totalStock = Int32.Parse(BooksAddTotalStockTb.Text);
                int avalaibleStock = Int32.Parse(BooksAddAvalaibleStockTb.Text);
                String[] lines = BooksAddPublicationDateTb.Text.Split("-./".ToCharArray());
                Books.PubDate date = new Books.PubDate(lines[0], lines[1], lines[2]);

                bool ret = booksAdapter.AddBook(bookId, title, author, date, publisher, totalStock, avalaibleStock, type);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Windows.Forms;

namespace Library_Form_Application
{
    public class Books:Entity
    {
        public List<Book> books;
        public class Book
        {
            public String bookId;
            public String title;
            public String author;
            public String pubDate;
            public String publisher;
            public String totalStock;
            public String avalaibleStock;
            public String type;

            public Book(String bookIdC, String titleC, String authorC, String pubDateC, String publisherC, String totalC, String avalaibleC, String typeC)
            {
                bookId = bookIdC;
                title = titleC;
                author = authorC;
                pubDate = pubDateC;
                publisher = publisherC;
                totalStock = totalC;
                avalaibleStock = avalaibleC;
                type = typeC;
            }
        }

        private int _numberOfBooks;

        public Books(OracleConnection conn):base(conn)
        {
            _numberOfBooks = 0;
            books = new List<Book>();
        }

        public List<Book> GetAllBooks()
        {
            String command = "SELECT * FROM BOOKS";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            books.Clear();
            /* get number of books */
            /*System.Data.DataTable dt = new System.Data.DataTable();            
            dt.Load(dr);
            numEntries = dt.Rows.Count;*/

            while (dr.Read())
            {
                try
                {
                    books.Add(new Book(
                        dr["BOOK_ID"].ToString(),
                        dr["TITLE"].ToString(),
                        dr["AUTHOR"].ToString(),
                        dr["PUBLICATION_DATE"].ToString(),
                        dr["PUBLISHER"].ToString(),
                        dr["TOTAL_STOCK"].ToString(),
                        dr["AVALAIBLE_STOCK"].ToString(),
                        dr["TYPE"].ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la citirea cartilor : " + ex.ToString());
                }
            }
            _numberOfBooks = books.Count;
            return books;
        }

        private int GetNextBookIndex()
        {
            int index = 0;
            String cmdString = String.Format("SELECT MAX(BOOK_ID) MAX_ID FROM Books");
            OracleCommand GetMaxIndexBookCmd = new OracleCommand(cmdString, _connection);
            
            OracleDataReader dr = GetMaxIndexBookCmd.ExecuteReader();
            if(dr.Read())
            {
                index = Int32.Parse(dr["MAX_ID"].ToString()) + 1;
            }

            return index;
        }

    


        public bool AddBook(String title, String author, CustomDate pubDate, String publisher, int totalStock, int avalaibleStock, String type)
        {
            bool success = false;
            String date = pubDate.DateToString();

            String cmdString = String.Format("INSERT INTO Books (BOOK_ID, TITLE, AUTHOR, PUBLICATION_DATE, PUBLISHER, TOTAL_STOCK, AVALAIBLE_STOCK, TYPE) " +
                "VALUES( {0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, '{7}' )", GetNextBookIndex(), title, author, date, publisher, totalStock, avalaibleStock, type);
            OracleCommand insertBookCmd = new OracleCommand(cmdString);
            insertBookCmd.Connection = _connection;

            int colNum = insertBookCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
                _numberOfBooks++;
            }
                
            return success;
        }

        public bool DeleteBook(String bookId)
        {
            bool bIsDeleted = false;

            String cmdString = String.Format("DELETE FROM Books WHERE BOOK_ID = {0}", bookId);
            OracleCommand deleteBookCmd = new OracleCommand(cmdString);
            deleteBookCmd.Connection = _connection;

            int colNum = deleteBookCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                bIsDeleted = true;
            }

            return bIsDeleted;
        }
    }
} 

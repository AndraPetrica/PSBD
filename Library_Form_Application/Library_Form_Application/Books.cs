using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Windows.Forms;

namespace Library_Form_Application
{
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

    public class Books:Entity
    {
        public List<Book> books;    

        public Books(OracleConnection conn):base(conn)
        {
            books = new List<Book>();
        }

        public List<Book> GetAllBooks()
        {
            String command = "SELECT * FROM BOOKS";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            books.Clear();

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

        public bool UpdateBook(String bookId, String totalStock, String avalStock, String type)
        {
            bool bIsUpdated = false;
            try
            {
                int total = Int32.Parse(totalStock);
                int aval = Int32.Parse(avalStock);
                int book = Int32.Parse(bookId);
                String cmdString = String.Format("UPDATE Books " +
                    "SET TOTAL_STOCK = {0}, AVALAIBLE_STOCK = {1}, TYPE = '{2}' WHERE BOOK_ID = {3}", total, aval, type, book);
                OracleCommand deleteBookCmd = new OracleCommand(cmdString);
                deleteBookCmd.Connection = _connection;

                int colNum = deleteBookCmd.ExecuteNonQuery();
                if (colNum != 0)
                {
                    bIsUpdated = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return bIsUpdated;
        }

        public List<Book> Search(String title, String author, String type)
        {
            String command = "SELECT * FROM BOOKS WHERE ";
            bool previousSelected = false;

            if(title.CompareTo("") != 0)
            {
                command += String.Format("TITLE = '{0}'", title);
                previousSelected = true;
            }
            if(author.CompareTo("") != 0)
            {
                command += (previousSelected ? " AND " : "");
                command += String.Format("AUTHOR = '{0}'", author);
                previousSelected = true;
            }

            if (type.CompareTo("") != 0)
            {
                command += (previousSelected ? " AND " : "");
                command += String.Format("TYPE = '{0}'", type);
                previousSelected = true;
            }

            if(!previousSelected)
            {
                command = "SELECT * FROM BOOKS";
            }

            command += " ORDER BY TITLE ASC";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            books.Clear();

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
            return books;
        }

        public List<String> GetAuthors()
        {
            List<String> authors = new List<String>();
            String command = "SELECT DISTINCT(AUTHOR) FROM BOOKS ORDER BY AUTHOR ASC";
           
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    authors.Add(dr["AUTHOR"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting authors : " + ex.ToString());
                }
            }

            return authors;
        }

        public List<String> GetTitles()
        {
            List<String> titles = new List<String>();
            String command = "SELECT DISTINCT(TITLE) FROM BOOKS ORDER BY TITLE ASC";

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    titles.Add(dr["TITLE"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting titles : " + ex.ToString());
                }
            }

            return titles;
        }
    }
} 

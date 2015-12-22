using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Library_Form_Application
{
    public class Books:Entity
    {
        private int _numberOfBooks;

        public Books(OracleConnection conn):base(conn)
        {
            _numberOfBooks = 0;
        }

        public String[] GetAllBooks()
        {
            String cmdSring = "SELECT * FROM BOOKS";
            OracleCommand cmd = new OracleCommand(cmdSring,_connection);
            OracleDataReader dr = cmd.ExecuteReader();
            int numEntries = dr.FieldCount;

            if(numEntries > 0 )
            {
                 String[] entries = new String[numEntries];
                 int i = 0;
                 while(dr.Read())
                 {
                     entries[i] = dr[0].ToString() ;
                     ++i;
                 }

                return entries;
            }
         
            return null;
        }


        public bool AddBook(String title, String author, CustomDate pubDate, String publisher, int totalStock, int avalaibleStock, String type)
        {
            bool success = false;
            String date = pubDate.DateToString();

            String cmdString = String.Format("INSERT INTO Books (BOOK_ID, TITLE, AUTHOR, PUBLICATION_DATE, PUBLISHER, TOTAL_STOCK, AVALAIBLE_STOCK, TYPE) " +
                "VALUES( {0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, '{7}' )", _numberOfBooks, title, author, date, publisher, totalStock, avalaibleStock, type);
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
    }
} 

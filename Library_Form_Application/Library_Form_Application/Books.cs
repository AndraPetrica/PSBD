using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Library_Form_Application
{
   public struct PubDate
    {
        String day;
        String month;
        String year;
        public PubDate(String d, String m, String y)
        {
            day = d;
            month = m;
            year = y;
        }
       public String ToStringDate()
        {
            String ret = "to_date('" + day + "-" + month + "-" + year + "','DD-MM-YYYY')";
            return ret;
        }
    }

    public class Books:Entity
    {
        public Books(OracleConnection conn):base(conn)
        {

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


        public bool AddBook(int bookId, String title, String author, PubDate pubDate, String publisher, int totalStock, int avalaibleStock, String type)
        {
            bool success = false;
            String date = pubDate.ToStringDate();

            String cmdString = String.Format("INSERT INTO Books (BOOK_ID, TITLE, AUTHOR, PUBLICATION_DATE, PUBLISHER, TOTAL_STOCK, AVALAIBLE_STOCK, TYPE) " +
                "VALUES( {0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, '{7}' )",bookId, title, author, date, publisher, totalStock, avalaibleStock, type);
            OracleCommand insertBookCmd = new OracleCommand(cmdString);
            insertBookCmd.Connection = _connection;

            int rowsNum = insertBookCmd.ExecuteNonQuery();
            if (rowsNum != 0)
                success = true;

            return success;
        }
    }
} 

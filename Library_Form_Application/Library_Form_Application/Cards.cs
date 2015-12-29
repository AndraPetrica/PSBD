using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Library_Form_Application
{
    class Cards:Entity
    {
        public List<Card> cards;

        public class Card
        {
            public String cardId;
            public String creationDate;
            public String lastValidation;
            public String cnp;
            public String firstName;
            public String lastName;

            public Card(String cardIdC, String creationDateC, String lastValidationC, String cnpC, String firstNameC, String lastNameC)
            {
                cardId = cardIdC;
                creationDate = creationDateC;
                lastValidation = lastValidationC;
                cnp = cnpC;
                firstName = firstNameC;
                lastName = lastNameC;
            }
        }

        public Cards(OracleConnection conn):base(conn)
        {
            cards = new List<Card>();
        }

        public List<Card> GetAllCards()
        {
            String command = "SELECT c.CARD_ID, c.CREATION_DATE, c.LAST_VALIDATION, c.STUDENT_ID, s.FIRST_NAME, S.LAST_NAME " +
                                "FROM CARDS c JOIN STUDENTS s " +
                                "ON c.CARD_ID = s.CNP ";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            cards.Clear();

            while (dr.Read())
            {
                try
                {
                    cards.Add(new Card(
                        dr["CARD_ID"].ToString(),
                        dr["CREATION_DATE"].ToString(),
                        dr["LAST_VALIDATION"].ToString(),
                        dr["STUDENT_ID"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la citirea cartilor : " + ex.ToString());
                }
            }

            return cards;
        }

        private int GetNextCardIndex()
        {
            int index = 0;
            String cmdString = String.Format("SELECT MAX(CARD_ID) MAX_ID FROM CARDS");
            OracleCommand GetMaxIndexBookCmd = new OracleCommand(cmdString, _connection);

            OracleDataReader dr = GetMaxIndexBookCmd.ExecuteReader();
            if (dr.Read())
            {
                index = Int32.Parse(dr["MAX_ID"].ToString()) + 1;
            }

            return index;
        }

        public bool AddCard(String cardId, CustomDate creationDate, String CNP)
        {
            bool success = false;
            String date = creationDate.DateToString();

            String cmdString = String.Format("INSERT INTO CARDS (CARD_ID, CREATION_DATE, LAST_VALIDATION, STUDENT_ID) " +
                "VALUES( {0}, {1}, {2}, '{3}')", GetNextCardIndex(), date, date, CNP);
            OracleCommand insertCardCmd = new OracleCommand(cmdString);
            insertCardCmd.Connection = _connection;

            int colNum = insertCardCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }

            return success;
        }

        public bool DeleteCard(String cardId)
        {
            bool bIsDeleted = false;

            String cmdString = String.Format("DELETE FROM CARDS WHERE CARD_ID = {0}", cardId);
            OracleCommand deleteBookCmd = new OracleCommand(cmdString);
            deleteBookCmd.Connection = _connection;

            int colNum = deleteBookCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                bIsDeleted = true;
            }

            return bIsDeleted;
        }

        public bool UpdateCard(String cardId, CustomDate validationDate)
        {
            bool bIsUpdated = false;
            try
            {
                String date = validationDate.DateToString();
                String cmdString = String.Format("UPDATE Cards " +
                    "SET VALIDATION_DATE = {0} WHERE CARD_ID = {1}", date, cardId);
                OracleCommand deleteCardCmd = new OracleCommand(cmdString);
                deleteCardCmd.Connection = _connection;

                int colNum = deleteCardCmd.ExecuteNonQuery();
                if (colNum != 0)
                {
                    bIsUpdated = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return bIsUpdated;
        }

        /*public List<Card> Search(String title, String author, String type)
        {
            String command = "SELECT * FROM BOOKS WHERE ";
            bool previousSelected = false;

            if (title.CompareTo("") != 0)
            {
                command += String.Format("TITLE = '{0}'", title);
                previousSelected = true;
            }
            if (author.CompareTo("") != 0)
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

            if (!previousSelected)
            {
                command = "SELECT * FROM BOOKS";
            }

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            books.Clear();

            while (dr.Read())
            {
                try
                {
                    books.Add(new Card(
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
        }*/
    }
}

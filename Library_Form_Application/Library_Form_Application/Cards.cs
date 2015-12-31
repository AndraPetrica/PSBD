using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Library_Form_Application
{

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

    class Cards:Entity
    {
        public List<Card> cards;

        public Cards(OracleConnection conn):base(conn)
        {
            cards = new List<Card>();
        }

        public List<Card> GetAllCards()
        {
            String command = "SELECT c.CARD_ID, c.CREATION_DATE, c.LAST_VALIDATION, c.STUDENT_ID, s.FIRST_NAME, S.LAST_NAME " +
                                "FROM CARDS c JOIN STUDENTS s " +
                                "ON c.STUDENT_ID = s.CNP ";
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
                if (dr["MAX_ID"].ToString().CompareTo("") == 0)
                {
                    index = 0;
                }
                else
                {
                    index = Int32.Parse(dr["MAX_ID"].ToString()) + 1;
                }
            }

            return index;
        }

        public bool AddCard(CustomDate creationDate, String CNP)
        {
            bool success = false;
            String date = creationDate.DateToString();

            String cmdString = String.Format("INSERT INTO CARDS (CARD_ID, CREATION_DATE, LAST_VALIDATION, STUDENT_ID) " +
                "VALUES( {0}, {1}, {2}, '{3}')", GetNextCardIndex(), date, date, CNP);
            MessageBox.Show(cmdString);
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
                    "SET LAST_VALIDATION = {0} WHERE CARD_ID = {1}", date, cardId);
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

        public String GetCardIdByCNP(String cnp)
        {
            String cardId = "";
            String command = String.Format("SELECT CARD_ID FROM CARDS WHERE STUDENT_ID = {0}", cnp);
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                try
                {
                    cardId = dr["CARD_ID"].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la citirea cartilor : " + ex.ToString());
                }
            }
            return cardId;
        }

        public List<Card> Search(String name, String date, String studyYear)
        {
            String[] names = name.Split(" ".ToCharArray());
            List<Card> cardsList = new List<Card>();
            bool prevSelected = false;
            String command = "SELECT c.CARD_ID, c.CREATION_DATE, c.LAST_VALIDATION, c.STUDENT_ID, s.FIRST_NAME, S.LAST_NAME " +
                               "FROM CARDS c JOIN STUDENTS s " +
                               "ON c.STUDENT_ID = s.CNP ";
            String predicate = " WHERE ";
            if(name.CompareTo("") != 0)
            {
                predicate += String.Format(" s.FIRST_NAME = '{1}' AND s.LAST_NAME = '{0}'", names[0], names[1]);
                prevSelected = true;
            }

            if(studyYear.CompareTo("") != 0)
            {
                predicate += (prevSelected ? " AND " : "");
                predicate += String.Format(" s.STUDY_YEAR = {0}", studyYear);
                prevSelected = true;
            }

            if(date.CompareTo("") != 0)
            {
                String vDate = new CustomDate(date).DateToString();
                predicate += (prevSelected ? " AND " : "");
                predicate += String.Format(" c.LAST_VALIDATION > {0}", vDate);
                prevSelected = true;
            }

            if( prevSelected )
            {
                command += predicate;
            }

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    cardsList.Add(new Card(
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

            return cardsList;
        }
    }
}

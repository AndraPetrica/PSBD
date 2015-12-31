using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Library_Form_Application
{
    public class Debt
    {
        public String cnp;
        public String cardId;
        public String firstName;
        public String lastName;

        public Debt(String firstNameC, String lastNameC, String CNPC, String cardIdC)
        {
            cnp = CNPC;
            cardId = cardIdC;
            firstName = firstNameC;
            lastName = lastNameC;
        }
    }

    class Debts : Entity
    {
        public List<Debt> debts;

        public Debts(OracleConnection conn) : base(conn)
        {
            debts = new List<Debt>();
        }

        public List<Debt> GetAllDebts()
        {
            String command = "SELECT DISTINCT(l.CARD_ID), c.STUDENT_ID, s.FIRST_NAME, s.LAST_NAME " +
                                "from LOANS l JOIN CARDS c " +
                                  "on l.CARD_ID = c.CARD_ID " +
                                "join STUDENTS s " +
                                  "on c.STUDENT_ID = s.CNP " +
                                "where l.STATUS = 'On Loan'";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            debts.Clear();

            while (dr.Read())
            {
                try
                {
                    debts.Add(new Debt(
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["STUDENT_ID"].ToString(),
                        dr["CARD_ID"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading Loans: " + ex.ToString());
                }
            }

            return debts;
        }

        public List<String> GetListOfBooksByCardId(String cardId, String Status)
        {
            List<String> bookList = new List<String>();

            String command = "SELECT b.TITLE, b.AUTHOR , b.PUBLISHER, b.PUBLICATION_DATE " +
                                "FROM BOOKS b JOIN LOANS L " +
                                "ON b.BOOK_ID = l.BOOK_ID " +
                                "WHERE l.STATUS = '" + Status + "' " +
                                "AND l.CARD_ID = " + cardId;

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            
            while (dr.Read())
            {
                try
                {
                    bookList.Add(
                        dr["TITLE"].ToString() + " - " +
                        dr["AUTHOR"].ToString() + " - " +
                        dr["PUBLISHER"].ToString() + "@" +
                        dr["PUBLICATION_DATE"].ToString().Split(" ".ToCharArray())[0]
                        );
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading debts: " + ex.ToString());
                }
            }

            return bookList;
        }

        public List<Debt> Search(String cardId, String bookId, String studyYear)
        {
            String command = "SELECT DISTINCT(l.CARD_ID), c.STUDENT_ID, s.FIRST_NAME, s.LAST_NAME " +
                               "from LOANS l JOIN CARDS c " +
                                 "on l.CARD_ID = c.CARD_ID " +
                               "join STUDENTS s " +
                                 "on c.STUDENT_ID = s.CNP " +
                               "WHERE l.STATUS = 'On Loan'";
            String restriction = " AND ";
            bool prevSelected = false;

            if(cardId.CompareTo("") != 0)
            {
                restriction += " c.CARD_ID = " + cardId;
                prevSelected = true;
            }
            if(bookId.CompareTo("") != 0)
            {
                restriction += (prevSelected ? " AND " : "");
                restriction += " l.BOOK_ID = " + bookId;
                prevSelected = true;
            }
            if (studyYear.CompareTo("") != 0)
            {
                restriction += (prevSelected ? " AND " : "");
                restriction += " s.STUDY_YEAR = " + studyYear;
                prevSelected = true;
            }
            if(prevSelected)
            {
                command += restriction;
            }
            MessageBox.Show(command);
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            List<Debt> debtsList = new List<Debt>();

            while (dr.Read())
            {
                try
                {
                    debtsList.Add(new Debt(
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["STUDENT_ID"].ToString(),
                        dr["CARD_ID"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading Loans: " + ex.ToString());
                }
            }

            return debtsList;
        }

    }
  
}

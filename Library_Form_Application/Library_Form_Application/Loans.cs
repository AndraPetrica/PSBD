using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Library_Form_Application
{
    public class Loan
    {
        public String loanId;
        public String status;
        public String loanDate;
        public String returnDate;
        public String cardId;
        public String bookId;
        public String firstName;
        public String lastName;
        public String bookTitle;
        public String bookAuthor;

        public Loan(String loanIdC, String statusC, String loanDateC, String returnDateC, String cardIdC, String bookIdC, String firstNameC, String lastNameC, String bookTitleC, String bookAuthorC)
        {
            loanId = loanIdC;
            status = statusC;
            loanDate = loanDateC;
            returnDate = returnDateC;
            cardId = cardIdC;
            bookId = bookIdC;
            firstName = firstNameC;
            lastName = lastNameC;
            bookTitle = bookTitleC;
            bookAuthor = bookAuthorC;
        }
    }

    class Loans : Entity
    {
        public List<Loan> loans;

        public Loans(OracleConnection conn) : base(conn)
        {
            loans = new List<Loan>();
            InitLoanTriggers();
        }

        public List<Loan> GetAllLoans()
        {
            String command = "SELECT l.LOAN_ID, l.LOAN_DATE, l.RETURN_DATE, l.STATUS, l.CARD_ID, l.BOOK_ID, c.STUDENT_ID, s.FIRST_NAME, s.LAST_NAME, b.TITLE, b.AUTHOR " +
                                "FROM LOANS l JOIN CARDS c " + 
                                    "ON l.CARD_ID = c.CARD_ID " +
                                    "JOIN STUDENTS s ON c.STUDENT_ID = s.CNP " +
                                    "JOIN BOOKS b ON b.BOOK_ID = l.BOOK_ID";
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            loans.Clear();

            while (dr.Read())
            {
                try
                {
                    loans.Add(new Loan(
                        dr["LOAN_ID"].ToString(),
                        dr["STATUS"].ToString(),
                        dr["LOAN_DATE"].ToString(),
                        dr["RETURN_DATE"].ToString(),
                        dr["CARD_ID"].ToString(),
                        dr["BOOK_ID"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["TITLE"].ToString(),
                        dr["AUTHOR"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading Loans: " + ex.ToString());
                }
            }

            return loans;
        }

        private int GetNextLoanIndex()
        {
            int index = 0;
            String cmdString = String.Format("SELECT MAX(LOAN_ID) MAX_ID FROM LOANS");
            OracleCommand GetMaxIndexLoanCmd = new OracleCommand(cmdString, _connection);

            OracleDataReader dr = GetMaxIndexLoanCmd.ExecuteReader();
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

        public bool AddLoan(String bookid, String CNP)
        {
            bool success = false;
            CustomDate date = new CustomDate(DateTime.Now.ToString("dd/MM/yyyy"));

            String cmdString = String.Format("INSERT INTO LOANS (LOAN_ID, LOAN_DATE, STATUS, CARD_ID, BOOK_ID) " +
                "VALUES( {0}, {1}, '{2}', (SELECT CARD_ID FROM CARDS WHERE STUDENT_ID = {3}), {4})", GetNextLoanIndex(), date.DateToString(), "On Loan", CNP, bookid);
            MessageBox.Show(cmdString);
            OracleCommand insertLoanCmd = new OracleCommand(cmdString);
            insertLoanCmd.Connection = _connection;

            int colNum = insertLoanCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }
            return success;
        }

        public bool UpdateLoan(String loanId, String status)
        {
            bool bIsUpdated = false;
            try
            {
                String cmdString = String.Format("UPDATE Loans " +
                    "SET STATUS = '{0}' WHERE LOAN_ID = {1}", status, loanId);
                MessageBox.Show(cmdString);
                OracleCommand updateLoanCmd = new OracleCommand(cmdString);
                updateLoanCmd.Connection = _connection;

                int colNum = updateLoanCmd.ExecuteNonQuery();
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
                    MessageBox.Show("Error getting card id by cnp : " + ex.ToString());
                }
            }
            return cardId;
        }

        public List<Loan> Search(String cardId, String bookId, String status, String date)
        {
            String command = "SELECT l.LOAN_ID, l.LOAN_DATE, l.RETURN_DATE, l.STATUS, l.CARD_ID, l.BOOK_ID, c.STUDENT_ID, s.FIRST_NAME, s.LAST_NAME, b.TITLE, b.AUTHOR " +
                                   "FROM LOANS l JOIN CARDS c " +
                                       "ON l.CARD_ID = c.CARD_ID " +
                                       "JOIN STUDENTS s ON c.STUDENT_ID = s.CNP " +
                                       "JOIN BOOKS b ON b.BOOK_ID = l.BOOK_ID";

            bool prevSelected = false;
            String restrictions = " WHERE ";
            if(cardId.CompareTo("") != 0)
            {
                restrictions += "c.CARD_ID = " + cardId;
                prevSelected = true;
            }
            if(bookId.CompareTo("") != 0)
            {
                restrictions += (prevSelected ? " AND " : "");
                restrictions += " b.BOOK_ID = " + bookId;
                prevSelected = true;
            }
            if(status.CompareTo("") != 0)
            {
                restrictions += (prevSelected ? " AND " : "");
                restrictions += String.Format(" l.status = '{0}' ", status);
                prevSelected = true;
            }
            if(date.CompareTo("") != 0)
            {
                String dateS = new CustomDate(date).DateToString();
                restrictions += (prevSelected ? " AND " : "");
                restrictions += String.Format(" l.RETURN_DATE > {0} ", dateS);
                prevSelected = true;
            }
            if(prevSelected)
            {
                command += restrictions;
            }
           
            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            List<Loan> loanList = new List<Loan>();

            while (dr.Read())
            {
                try
                {
                    loanList.Add(new Loan(
                        dr["LOAN_ID"].ToString(),
                        dr["STATUS"].ToString(),
                        dr["LOAN_DATE"].ToString(),
                        dr["RETURN_DATE"].ToString(),
                        dr["CARD_ID"].ToString(),
                        dr["BOOK_ID"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["TITLE"].ToString(),
                        dr["AUTHOR"].ToString()
                        ));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading Loans: " + ex.ToString());
                }
            }
            loans = loanList;
            return loanList;
        }

        private void InitLoanTriggers()
        {

        }
    }
}

using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Library_Form_Application
{
    class Penalization
    {
        public string penalization_id;
        public string sum;
        public string status;
        public string card_id;
        public string first_name;
        public string last_name;

        public Penalization(string penalId, string s, string stat, string cardId, string fName, string lName)
        {
            penalization_id = penalId;
            sum = s;
            status = stat;
            card_id = cardId;
            first_name = fName;
            last_name = lName;
        }
    }

    class Penalizations:Entity
    {
        public List<Penalization> penalizations;

        public Penalizations(OracleConnection conn) : base(conn)
        {
            penalizations = new List<Penalization>();
        }

        public List<Penalization> GetAllPenalizations()
        {
            List<Penalization> penalizationsList = new List<Penalization>();

            String cmdSring = "SELECT p.PENALIZATION_ID, p.SUM, p.STATUS, p.CARD_ID, s.FIRST_NAME, s.LAST_NAME " +
                                  "FROM PENALIZATIONS p JOIN CARDS c " +
                                    "ON p.CARD_ID = c.CARD_ID " +
                                    "JOIN STUDENTS s " +
                                    "ON c.STUDENT_ID = s.CNP" ;
            OracleCommand cmd = new OracleCommand(cmdSring, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    Penalization stud = new Penalization(dr["PENALIZATION_ID"].ToString(),
                        dr["SUM"].ToString(),
                        dr["STATUS"].ToString(),
                        dr["CARD_ID"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString()
                        );

                    penalizationsList.Add(stud);
                }
                catch (Exception exc)
                {
                    System.Windows.Forms.MessageBox.Show("Penalization could not be added to the list \n" + exc.Message);
                }
            }

            penalizations = penalizationsList;
            return penalizations;
        }

        private int GetNextPenalizationIndex()
        {
            int index = 0;
            String cmdString = String.Format("SELECT MAX(PENALIZATION_ID) MAX_ID FROM PENALIZATIONS");
            OracleCommand getMaxIndexCommand = new OracleCommand(cmdString, _connection);

            OracleDataReader dr = getMaxIndexCommand.ExecuteReader();
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

        public bool AddPenalization(string cnp, string sum)
        {
            bool success = false;

            int iSum = Convert.ToInt32(sum);

            String cmdString = String.Format("INSERT INTO PENALIZATIONS (PENALIZATION_ID, SUM, STATUS, CARD_ID) " +
                "VALUES( {0}, {1}, '{2}', (SELECT CARD_ID FROM CARDS WHERE STUDENT_ID = '{3}') )", GetNextPenalizationIndex(), iSum, "UnPaid", cnp);
            OracleCommand insertPenalizationCmd = new OracleCommand(cmdString);
            insertPenalizationCmd.Connection = _connection;

            int colNum = insertPenalizationCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }

            return success;
        }

        public bool DeletePenalization(string penalizationId)
        {
            bool success = false;
            int iPenalizationId = Convert.ToInt32(penalizationId);

            String cmdString = String.Format("DELETE FROM Penalizations WHERE PENALIZATION_ID = {0}", iPenalizationId);
            OracleCommand deletePenalizationCmd = new OracleCommand(cmdString);
            deletePenalizationCmd.Connection = _connection;

            int colNum = deletePenalizationCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }

            return success;
        }

        public bool UpdatePenalization(String penalizationId, String sum, String status)
        {
            bool success = false;
            try
            {
                int iSum = Convert.ToInt32(sum);
                int iPenalizationId = Convert.ToInt32(penalizationId);

                String cmdString = String.Format("UPDATE Penalizations " +
                    "SET SUM = {0}, STATUS = '{1}' WHERE PENALIZATION_ID = {2}", iSum, status, iPenalizationId);
                OracleCommand updatePenalizationCmd = new OracleCommand(cmdString);
                updatePenalizationCmd.Connection = _connection;

                int colNum = updatePenalizationCmd.ExecuteNonQuery();
                if (colNum != 0)
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return success;
        }

        public List<Penalization> Search(String cnp, String status, String sum)
        {

            List<Penalization> penalizationsList = new List<Penalization>();

            String cmdSring = "SELECT p.PENALIZATION_ID, p.SUM, p.STATUS, p.CARD_ID, s.FIRST_NAME, s.LAST_NAME " +
                                  "FROM PENALIZATIONS p JOIN CARDS c " +
                                    "ON p.CARD_ID = c.CARD_ID " +
                                    "JOIN STUDENTS s " +
                                    "ON c.STUDENT_ID = s.CNP";

            String restriction = " WHERE ";
            bool prevSelected = false;
            if (cnp.CompareTo("") != 0)
            {
                restriction += " s.CNP = '" + cnp + "' ";
                prevSelected = true;
            }
            if (status.CompareTo("") != 0)
            {
                restriction += (prevSelected ? " AND " : "");
                restriction += " p.STATUS = '" + status + "' ";
                prevSelected = true;
            }
            if (sum.CompareTo("") != 0)
            {
                restriction += (prevSelected ? " AND " : "");
                if(sum.CompareTo("0") == 0)
                {
                    restriction += " p.SUM <= 50 ";
                }
                else if (sum.CompareTo("1") == 0)
                {
                    restriction += " p.SUM > 50 AND p.SUM < 100";
                }
                else
                {
                    restriction += " p.SUM >= 100 ";
                }
                prevSelected = true;
            }

            if(prevSelected)
            {
                cmdSring += restriction;
            }

            OracleCommand cmd = new OracleCommand(cmdSring, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    Penalization stud = new Penalization(dr["PENALIZATION_ID"].ToString(),
                        dr["SUM"].ToString(),
                        dr["STATUS"].ToString(),
                        dr["CARD_ID"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString()
                        );

                    penalizationsList.Add(stud);
                }
                catch (Exception exc)
                {
                    System.Windows.Forms.MessageBox.Show("Penalization could not be added to the list \n" + exc.Message);
                }
            }

            penalizations = penalizationsList;
            return penalizationsList;
        }
    }
}

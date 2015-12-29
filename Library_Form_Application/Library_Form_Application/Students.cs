using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Windows.Forms;

namespace Library_Form_Application
{
    class Student
    {
        public string CNP;
        public string first_name;
        public string last_name;
        public string birth_date;
        public string address;
        public string phone;
        public string email;
        public string gender;
        public string study_year;

        public Student(string cnp, string fName, string lName, string bDate, string addr, string phoneNo, string emailAddr, string gend, string studyYear)
        {
            CNP = cnp;
            first_name = fName;
            last_name = lName;
            birth_date = bDate;
            address = addr;
            phone = phoneNo;
            email = emailAddr;
            gender = gend;
            study_year = studyYear;
        }
    }

    class Students:Entity
    {
        public List<Student> students;

        public Students(OracleConnection conn) : base(conn)
        {
            students = new List<Student>();
        }

        public List<Student> GetAllStudents()
        {
            List<Student> studentsList = new List<Student>();

            String cmdSring = "SELECT * FROM STUDENTS";
            OracleCommand cmd = new OracleCommand(cmdSring, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    Student stud = new Student(dr["CNP"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["BIRTH_DATE"].ToString(),
                        dr["ADDRESS"].ToString(),
                        dr["PHONE"].ToString(),
                        dr["EMAIL"].ToString(),
                        dr["GENDER"].ToString(),
                        dr["STUDY_YEAR"].ToString()
                        );

                    studentsList.Add(stud);
                }
                catch(Exception exc)
                {
                    System.Windows.Forms.MessageBox.Show("Student could not be added to the list \n" + exc.Message);
                }
            }

            students = studentsList;
            return students;
        }

        public bool AddStudent(string cnp, string fName, string lName, string bDate, string addr, string phoneNo, string emailAddr, string gend, string studyYear)
        {
            bool success = false;

            CustomDate birth_date = new CustomDate(bDate);
            String date = birth_date.DateToString();
            int study_year = Convert.ToInt32(studyYear);

            String cmdString = String.Format("INSERT INTO Students (CNP, FIRST_NAME, LAST_NAME, BIRTH_DATE, ADDRESS, PHONE, EMAIL, GENDER, STUDY_YEAR) " +
                "VALUES( '{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', {8} )", cnp, fName, lName, date, addr, phoneNo, emailAddr, gend, study_year);
            OracleCommand insertStudentCmd = new OracleCommand(cmdString);
            insertStudentCmd.Connection = _connection;

            int colNum = insertStudentCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }

            return success;
        }

        public bool DeleteStudent(string cnp)
        {
            bool success = false;

            String cmdString = String.Format("DELETE FROM Students WHERE CNP = '{0}'", cnp);
            OracleCommand deleteStudentCmd = new OracleCommand(cmdString);
            deleteStudentCmd.Connection = _connection;

            int colNum = deleteStudentCmd.ExecuteNonQuery();
            if (colNum != 0)
            {
                success = true;
            }

            return success;
        }

        public bool UpdateStudent(String cnp, String birthDate, String address, String phone, String email, String studyYear)
        {
            bool success = false;
            try
            {
                int study_year = Convert.ToInt32(studyYear);
                CustomDate birth = new CustomDate(birthDate);
                String birth_date = birth.DateToString();

                String cmdString = String.Format("UPDATE Students " +
                    "SET BIRTH_DATE = {0}, ADDRESS = '{1}', PHONE = '{2}', EMAIL = '{3}', STUDY_YEAR = {4} WHERE CNP = '{5}'", birth_date, address, phone, email, study_year, cnp);
                OracleCommand updateStudentCmd = new OracleCommand(cmdString);
                updateStudentCmd.Connection = _connection;

                int colNum = updateStudentCmd.ExecuteNonQuery();
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

        public List<Student> Search(String fName, String lName, String studyYear)
        {
            String command = "SELECT * FROM STUDENTS WHERE ";
            bool previousSelected = false;

            if (fName.CompareTo("") != 0)
            {
                command += String.Format("FIRST_NAME = '{0}'", fName);
                previousSelected = true;
            }
            if (lName.CompareTo("") != 0)
            {
                command += (previousSelected ? " AND " : "");
                command += String.Format("LAST_NAME = '{0}'", lName);
                previousSelected = true;
            }

            if (studyYear.CompareTo("") != 0)
            {
                int study_year = Convert.ToInt32(studyYear);
                command += (previousSelected ? " AND " : "");
                command += String.Format("STUDY_YEAR = {0}", study_year);
                previousSelected = true;
            }

            if (!previousSelected)
            {
                command = "SELECT * FROM STUDENTS";
            }

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();
            students.Clear();

            while (dr.Read())
            {
                try
                {
                    students.Add(new Student(
                        dr["CNP"].ToString(),
                        dr["FIRST_NAME"].ToString(),
                        dr["LAST_NAME"].ToString(),
                        dr["BIRTH_DATE"].ToString(),
                        dr["ADDRESS"].ToString(),
                        dr["PHONE"].ToString(),
                        dr["EMAIL"].ToString(),
                        dr["GENDER"].ToString(),
                        dr["STUDY_YEAR"].ToString()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading students : " + ex.ToString());
                }
            }

            return students;
        }

        public List<String> GetFirstNames()
        {
            List<String> fNames = new List<String>();
            String command = "SELECT DISTINCT(FIRST_NAME) FROM STUDENTS";

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    fNames.Add(dr["FIRST_NAME"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting first names : " + ex.ToString());
                }
            }

            return fNames;
        }

        public List<String> GetLastNames()
        {
            List<String> lNames = new List<String>();
            String command = "SELECT DISTINCT(LAST_NAME) FROM STUDENTS";

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    lNames.Add(dr["LAST_NAME"].ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting last names : " + ex.ToString());
                }
            }

            return lNames;
        }

        public String GetNameByCNP(String cnp)
        {
            String name = "";
            String command = String.Format("SELECT FIRST_NAME, LAST_NAME FROM STUDENTS WHERE CNP = '{0}'", cnp);

            OracleCommand cmd = new OracleCommand(command, _connection);
            OracleDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                try
                {
                    name = dr["FIRST_NAME"].ToString() + dr["LAST_NAME"].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error getting last names : " + ex.ToString());
                }
            }

            return name;
        }
    }
} 

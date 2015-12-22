using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

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
        public List<Student> _students;

        public Students(OracleConnection conn) : base(conn)
        {
            _students = new List<Student>();
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();

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

                    students.Add(stud);
                }
                catch(Exception exc)
                {
                    System.Windows.Forms.MessageBox.Show("Student could not be added to the list \n" + exc.Message);
                }
            }

            _students = students;
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
    }
}

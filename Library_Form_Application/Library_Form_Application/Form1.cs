using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Connection;
using Oracle.DataAccess.Client;


namespace Library_Form_Application
{
    public partial class Form1 : Form
    {
        enum adaptersIndexes
        {
            studentsIndex,
            booksIndex,
            cardsIndex,
            penalizationsIndex,
            debtsIndex,
            loansIndex,
            returnsIndex
        }

        public Form1()
        {
            InitializeComponent();
        }

        #region Connection

        private Connection.Connection _connection;
        private OracleConnection _oracleConn;
        private Entity[] _adapters;

        public bool InitOracleDbConnection()
        {
            bool bRet = true;
            try
            {
                _connection = new Connection.Connection();
                _oracleConn = _connection.GetConnectionObject();
                bRet = _connection.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bRet;
        }

        public bool Login()
        {
            bool bRet = _connection.Login("user1", "user1");
            String message;
            message = (bRet == true ? "Utilizator logat" : "Utilizator nelogat");
            MessageBox.Show(message);
            return bRet;
        }

        private void InitAdapters()
        {
            _adapters = new Entity[7];
            _adapters[(int)adaptersIndexes.booksIndex] = new Books(_oracleConn);
            _adapters[(int)adaptersIndexes.studentsIndex] = new Students(_oracleConn);
          
        }

        #endregion Connection

        #region Main
        private void LoadData(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            String tab = tc.SelectedTab.Text;
            DisableEditElements();
            if (tab.CompareTo("Students") == 0)
            {
                LoadAllStudents();
            }
            else if (tab.CompareTo("Books") == 0)
            {
                LoadAllBooks();
            }
            else if (tab.CompareTo("Cards") == 0)
            {
                LoadAllCards();
            }
            else if (tab.CompareTo("Penalizations") == 0)
            {

            }
            else if (tab.CompareTo("Debts") == 0)
            {

            }
            else if (tab.CompareTo("Loans") == 0)
            {

            }
            else if (tab.CompareTo("Returns") == 0)
            {
            }
        }
        private void DisableEditElements()
        {
            //Books
            BooksEditGroup.Enabled = false;
            BooksEditButton.Enabled = false;
            BooksSaveButton.Enabled = false;
            BooksDeleteButton.Enabled = false;

            //Students
            StudentsEditGroup.Enabled = false;
            StudentsEditButton.Enabled = false;
            StudentsSaveButton.Enabled = false;
            StudentsDeleteButton.Enabled = false;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            bool isInitializedOK = InitOracleDbConnection();
            if ( true == isInitializedOK)
            {
                Login();
                InitAdapters();
                DisableEditElements();
            }
            
        }

        #endregion Main

        #region Books

        private void LoadAllBooks()
        {
            Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
            List<Book> books = booksAdapter.GetAllBooks();

            listBoxBooks.Items.Clear();
            foreach (Book book in books)
            {
                listBoxBooks.Items.Add(book.title + " " + book.author + " " + book.pubDate.Split(' ')[0] );
            }

            List<String> authors = booksAdapter.GetAuthors();
            BooksAuthorCmB.Items.Clear();
            foreach(String author in authors)
            {
                BooksAuthorCmB.Items.Add(author);
            }

            List<String> titles = booksAdapter.GetTitles();
            BooksTitleCmB.Items.Clear();
            foreach(String title in titles)
            {
                BooksTitleCmB.Items.Add(title);
            }

            BooksTypeCmB.Items.Clear();
            BooksTypeCmB.Items.Add("home");
            BooksTypeCmB.Items.Add("library");
            BooksAddTypeCmB.Items.Clear();
            BooksAddTypeCmB.Items.Add("home");
            BooksAddTypeCmB.Items.Add("library");
            BooksEditTypeCmB.Items.Clear();
            BooksEditTypeCmB.Items.Add("home");
            BooksEditTypeCmB.Items.Add("library");
        }

        private void AddBook(object sender, EventArgs e)
        {
            try
            {
                String title = BooksAddTitleTb.Text;
                String author = BooksAddAuthorTb.Text;
                String publisher = BooksAddPublisherTb.Text;
                String type = BooksAddTypeCmB.Text;
                int totalStock = Int32.Parse(BooksAddTotalStockTb.Text);
                int avalaibleStock = Int32.Parse(BooksAddAvalaibleStockTb.Text);
                String[] lines = BooksAddPublicationDateTb.Text.Split("-./".ToCharArray());
                CustomDate date = new CustomDate(lines[0], lines[1], lines[2]);

                Books booksAdapter = (Books)_adapters[(int)adaptersIndexes.booksIndex];
                bool ret = booksAdapter.AddBook(title, author, date, publisher, totalStock, avalaibleStock, type);
                String message = (ret ? "Book added !" : "Book not added : Fill all fields !");
                MessageBox.Show(message);
                LoadAllBooks();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteBook(object sender, EventArgs e)
        {
            try
            {
                Books library = (Books)_adapters[(int)adaptersIndexes.booksIndex];
                int index = listBoxBooks.SelectedIndex;
                Book entry = library.books[index];
                bool bIsDeleted = library.DeleteBook(entry.bookId);
                String message = (bIsDeleted ? "Book successfully deleted" : "Book not deleted");
                MessageBox.Show(message);
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare deleting book : " + ex.ToString());
            }
        }

        private void EditBook(object sender, EventArgs e)
        {
            BooksEditGroup.Enabled = true;
            BooksSaveButton.Enabled = true;
        }

        private void SaveBookChanges(object sender, EventArgs e)
        {
            try
            {
                Books library = (Books)_adapters[(int)adaptersIndexes.booksIndex];
                int index = listBoxBooks.SelectedIndex;
                Book entry = library.books[index];
                bool isUpdated = library.UpdateBook(entry.bookId, BooksEditTotalStockTB.Text, BooksEditAvalStockTB.Text, BooksEditTypeCmB.Text);
                String message = (isUpdated ? "Book successfully updated" : "Book not updated");
                MessageBox.Show(message);
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating book: " + ex.Message);
            }
        }

        private void OnSelectBook(object sender, EventArgs e)
        {
            try
            {
                BooksEditButton.Enabled = true;
                Books library = (Books)_adapters[(int)adaptersIndexes.booksIndex];
                int index = listBoxBooks.SelectedIndex;
                Book entry = library.books[index];
                BooksEditTotalStockTB.Text = entry.totalStock;
                BooksEditAvalStockTB.Text = entry.avalaibleStock;
                BooksEditTypeCmB.SelectedText = entry.type;
                BooksDeleteButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void SearchBooks(object sender, EventArgs e)
        {
            String title = "";
            String author = "";
            String type = "";
            if (BooksTitleCB.Checked == true)
            {
                title = BooksTitleCmB.Text;
            }
            if (BooksAuthorCB.Checked == true)
            {
                author = BooksAuthorCmB.Text;
            }
            if (BooksTypeCB.Checked == true)
            {
                type = BooksTypeCmB.Text;
            }

            Books library = (Books)_adapters[(int)adaptersIndexes.booksIndex];

            List<Book> books = library.Search(title, author, type);

            listBoxBooks.Items.Clear();
            foreach (Book book in books)
            {
                listBoxBooks.Items.Add(book.title + " " + book.author + " " + book.pubDate.Split(' ')[0]);
            }

        }

        #endregion Books

        #region Students

        private void LoadAllStudents()
        {
            Students studentsAdapter = ((Students)_adapters[(int)adaptersIndexes.studentsIndex]);
            List<Student> students = studentsAdapter.GetAllStudents();

            listBoxStudents.Items.Clear();
            foreach (Student stud in students)
            {
                listBoxStudents.Items.Add(stud.first_name + " " + stud.last_name + " " + stud.CNP);
            }

            List<String> firstNames = studentsAdapter.GetFirstNames();
            StudentsFNameCmB.Items.Clear();
            foreach (String fName in firstNames)
            {
                StudentsFNameCmB.Items.Add(fName);
            }

            List<String> lastNames = studentsAdapter.GetLastNames();
            StudentsLNameCmB.Items.Clear();
            foreach (String lName in lastNames)
            {
                StudentsLNameCmB.Items.Add(lName);
            }

            StudentsStudyYearCmB.Items.Clear();
            StudentsStudyYearCmB.Items.Add("1");
            StudentsStudyYearCmB.Items.Add("2");
            StudentsStudyYearCmB.Items.Add("3");
            StudentsStudyYearCmB.Items.Add("4");

            StudentsEditStudyYearCmB.Items.Clear();
            StudentsEditStudyYearCmB.Items.Add("1");
            StudentsEditStudyYearCmB.Items.Add("2");
            StudentsEditStudyYearCmB.Items.Add("3");
            StudentsEditStudyYearCmB.Items.Add("4");

            studentsAddStudyYearCmB.Items.Clear();
            studentsAddStudyYearCmB.Items.Add("1");
            studentsAddStudyYearCmB.Items.Add("2");
            studentsAddStudyYearCmB.Items.Add("3");
            studentsAddStudyYearCmB.Items.Add("4");
        }

        private void AddStudent(object sender, EventArgs e)
        {
            try
            {
                String CNP = studentsAddCNPTb.Text;
                String firstName = studentsAddFirstNameTb.Text;
                String lastName = studentsAddLastNameTb.Text;
                String birthDate = studentsAddBirthDateTb.Text;
                String address = studentsAddAddressTb.Text;
                String phone = studentsAddPhoneTb.Text;
                String email = studentsAddEmailTb.Text;
                String yearOfStudy = studentsAddStudyYearCmB.Text;
                String gender = studentsAddGenderTb.Text;

                Students studentsAdapter = (Students)_adapters[(int)adaptersIndexes.studentsIndex];
                bool success = studentsAdapter.AddStudent(CNP, firstName, lastName, birthDate, address, phone, email, gender, yearOfStudy);

                if (success)
                {
                    LoadAllStudents();
                    ClearAddStudentFields();
                }
                
                String message = (success ? "Student added !" : "Student not added : Fill all fields !");
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteStudent(object sender, EventArgs e)
        {
            try
            {
                int index = listBoxStudents.SelectedIndex;
                Students students = (Students)_adapters[(int)adaptersIndexes.studentsIndex];
                Student stud = students.students[index];
                bool success = students.DeleteStudent(stud.CNP);

                if (success)
                {
                    LoadAllStudents();
                    ClearAddStudentFields();
                }

                String message = (success ? "Student deleted !" : "Student not deleted");
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearAddStudentFields()
        {
            studentsAddCNPTb.Text = "";
            studentsAddFirstNameTb.Text = "";
            studentsAddLastNameTb.Text = "";
            studentsAddBirthDateTb.Text = "";
            studentsAddAddressTb.Text = "";
            studentsAddPhoneTb.Text = "";
            studentsAddEmailTb.Text = "";
            studentsAddStudyYearCmB.Text = "";
            studentsAddGenderTb.Text = "";
        }
        private void EditStudent(object sender, EventArgs e)
        {
            StudentsEditGroup.Enabled = true;
            StudentsSaveButton.Enabled = true;
        }

        private void SaveStudentChanges(object sender, EventArgs e)
        {
            try
            {
                Students studentsAdapter = (Students)_adapters[(int)adaptersIndexes.studentsIndex];
                int index = listBoxStudents.SelectedIndex;
                Student entry = studentsAdapter.students[index];
                bool isUpdated = studentsAdapter.UpdateStudent(entry.CNP, StudentsEditBirthDateTB.Text, StudentsEditAddressTB.Text, StudentsEditPhoneTB.Text, StudentsEditEmailTB.Text, StudentsEditStudyYearCmB.Text);
                String message = (isUpdated ? "Student successfully updated" : "Student not updated");
                MessageBox.Show(message);
                LoadAllStudents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating student: " + ex.Message);
            }
        }

        private void SearchStudents(object sender, EventArgs e)
        {
            String fName = "";
            String lName = "";
            String studyYear = "";
            if (StudentsFNameCB.Checked == true)
            {
                fName = StudentsFNameCmB.Text;
            }
            if (StudentsLNameCB.Checked == true)
            {
                lName = StudentsLNameCmB.Text;
            }
            if (StudentsStudyYearCB.Checked == true)
            {
                studyYear = StudentsStudyYearCmB.Text;
            }

            Students studentsAdapter = (Students)_adapters[(int)adaptersIndexes.studentsIndex];

            List<Student> students = studentsAdapter.Search(fName, lName, studyYear);

            listBoxStudents.Items.Clear();
            foreach (Student stud in students)
            {
                listBoxStudents.Items.Add(stud.first_name + " " + stud.last_name + " " + stud.CNP);
            }
        }

        private void OnSelectStudent(object sender, EventArgs e)
        {
            try
            {
                StudentsEditButton.Enabled = true;
                Students studentsAdapter = (Students)_adapters[(int)adaptersIndexes.studentsIndex];
                int index = listBoxStudents.SelectedIndex;
                Student entry = studentsAdapter.students[index];

                StudentsEditBirthDateTB.Text = entry.birth_date.Split(' ')[0];
                StudentsEditAddressTB.Text = entry.address;
                StudentsEditPhoneTB.Text = entry.phone;
                StudentsEditEmailTB.Text = entry.email;
                StudentsEditStudyYearCmB.Text = entry.study_year;

                StudentsDeleteButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion Students

        #region Cards

        private void LoadAllCards()
        {

        }

        private void AddCard(object sender, EventArgs e)
        {

        }

        private void OnSelectCard(object sender, EventArgs e)
        {

        }

        private void EditCard(object sender, EventArgs e)
        {

        }

        private void SaveCardChanges(object sender, EventArgs e)
        {

        }

        private void DeleteCard(object sender, EventArgs e)
        {

        }

        #endregion Cards

    }
}

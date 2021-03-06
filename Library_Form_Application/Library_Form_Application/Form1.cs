﻿using System;
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
            loansIndex
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
            if(!bRet)
            {
                MessageBox.Show("Connection problems");
            }
            return bRet;
        }

        private void InitAdapters()
        {
            _adapters = new Entity[6];
            _adapters[(int)adaptersIndexes.booksIndex] = new Books(_oracleConn);
            _adapters[(int)adaptersIndexes.studentsIndex] = new Students(_oracleConn);
            _adapters[(int)adaptersIndexes.cardsIndex] = new Cards(_oracleConn);
            _adapters[(int)adaptersIndexes.penalizationsIndex] = new Penalizations(_oracleConn);
            _adapters[(int)adaptersIndexes.loansIndex] = new Loans(_oracleConn);
            _adapters[(int)adaptersIndexes.debtsIndex] = new Debts(_oracleConn);      
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
                LoadAllPenalizations();
            }
            else if (tab.CompareTo("Debts") == 0)
            {
                LoadAllDebts();
            }
            else if (tab.CompareTo("Loans") == 0)
            {
                LoadAllLoans();
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

            //Cards
            CardsEditGroup.Enabled = false;
            CardsEditButton.Enabled = false;
            CardsDeleteButton.Enabled = false;
            CardsSaveButton.Enabled = false;

            //Penalizations
            PenalizationsEditGroup.Enabled = false;
            PenalizationsSaveButton.Enabled = false;
            PenalizationsEditButton.Enabled = false;
            PenalizationsDeleteButton.Enabled = false;

            //Loans
            LoansEditGroup.Enabled = false;
            LoansEditButton.Enabled = false;
            LoansSaveButton.Enabled = false;


        }

        private void OnLoad(object sender, EventArgs e)
        {
            try
            {
                bool isInitializedOK = InitOracleDbConnection();
                if (true == isInitializedOK)
                {
                    Login();
                    InitAdapters();
                    DisableEditElements();
                    LoadAllStudents();
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ClearAddFields()
        {
            //Students
            studentsAddCNPTb.Text = "";
            studentsAddFirstNameTb.Text = "";
            studentsAddLastNameTb.Text = "";
            studentsAddBirthDateTb.Text = "";
            studentsAddAddressTb.Text = "";
            studentsAddPhoneTb.Text = "";
            studentsAddEmailTb.Text = "";
            studentsAddStudyYearCmB.Text = "";
            studentsAddGenderTb.Text = "";

            //Books
            BooksAddTitleTb.Text = "";
            BooksAddAuthorTb.Text = "";
            BooksAddPublicationDateTb.Text = "";
            BooksAddPublisherTb.Text = "";
            BooksAddTotalStockTb.Text = "";
            BooksAddAvalaibleStockTb.Text = "";
            BooksAddTypeCmB.Text = "";

            //Cards
            CardsAddCreationDateTb.Text = "";

            //Penalizations
            PenalizationsAddSumTB.Text = "";

            //Debts

            //Loans

            //Returns
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
                listBoxBooks.Items.Add(book.title + " - " + book.author + " " + book.avalaibleStock + "/" + book.totalStock + " - " + book.pubDate.Split(' ')[0] );
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
                    ClearAddFields();
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
                    ClearAddFields();
                }

                String message = (success ? "Student deleted !" : "Student not deleted");
                MessageBox.Show(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            Cards cardsAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
            Students studentsAdapter = ((Students)_adapters[(int)adaptersIndexes.studentsIndex]);

            List<Card> cards = cardsAdapter.GetAllCards();
            List<Student> names = studentsAdapter.GetAllStudents();

            listBoxCards.Items.Clear();
            CardsAddNameCmB.Items.Clear();
            CardsStudentNameCmB.Items.Clear();
            CardsStudyYearCmb.Items.Clear();

            foreach (Card card in cards)
            {
                listBoxCards.Items.Add(card.lastName + " " + card.firstName + " " + card.lastValidation.Split(" ".ToCharArray())[0]);
            }

            foreach (Student s in names)
            {
                CardsAddNameCmB.Items.Add(s.last_name + " " + s.first_name);
                CardsStudentNameCmB.Items.Add(s.last_name + " " + s.first_name);
            }

            CardsStudyYearCmb.Items.Add("1");
            CardsStudyYearCmb.Items.Add("2");
            CardsStudyYearCmb.Items.Add("3");
            CardsStudyYearCmb.Items.Add("4");
        }

        private void AddCard(object sender, EventArgs e)
        {
            try
            {
                String cnp = CardsAddCNPCmB.Text;
                CustomDate date = new CustomDate(CardsAddCreationDateTb.Text);

                Cards cardsAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
                bool ret = cardsAdapter.AddCard(date, cnp);
                String message = (ret ? "Card added !" : "Card not added : Fill all fields !");
                MessageBox.Show(message);
                LoadAllCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSelectCard(object sender, EventArgs e)
        {
            try
            {
                BooksEditButton.Enabled = true;
                Cards cardAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
                int index = listBoxCards.SelectedIndex;
                Card entry = cardAdapter.cards[index];
                CardsEditValidDateTb.Text = entry.lastValidation.Split(" ".ToCharArray())[0];
                CardsEditButton.Enabled = true;
                CardsDeleteButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void EditCard(object sender, EventArgs e)
        {
            CardsEditGroup.Enabled = true;
            CardsSaveButton.Enabled = true;
        }

        private void SaveCardChanges(object sender, EventArgs e)
        {
            try
            {
                CustomDate date = new CustomDate(CardsEditValidDateTb.Text);
                Cards cardAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
                int index = listBoxCards.SelectedIndex;
                Card entry = cardAdapter.cards[index];
                bool bIsUpdated = cardAdapter.UpdateCard(entry.cardId, date);
                String message = (bIsUpdated ? "Card successfully updated" : "Card not updated");
                MessageBox.Show(message);
                LoadAllCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare updating card : " + ex.ToString());
            }
        }

        private void DeleteCard(object sender, EventArgs e)
        {
            try
            {
                Cards cardAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
                int index = listBoxCards.SelectedIndex;
                Card entry = cardAdapter.cards[index];
                bool bIsDeleted = cardAdapter.DeleteCard(entry.cardId);
                String message = (bIsDeleted ? "Card successfully deleted" : "Card not deleted");
                MessageBox.Show(message);
                LoadAllCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare deleting card : " + ex.ToString());
            }
        }

        private void OnCardsNameChanged(object sender, EventArgs e)
        {
            Students studentsAdapter = ((Students)_adapters[(int)adaptersIndexes.studentsIndex]);
            String[] names = CardsAddNameCmB.Text.Split(" ".ToCharArray());
            List<String> CNPs = studentsAdapter.GetCNPListByName(names[1], names[0]);
            CardsAddCNPCmB.Items.Clear();

            foreach (String cnp in CNPs)
            {
                CardsAddCNPCmB.Items.Add(cnp);
            }
        }

        private void SearchCards(object sender, EventArgs e)
        {
            Cards cardsAdapter = (Cards)_adapters[(int)adaptersIndexes.cardsIndex];
            String name = "";
            String date = "";
            String year = "";

            if(CardsStudentNameCB.Checked)
            {
                name = CardsStudentNameCmB.Text;
            }
            if(CardsValidationDateCB.Checked)
            {
                date = CardsValidationDateCmB.Text;
            }
            if(CardsStudyYearCB.Checked)
            {
                year = CardsStudyYearCmb.Text;
            }

            List<Card> cards = cardsAdapter.Search(name, date, year);
            
            listBoxCards.Items.Clear();
           
            foreach (Card card in cards)
            {
                listBoxCards.Items.Add(card.lastName + " " + card.firstName + " " + card.lastValidation.Split(" ".ToCharArray())[0]);
            }
        }

        #endregion Cards

        #region Penalizations

        private void LoadAllPenalizations()
        {
            Penalizations penalizationsAdapter = ((Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex]);
            List<Penalization> penalizations = penalizationsAdapter.GetAllPenalizations();

            listBoxPenalizations.Items.Clear();
            foreach (Penalization penalization in penalizations)
            {
                listBoxPenalizations.Items.Add(penalization.sum + " " + penalization.status + " " + penalization.first_name + " " + penalization.last_name);
            }

            PenalizationsStatusCmB.Items.Clear();
            PenalizationsStatusCmB.Items.Add("Paid");
            PenalizationsStatusCmB.Items.Add("Unpaid");

            PenalizationsEditStatusCmB.Items.Clear();
            PenalizationsEditStatusCmB.Items.Add("Paid");
            PenalizationsEditStatusCmB.Items.Add("Unpaid");

            PenalizationsSumCmB.Items.Clear();
            PenalizationsSumCmB.Items.Add("<50");
            PenalizationsSumCmB.Items.Add("50-100");
            PenalizationsSumCmB.Items.Add(">100");

            //Populate add combos
            Cards cardAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
            List<Card> names = cardAdapter.GetAllCards();

            PenalizationsAddNameCmB.Items.Clear();
            foreach (Card s in names)
            {
                PenalizationsAddNameCmB.Items.Add(s.lastName + " " + s.firstName);
                PenalizationsStudNameCmB.Items.Add(s.lastName + " " + s.firstName);
            }

        }

        private void EditPenalization(object sender, EventArgs e)
        {
            PenalizationsEditGroup.Enabled = true;
            PenalizationsSaveButton.Enabled = true;
        }

        private void SavePenalizationChanges(object sender, EventArgs e)
        {
            try
            {
                Penalizations penalizationsAdapter = (Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex];
                int index = listBoxPenalizations.SelectedIndex;
                Penalization entry = penalizationsAdapter.penalizations[index];
                bool isUpdated = penalizationsAdapter.UpdatePenalization(entry.penalization_id, PenalizationsEditSumTB.Text, PenalizationsEditStatusCmB.Text);
                String message = (isUpdated ? "Penalization successfully updated" : "Penalization not updated");
                MessageBox.Show(message);
                LoadAllPenalizations();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating penalization: " + ex.Message);
            }
        }

        private void AddPenalization(object sender, EventArgs e)
        {
            try
            {
                Penalizations penalizationsAdapter = ((Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex]);
                bool succes = penalizationsAdapter.AddPenalization(PenalizationsAddCNPCmB.Text, PenalizationsAddSumTB.Text);
                String message = (succes ? "Penalization added" : "Penalization not added");
                MessageBox.Show(message);
                ClearAddFields();
                LoadAllPenalizations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeletePenalization(object sender, EventArgs e)
        {
            try
            {
                Penalizations penalizationsAdapter = (Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex];
                int index = listBoxPenalizations.SelectedIndex;
                Penalization entry = penalizationsAdapter.penalizations[index];
                bool isUpdated = penalizationsAdapter.DeletePenalization(entry.penalization_id);
                String message = (isUpdated ? "Penalization successfully deleted" : "Penalization not deleted");
                MessageBox.Show(message);
                LoadAllPenalizations();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting penalization: " + ex.Message);
            }
        }

        private void SearchPenalizations(object sender, EventArgs e)
        {
            String cnp = "";
            String status = "";
            String sum = "";
            if(PenalizationsStudNameCB.Checked)
            {
                int index = PenalizationsStudNameCmB.SelectedIndex;
                Cards cardAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
                cnp = cardAdapter.cards[index].cnp;
            }
            if(PenalizationsStatusCB.Checked)
            {
                status = PenalizationsStatusCmB.Text;
            }
            if(PenalizationsSumCB.Checked)
            {
                sum = PenalizationsSumCmB.SelectedIndex.ToString();
            }

            Penalizations penalizationsAdapter = ((Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex]);
            List<Penalization> penalizations = penalizationsAdapter.Search(cnp, status, sum);

            listBoxPenalizations.Items.Clear();
            foreach (Penalization penalization in penalizations)
            {
                listBoxPenalizations.Items.Add(penalization.sum + " " + penalization.status + " " + penalization.first_name + " " + penalization.last_name);
            }

        }

        private void OnPenalizationNameChanged(object sender, EventArgs e)
        {
            Students studentsAdapter = ((Students)_adapters[(int)adaptersIndexes.studentsIndex]);
            String[] names = PenalizationsAddNameCmB.Text.Split(" ".ToCharArray());
            List<String> CNPs = studentsAdapter.GetCNPListByName(names[1], names[0]);
            PenalizationsAddCNPCmB.Items.Clear();

            foreach (String cnp in CNPs)
            {
                PenalizationsAddCNPCmB.Items.Add(cnp);
            }
        }
        
        private void OnSelectPenalization(object sender, EventArgs e)
        {
            try
            {
                PenalizationsEditButton.Enabled = true;
                Penalizations penalizationsAdapter = (Penalizations)_adapters[(int)adaptersIndexes.penalizationsIndex];
                int index = listBoxPenalizations.SelectedIndex;
                Penalization entry = penalizationsAdapter.penalizations[index];

                PenalizationsEditStatusCmB.Text = entry.status;
                PenalizationsEditSumTB.Text = entry.sum;
                PenalizationsDeleteButton.Enabled = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion Penalizations

        #region Debts

        private void LoadAllDebts()
        {
            Debts debtsAdapter = ((Debts)_adapters[(int)adaptersIndexes.debtsIndex]);
            List<Debt> debts = debtsAdapter.GetAllDebts();

            listBoxDebts.Items.Clear();
            listBoxDebtsBooks.Items.Clear();
            DebtsEditFirstName.Text = "";
            DebtsEditLastName.Text = "";

            foreach (Debt debt in debts)
            {
                listBoxDebts.Items.Add(debt.firstName + " " + debt.lastName);
            }

            DebtsStudyYearCmB.Items.Clear();
            DebtsStudyYearCmB.Items.Add("1");
            DebtsStudyYearCmB.Items.Add("2");
            DebtsStudyYearCmB.Items.Add("3");
            DebtsStudyYearCmB.Items.Add("4");

            Cards cardsAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
            List<Card> cards = cardsAdapter.GetAllCards();
            DebtsStudentNameCmB.Items.Clear();
            foreach ( Card c in cards)
            {
                DebtsStudentNameCmB.Items.Add(c.firstName + " " + c.lastName + " " + c.cnp);
            }

            Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
            List<Book> books = booksAdapter.GetAllBooks();
            DebtsBookCmB.Items.Clear();

            foreach (Book book in books)
            {
                DebtsBookCmB.Items.Add(book.title + " - " + book.author + "  - " + book.publisher + "@" + book.pubDate.Split(" ".ToCharArray())[0]);
            }
        }

        private void OnDebtSelected(object sender, EventArgs e)
        {
            int index = listBoxDebts.SelectedIndex;
            Debts debtsAdapter = ((Debts)_adapters[(int)adaptersIndexes.debtsIndex]);

            if (index < debtsAdapter.debts.Count)
            {
                Debt debt = debtsAdapter.debts[index];
                DebtsEditFirstName.Text = debt.firstName;
                DebtsEditLastName.Text = debt.lastName;

                listBoxDebtsBooks.Items.Clear();
                List<String> books = debtsAdapter.GetListOfBooksByCardId(debt.cardId, "On Loan");

                foreach (String book in books)
                {
                    listBoxDebtsBooks.Items.Add(book);
                }
            }
        }

        private void SearchDebts(object sender, EventArgs e)
        {
            String cardId = "";
            String book = "";
            String year = "";
            
            if(DebtsStudentNameCB.Checked)
            {
                int index = DebtsStudentNameCmB.SelectedIndex;               
                Cards cardsAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
                cardId = cardsAdapter.cards[index].cardId;
            }
            if(DebtsBookCB.Checked)
            {
                int index = DebtsBookCmB.SelectedIndex;
                Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
                book = booksAdapter.books[index].bookId;
            }
            if(DebtsStudyYearCB.Checked)
            {
                year = DebtsStudyYearCmB.Text;
            }

            Debts debtsAdapter = ((Debts)_adapters[(int)adaptersIndexes.debtsIndex]);
            List<Debt> debts = debtsAdapter.Search(cardId, book, year);

            listBoxDebts.Items.Clear();
            listBoxDebtsBooks.Items.Clear();
            DebtsEditFirstName.Text = "";
            DebtsEditLastName.Text = "";

            foreach (Debt debt in debts)
            {
                listBoxDebts.Items.Add(debt.firstName + " " + debt.lastName);
            }
        }

        #endregion Debts


        #region Loans

        private void LoadAllLoans()
        {
            Cards cardsAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
            List<Card> cards = cardsAdapter.GetAllCards();
            Loans loansAdapter = ((Loans)_adapters[(int)adaptersIndexes.loansIndex]);
            List<Loan> loans = loansAdapter.GetAllLoans();
            Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
            List<Book> books = booksAdapter.GetAllBooks();

            listBoxLoans.Items.Clear();

            //main listbox
            foreach(Loan loan in loans)
            {
                listBoxLoans.Items.Add(loan.firstName + " " + loan.lastName + " - " + loan.bookTitle + ", " + loan.bookAuthor + " - loan : " + loan.loanDate.Split(" ".ToCharArray())[0]);
            }

            //Add loan group
            foreach (Card card in cards)
            {
                LoansAddNameCmB.Items.Add(card.lastName + " " + card.firstName);
            }

            // if returned/lost then trigger
            LoansEditStatusCmB.Items.Clear();
            LoansEditStatusCmB.Items.Add("On Loan");
            LoansEditStatusCmB.Items.Add("Returned");
            LoansEditStatusCmB.Items.Add("Lost");

            //Search Group
            LoansStudentNameCmB.Items.Clear();
            foreach (Card card in cards)
            {
                LoansStudentNameCmB.Items.Add(card.lastName + " " + card.firstName);
            }
            
            LoansBookCmB.Items.Clear();
            foreach (Book book in books)
            {
                LoansBookCmB.Items.Add(book.title + " - " + book.author + "  - " + book.publisher + " - " + book.pubDate.Split(" ".ToCharArray())[0]);
            }

            LoansStatusCmB.Items.Clear();
            LoansStatusCmB.Items.Add("On Loan");
            LoansStatusCmB.Items.Add("Returned");
            LoansStatusCmB.Items.Add("Lost");


        }

        private void AddLoan(object sender, EventArgs e)
        {
            try
            {
                int indexCNP = LoansAddCNPCmB.SelectedIndex;
                Cards cardsAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
                Card card = cardsAdapter.cards[indexCNP];

                Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
                int indexBook = LoansAddBookCmB.SelectedIndex;
                Book book = booksAdapter.books[indexBook];

                Loans loansAdapter = ((Loans)_adapters[(int)adaptersIndexes.loansIndex]);
                bool loandIsAdded = loansAdapter.AddLoan(book.bookId, card.cnp);
                String message = (loandIsAdded ? "Book on loan" : "Book not on loan");
                MessageBox.Show(message);
                LoadAllLoans();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error adding loan : " + ex.ToString());
            }
             
        }

        private void SaveLoanChanges(object sender, EventArgs e)
        {
            try
            {
                Loans loansAdapter = ((Loans)_adapters[(int)adaptersIndexes.loansIndex]);
                int index = listBoxLoans.SelectedIndex;
                Loan loan = loansAdapter.loans[index];
                bool loandIsUpdated = loansAdapter.UpdateLoan(loan.loanId, LoansEditStatusCmB.Text);
                String message = (loandIsUpdated ? "Loan Updated" : "Loan not updated");
                MessageBox.Show(message);
                LoadAllLoans();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding loan : " + ex.ToString());
            }
        }

        private void EditLoan(object sender, EventArgs e)
        {
            LoansEditGroup.Enabled = true;
            LoansSaveButton.Enabled = true;
        }

        private void OnLoanSelected(object sender, EventArgs e)
        {
            LoansEditButton.Enabled = true;
            Loans loansAdapter = ((Loans)_adapters[(int)adaptersIndexes.loansIndex]);
            int index = listBoxLoans.SelectedIndex;
            if(index < loansAdapter.loans.Count)
            {
                Loan loan = loansAdapter.loans[index];
                LoansEditStatusCmB.Text = loan.status;
            }
           
        }

        private void OnLoanNameSelected(object sender, EventArgs e)
        {
            Students studentsAdapter = ((Students)_adapters[(int)adaptersIndexes.studentsIndex]);
            String[] names = LoansAddNameCmB.Text.Split(" ".ToCharArray());
            List<String> CNPs = studentsAdapter.GetCNPListByName(names[1], names[0]);
            LoansAddCNPCmB.Items.Clear();

            foreach (String cnp in CNPs)
            {
                LoansAddCNPCmB.Items.Add(cnp);
            }
        }

        private void OnLoanCnpSelected(object sender, EventArgs e)
        {
            Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
            List<Book> books = booksAdapter.GetAllBooks();
            LoansAddBookCmB.Items.Clear();

            foreach ( Book book in books)
            {
                LoansAddBookCmB.Items.Add(book.title + " - " + book.author + "  - " + book.publisher + " - " + book.pubDate.Split(" ".ToCharArray())[0]);
            }
        }

        private void SearchLoans(object sender, EventArgs e)
        {
            String student = "";
            String date = "";
            String book = "";
            String status = "";

            if(LoansStudNameCB.Checked)
            {
                int index = LoansStudentNameCmB.SelectedIndex;
                Cards cardsAdapter = ((Cards)_adapters[(int)adaptersIndexes.cardsIndex]);
                Card card = cardsAdapter.cards[index];
                student = card.cardId;
            }
            if(LoansReturnDateCB.Checked)
            {
                date = LoansReturnDateCmB.Text;
            }
            if(LoansBookCB.Checked)
            {
                //Get index and book id 
                int index = LoansBookCmB.SelectedIndex;
                Books booksAdapter = ((Books)_adapters[(int)adaptersIndexes.booksIndex]);
                Book b = booksAdapter.books[index];
                book = b.bookId;
            }
            if(LoansStatusCB.Checked)
            {
                status = LoansStatusCmB.Text;
            }

            Loans loansAdapter = ((Loans)_adapters[(int)adaptersIndexes.loansIndex]);
            List<Loan> loans = loansAdapter.Search(student, book, status, date);

            listBoxLoans.Items.Clear();

            //main listbox
            foreach (Loan loan in loans)
            {
                listBoxLoans.Items.Add(loan.firstName + " " + loan.lastName + " - " + loan.bookTitle + ", " + loan.bookAuthor + " - loan : " + loan.loanDate.Split(" ".ToCharArray())[0]);
            }           
        }



        #endregion Loans


    }
}

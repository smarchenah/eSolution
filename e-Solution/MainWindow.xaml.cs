using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace e_Solution
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Entities db;
        int _currentQuestionId;
        public List<Item> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();            
        }

        #region Events        

        /// <summary>
        /// Show the Home panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeMenu_Click(object sender, RoutedEventArgs e)
        {
            goToHomePanel();
        }

        /// <summary>
        /// Show the Admin panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdminMenu_Click(object sender, RoutedEventArgs e)
        {
            goToAdminPanel();
        }

        /// <summary>
        /// Show the User panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserMenu_Click(object sender, RoutedEventArgs e)
        {
            goToUserPanel();
        }

        /// <summary>
        /// Show the Statistic Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsMenu_Click(object sender, RoutedEventArgs e)
        {
            goToStatisticPanel();
        }

        /// <summary>
        /// Change the color of the text in Red
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbRed_Click(object sender, RoutedEventArgs e)
        {
            txtQuestion.Foreground = Brushes.Red;
        }

        /// <summary>
        /// Change the color of the text in Black
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbBlack_Click(object sender, RoutedEventArgs e)
        {
            txtQuestion.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Change the color of the text in Blue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbBlue_Click(object sender, RoutedEventArgs e)
        {
            txtQuestion.Foreground = Brushes.Blue;
        }

        /// <summary>
        /// Save the question in the DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendQuestion_Click(object sender, RoutedEventArgs e)
        {
            SendQuestion();
        }

        /// <summary>
        /// Initialized the values in the Admin panel for a new question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewQuestion_Click(object sender, RoutedEventArgs e)
        {
            NewQuestion();
        }

        /// <summary>
        /// Save the answer in the DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendAnswer_Click(object sender, RoutedEventArgs e)
        {
            SendAnswerToQuestion();
        }

        /// <summary>
        /// Load the Detail for a question
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgQuestionsAdmin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgQuestionsAdmin.SelectedItem != null)
            {
                var type = dgQuestionsAdmin.SelectedItem.GetType();
                var questionId = 0;
                if (type.FullName == "e_Solution.eQuestion")
                {
                    questionId = ((eQuestion)dgQuestionsAdmin.SelectedItem).id;
                }
                else if (type.FullName == "e_Solution.Questions")
                {
                    questionId = ((Questions)dgQuestionsAdmin.SelectedItem).id;
                }
                LoadDetailQuestion(questionId);
                LoadNotAnsweredQuestions();
            }
        }

        private void dgQuestionsNotAnswered_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var questionId = ((eQuestion)dgQuestionsNotAnswered.SelectedItem).id;
            LoadDetailQuestion(questionId);
            EnableControlsUserPanel(true);
        }

        private void dgUserAnswers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var questionId = ((eQuestion)dgUserAnswers.SelectedItem).id;
            LoadDetailQuestion(questionId);
        }

        private void lbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var userId = ((Item)lbUsers.SelectedItem).userId;
            lblSelectedUser.Content = ((Item)lbUsers.SelectedItem).Name;

            sliderQuestionsAnswered.Value = double.Parse(getUserAnswers(userId));
            sliderQuestionsAnswered.Maximum = db.Questions.Count();
            sliderQuestionsNotAnswered.Value = getNotAnsweredQuestions();
            sliderQuestionsNotAnswered.Maximum = db.Questions.Count();
            sliderUrgentQuestions.Value = double.Parse(getUrgentQuestions());
            sliderUrgentQuestions.Maximum = db.Questions.Count();
            sliderRedQuestions.Value = double.Parse(getRedTextQuestions());
            sliderRedQuestions.Maximum = db.Questions.Count();
            sliderBlackQuestions.Value = double.Parse(getBlackTextQuestions());
            sliderBlackQuestions.Maximum = db.Questions.Count();
            sliderBlueQuestions.Value = double.Parse(getBlueTextQuestions());
            sliderBlueQuestions.Maximum = db.Questions.Count();
            sliderTotalQuestions.Value = db.Questions.Count();
            sliderTotalQuestions.Maximum = db.Questions.Count();

            LoadUserAnswers(userId);
        }

        #endregion

        private string getUrgentQuestions()
        {
            db = new Entities();
            var numUrgentQuestions = from question in db.Questions
                                     where question.urgent == true
                                     select question;


            return numUrgentQuestions.Count().ToString();
        }

        private string getRedTextQuestions()
        {
            db = new Entities();
            var numRedTextQuestions = from question in db.Questions
                                      where question.textColor == "Red"
                                      select question;
            return numRedTextQuestions.Count().ToString();
        }

        private string getBlackTextQuestions()
        {
            db = new Entities();
            var numBlackTextQuestions = from question in db.Questions
                                        where question.textColor == "Black"
                                        select question;
            return numBlackTextQuestions.Count().ToString();
        }

        private string getBlueTextQuestions()
        {
            db = new Entities();
            var numBlueTextQuestions = from question in db.Questions
                                       where question.textColor == "Blue"
                                       select question;
            return numBlueTextQuestions.Count().ToString();
        }
        /// <summary>
        /// Set the controls available or not 
        /// </summary>
        /// <param name="isEnabled"></param>
        private void EnableControlsUserPanel(bool isEnabled)
        {
            txtAnswer.IsReadOnly = !isEnabled;
            txtFirstName.IsReadOnly = !isEnabled;
            txtLastName.IsReadOnly = !isEnabled;
            btnSendAnswer.IsEnabled = isEnabled;
            if (isEnabled)
            {
                txtAnswer.Text = String.Empty;
                txtFirstName.Text = String.Empty;
                txtLastName.Text = String.Empty;
            }
        }

        private void goToHomePanel()
        {
            homePanel.Visibility = Visibility.Visible;
            adminPanel.Visibility = Visibility.Hidden;
            userPanel.Visibility = Visibility.Hidden;
            statisticsPanel.Visibility = Visibility.Hidden;            
        }

        /// <summary>
        /// Show the Admin panel
        /// </summary>
        private void goToAdminPanel()
        {
            homePanel.Visibility = Visibility.Hidden;
            adminPanel.Visibility = Visibility.Visible;
            userPanel.Visibility = Visibility.Hidden;
            statisticsPanel.Visibility = Visibility.Hidden;
            LoadQuestions();
            if (dgQuestionsAdmin.Items.Count <= 0)
            {
                openQuestionsPanel.Visibility = Visibility.Hidden;
            }            
        }

        /// <summary>
        /// Show the User panel
        /// </summary>
        private void goToUserPanel()
        {
            homePanel.Visibility = Visibility.Hidden;
            adminPanel.Visibility = Visibility.Hidden;
            userPanel.Visibility = Visibility.Visible;
            statisticsPanel.Visibility = Visibility.Hidden;            
            LoadNotAnsweredQuestions();
            EnableControlsUserPanel(false);
        }

        /// <summary>
        /// Show the Statistic Panel
        /// </summary>
        private void goToStatisticPanel()
        {
            homePanel.Visibility = Visibility.Hidden;
            adminPanel.Visibility = Visibility.Hidden;
            userPanel.Visibility = Visibility.Hidden;
            statisticsPanel.Visibility = Visibility.Visible;            
            lblStatisticsInfo.Content = "Bitte, wählen Sie a Benutzer auf der linken Seite, um die spezifischen Statistiken anzuzeigen";
            LoadUsers();            
        }

        private void LoadUserAnswers(int userId)
        {
            db = new Entities();
            List<eQuestion> listQuestions = new List<eQuestion>();
            if (db.Questions.Count() > 0)
            {
                listQuestions = AddDataUserAnswers(userId);
            }
            else
            {
                lblOpenQuestions.Visibility = Visibility.Hidden;
                dgUserAnswers.Visibility = Visibility.Hidden;
            }
            dgUserAnswers.ItemsSource = null;
            dgUserAnswers.ItemsSource = listQuestions;            
        }

        private List<eQuestion> AddDataUserAnswers(int userId)
        {
            db = new Entities();
            List<eQuestion> questions = new List<eQuestion>();

            var listQuestions = from question in db.Questions
                                where question.userId == userId
                                select question;

            foreach (Questions q in listQuestions)
            {
                questions.Add(new eQuestion
                {
                    id = q.id,
                    importance = q.importance,
                    urgent = q.urgent,
                    answer = getAnswerById(q.answerId),
                    textColor = q.textColor,
                    date = getDateAnswer(q.answerId)
                });
            }
            return questions;
        }

        private DateTime getDateAnswer(int? answerId)
        {
            DateTime answer = new DateTime();
            var listAnswers = from a in db.Answers
                              where a.id == answerId
                              select a;
            foreach (Answers a in listAnswers)
            {
                answer = a.date;
            }
            return answer;
        }

        private string getAnswerById(int? answerId)
        {
            var answer = string.Empty;
            var listAnswers = from a in db.Answers
                              where a.id == answerId
                              select a;
            foreach (Answers a in listAnswers)
            {
                answer = a.answer;
            }
            return answer;
        }

        /// <summary>
        /// Load the datagrid with all the non-answered questions
        /// </summary>
        private void LoadNotAnsweredQuestions()
        {
            db = new Entities();            
            List<eQuestion> listQuestions = new List<eQuestion>();
            if (db.Questions.Count() > 0)
            {
                listQuestions = AddDataNoAnsweredQuestions();
            }
            else
            {
                lblOpenQuestions.Visibility = Visibility.Hidden;
                dgQuestionsAdmin.Visibility = Visibility.Hidden;
            }
            dgQuestionsNotAnswered.ItemsSource = null;
            dgQuestionsNotAnswered.ItemsSource = listQuestions;
            if (listQuestions.Count == 0)
            {
                lblNoQuestionsAvailable.Visibility = Visibility.Visible;
                dgQuestionsNotAnswered.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Load all the questions
        /// </summary>
        private void LoadQuestions()
        {
            db = new Entities();
            List<eQuestion> listQuestions = new List<eQuestion>();
            if (db.Questions.Count() > 0)            
                listQuestions = CreateColumns();            
            else
            {
                lblOpenQuestions.Visibility = Visibility.Hidden;
                dgQuestionsAdmin.Visibility = Visibility.Hidden;
            }
            dgQuestionsAdmin.ItemsSource = null;
            dgQuestionsAdmin.ItemsSource = listQuestions;              
        }

        private List<eQuestion> CreateColumns()
        {
            db = new Entities();
            List<eQuestion> questions = new List<eQuestion>();                       

            var listQuestions = from question in db.Questions
                                orderby question.date
                                select question;

            foreach (Questions q in listQuestions)
            {
                var myUser = (q.userId != null) ? getUser((int)q.userId) : null;
                questions.Add(new eQuestion{ id = q.id,
                    importance = q.importance,
                    urgent = q.urgent,
                    question = q.question,
                    textColor = q.textColor,
                    date = q.date,
                    user = myUser});                                
            }
            return questions;
        }        

        /// <summary>
        /// Load Questions in User Panel
        /// </summary>
        /// <param name="dt"></param>
        private List<eQuestion> AddDataNoAnsweredQuestions()
        {
            db = new Entities();
            List<eQuestion> questions = new List<eQuestion>();

            var listQuestions = from question in db.Questions
                                where question.answerId == null && question.userId == null
                                select question;

            foreach (Questions q in listQuestions)
            {
                questions.Add(new eQuestion
                {
                    id = q.id,
                    importance = q.importance,
                    urgent = q.urgent,
                    question = q.question,
                    textColor = q.textColor,
                    date = DateTime.Parse(q.date.ToShortDateString()),
                    user = null
                });
            }
            return questions;
        }

        /// <summary>
        /// Get the description of the answer
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        private string getAnswer(int answerId)
        {
            var answer = String.Empty;
            var listAnswer = from a in db.Answers
                             where a.id == answerId
                             select a;

            foreach (Answers a in listAnswer)
            {
                answer = a.answer;
            }
            return answer;
        }

        /// <summary>
        /// Get the name of the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string getUser(int userId)
        {
            var user = String.Empty;
            var listUsers = from u in db.Users
                            where u.id == userId
                            select u;

            foreach (Users u in listUsers)
            {
                user = u.firstName + " " + u.lastName;
            }
            return user;
        }

        /// <summary>
        /// Save the question in the DB
        /// </summary>
        private void SendQuestion()
        {
            try
            {
                db = new Entities();                
                var importance = int.Parse(sliderImportance.Value.ToString());
                var urgent = toggleUrgency.IsChecked;
                var question = txtQuestion.Text;
                var color = "";
                if ((bool)rbRed.IsChecked)
                    color = "Red";
                else if ((bool)rbBlack.IsChecked)
                    color = "Black";
                else if ((bool)rbBlue.IsChecked)
                    color = "Blue";

                if (areAdminFieldsInformed())
                {
                    Questions questions = new Questions();
                    questions.importance = importance;
                    questions.urgent = (bool)urgent;
                    questions.question = question;
                    questions.textColor = color;
                    questions.date = DateTime.Now;
                    db.Questions.Add(questions);
                    db.SaveChanges();

                    LoadQuestions();
                    LoadNotAnsweredQuestions();
                    lblOpenQuestions.Visibility = Visibility.Visible;
                    dgQuestionsAdmin.Visibility = Visibility.Visible;
                    openQuestionsPanel.Visibility = Visibility.Visible;
                    lblErrorInfoAdmin.Visibility = Visibility.Hidden;
                }
                else
                {
                    lblErrorInfoAdmin.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                db.Dispose();
                throw ex;
            }
        }      
        
        private bool areAdminFieldsInformed()
        {
            bool result = false;
            if (txtQuestion.Text != String.Empty && (rbBlack.IsChecked == true || rbBlue.IsChecked == true || rbRed.IsChecked == true))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Initialized the values of the Admin Panel for a new question
        /// </summary>
        private void NewQuestion()
        {
            try
            {
                sliderImportance.Value = 0;
                toggleUrgency.IsChecked = false;
                txtQuestion.Text = String.Empty;
                rbRed.IsChecked = false;
                rbBlack.IsChecked = false;
                rbBlue.IsChecked = false;
                lblErrorInfoAdmin.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }                

        /// <summary>
        /// Save the User and Answer in the DB and Update the Question information
        /// </summary>
        private void SendAnswerToQuestion()
        {
            try
            {
                var firstName = txtFirstName.Text;
                var lastName = txtLastName.Text;
                var answer = txtAnswer.Text;
                                                
                if (areUserFieldsInformed())
                {
                    var q = from question in db.Questions
                            where question.id == _currentQuestionId
                            select question;

                    var userId = InsertUserIfNotExist();

                    //Insert the answer in the DB
                    var answerId = InsertAnswer();

                    //Update my question with the new answer and user name
                    foreach (Questions myQuestion in q)
                    {
                        myQuestion.userId = userId;
                        myQuestion.answerId = answerId;
                    }
                    db.SaveChanges();
                    LoadNotAnsweredQuestions();
                    lblErrorInfoUser.Visibility = Visibility.Hidden;
                }
                else
                    lblErrorInfoUser.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                db.Dispose();
                throw ex;                
            }
        }

        private bool areUserFieldsInformed()
        {
            bool result = false;
            if (txtAnswer.Text != String.Empty && txtFirstName.Text != String.Empty && txtLastName.Text != String.Empty)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Insert the Answer in the DB
        /// </summary>
        /// <returns></returns>
        private int InsertAnswer()
        {
            Answers ans = new Answers();
            ans.answer = txtAnswer.Text;
            ans.date = DateTime.Now;
            db.Answers.Add(ans);
            db.SaveChanges();
            var answerId = ans.id;
            return answerId;
        }

        /// <summary>
        /// Check if the user exist. If not, insert the new user
        /// </summary>
        /// <returns></returns>
        private int InsertUserIfNotExist()
        {
            bool isFound = false;
            var userId = 0;
            var listUsers = from user in db.Users
                            select user;

            foreach (Users user in listUsers)
            {
                if (user.firstName == txtFirstName.Text && user.lastName == txtLastName.Text)
                {
                    isFound = true;
                    userId = user.id;
                    break;
                }
            }
            if (!isFound)
            {
                Users u = new Users();
                u.firstName = txtFirstName.Text;
                u.lastName = txtLastName.Text;
                db.Users.Add(u);
                db.SaveChanges();
                userId = u.id;
            }                
            return userId;
        }        

        private int getNotAnsweredQuestions()
        {
            db = new Entities();
            var listNotAnsweredQuestions = from question in db.Questions
                                           where question.userId == null
                                           select question;

            return listNotAnsweredQuestions.Count();
        }

        /// <summary>
        /// Get the userId from a userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private int getUserId(string userName)
        {
            var userId = 0;
            var listUsers = from user in db.Users
                            select user;

            foreach (Users user in listUsers)
            {
                if (userName == user.firstName + " " + user.lastName)
                {
                    userId = user.id;
                    break;
                }
            }
            return userId;
        }

        /// <summary>
        /// Get the number of questions answered by user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string getUserAnswers(int userId)
        {
            var listQuestionsPerUser = from question in db.Questions
                                       where question.userId == userId
                                       select question;
            return listQuestionsPerUser.Count().ToString();
        }

        /// <summary>
        /// Load all the users for the ComboBox
        /// </summary>
        private void LoadUsers()
        {
            var listUsers = from user in db.Users
                            select user;
            if (listUsers.Count() > 0)
            {
                LoadListBoxUsers();
            }
            else
            {
                lbUsers.Visibility = Visibility.Hidden;
                sliderQuestionsAnswered.Value = 0;
                sliderQuestionsNotAnswered.Value = 0;
                sliderUrgentQuestions.Value = 0;
                sliderRedQuestions.Value = 0;
                sliderBlackQuestions.Value = 0;
                sliderBlueQuestions.Value = 0;
                sliderTotalQuestions.Value = 0;
                lblStatisticsInfo.Content = "There is not a user information.";
            }
        }

        private void LoadListBoxUsers()
        {
            Items = new List<Item>();
            db = new Entities();
            var listUsers = from user in db.Users
                           select user;

            foreach (Users user in listUsers)
            {
                Item ItemToAdd = new Item();
                ItemToAdd.userId = user.id;
                ItemToAdd.Name = user.firstName + " " + user.lastName;             
                Items.Add(ItemToAdd);
            }

            lbUsers.ItemsSource = Items;
        }        

        private void LoadDetailQuestion(int questionId)
        {
            try
            {
                db = new Entities();
                goToUserPanel();
                txtAnswer.Text = String.Empty;
                txtFirstName.Text = String.Empty;
                txtLastName.Text = String.Empty;
                txtQuestion.Text = string.Empty;
                userPanel.Visibility = Visibility.Visible;

                var listQuestions = from question in db.Questions
                                    where question.id == questionId
                                    select question;

                foreach (Questions q in listQuestions)
                {
                    lblQuestionUser.Content = ((Questions)q).question;
                    lblQuestionUser.Foreground = (((Questions)q).textColor == "Red") ? Brushes.Red : (((Questions)q).textColor == "Black")? Brushes.Black : Brushes.Blue;
                    lblQuestionUser.Visibility = Visibility.Visible;
                    _currentQuestionId = ((Questions)q).id;
                    if (!String.IsNullOrEmpty(((Questions)q).userId.ToString()))
                    {
                        var answer = from a in db.Answers
                                     where a.id == ((Questions)q).answerId
                                     select a;

                        foreach (Answers a in answer)
                        {
                            txtAnswer.Text = a.answer;
                        }

                        var user = from u in db.Users
                                   where u.id == ((Questions)q).userId
                                   select u;

                        foreach (Users u in user)
                        {
                            txtFirstName.Text = u.firstName;
                            txtLastName.Text = u.lastName;
                        }                       
                        EnableControlsUserPanel(false);                        
                    }
                    else
                        EnableControlsUserPanel(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Safety_Wheel.Models;
using Safety_Wheel.Pages.Student;
using Safety_Wheel.Services;

namespace Safety_Wheel
{
    public partial class MainWindow : MetroWindow
    {
        private readonly SafetyWheelContext _db = BaseDbService.Instance.Context;
        private TeacherService teacherService = new();
        private StudentService studentService = new();

        private Student _selectedStudent;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void UpdateUserName(string userName)
        {
            HeaderUserNameTextBlock.Text = userName ?? string.Empty;
        }

        private void BackImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainFrame.CanGoBack)
                return;

            var currentPage = MainFrame.Content as Page;

            if (currentPage is StudTest)
            {
                var confirm = new ClosedWindow(
                    "Вы намерены вернуться к выбору теста.",
                    "Тест будет считаться завершенным.")
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                if (confirm.ShowDialog() == true)
                {
                    StudTest._isTestActivated = false;
                    MainFrame.Navigate(new StudSelectedTestsPage(StudHomePage.NameDiscipline));
                }
            }
            else
            {
                MainFrame.GoBack();
            }
        }

        private void ExitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var confirm = new ClosedWindow("Вы намерены выйти из аккаунта.", "")
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (confirm.ShowDialog() == true)
            {
                Clear();
                MainFrame.Navigate(new MainPage());
            }
        }

        private void Clear()
        {
            CurrentUser.Clear();
            UpdateUserName("");
        }
        
        public void OpenTeacherManagerFlyout()
        {
            ReloadStudents();
            TeacherManagerFlyout.IsOpen = true;
        }

        private void TeacherManagerFlyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (TeacherManagerFlyout.IsOpen)
                ReloadStudents();
        }

        private void ReloadStudents()
        {
            using var db = new SafetyWheelContext();

            StudentsGrid.ItemsSource = db.Students
                .Where(s => s.TeachersId == CurrentUser.Id)
                .ToList();
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (teacherService.UserExistsByLogin(LoginTextBox.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }

            var student = new Student
            {
                Name = NameTextBox.Text,
                Login = LoginTextBox.Text,
                Password = PasswordTextBox.Text,
                TeachersId = CurrentUser.Id
            };

            studentService.Add(student);

            ClearInputs();
            ReloadStudents();
        }

        private void SaveStudent_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent == null)
            {
                MessageBox.Show("Выберите ученика");
                return;
            }

            using var db = new SafetyWheelContext();
            var student = db.Students.FirstOrDefault(s => s.Id == _selectedStudent.Id);
            if (student == null) return;

            student.Name = NameTextBox.Text;
            student.Login = LoginTextBox.Text;
            student.Password = PasswordTextBox.Text;

            db.SaveChanges();

            ReloadStudents();
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent == null)
                return;

            if (MessageBox.Show("Удалить ученика?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            using var db = new SafetyWheelContext();
            db.Students.Remove(_selectedStudent);
            db.SaveChanges();

            ClearInputs();
            _selectedStudent = null;

            ReloadStudents();
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student student)
            {
                _selectedStudent = student;

                NameTextBox.Text = student.Name;
                LoginTextBox.Text = student.Login;
                PasswordTextBox.Text = student.Password;
            }
        }

        private void ClearInputs()
        {
            NameTextBox.Clear();
            LoginTextBox.Clear();
            PasswordTextBox.Clear();
        }

    }
}

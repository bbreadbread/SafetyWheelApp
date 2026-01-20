using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Safety_Wheel.Models;
using Safety_Wheel.Pages.Student;
using Safety_Wheel.Services;
using Safety_Wheel.ViewModels;

namespace Safety_Wheel
{
    public partial class MainWindow : MetroWindow
    {
        private readonly TeacherService teacherService = new();
        private readonly StudentService studentService = new();

        private Student _selectedStudent;

        public MainViewModel VM { get; }

        public MainWindow()
        {
            InitializeComponent();
            VM = new MainViewModel();
            DataContext = VM;
            InitMonths();
        }

        public void UpdateUserName(string userName)
        {
            HeaderUserNameTextBlock.Text = userName ?? string.Empty;
        }

        private void BackImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainFrame.CanGoBack) return;

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
            if (TeacherManagerFlyout.IsOpen) ReloadStudents();
        }

        private void ReloadStudents()
        {
            using var db = new SafetyWheelContext();
            StudentsGrid.ItemsSource = db.Students
                                        .Where(s => s.TeachersId == CurrentUser.Id)
                                        .ToList();

            VM?.ReloadStudents();
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

            Student student = studentService.GetCurrentStudent(_selectedStudent.Id);
            if (student == null) return;

            student.Name = NameTextBox.Text;
            student.Login = LoginTextBox.Text;
            student.Password = PasswordTextBox.Text;

            studentService.Update(student);
            ReloadStudents();
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedStudent == null) return;

            if (MessageBox.Show("Удалить ученика?", "Подтверждение",
                MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            Student student = studentService.GetCurrentStudent(_selectedStudent.Id);
            studentService.Remove(student);

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

        private void Calendar_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if (sender is Calendar calendar && calendar.DisplayMode != CalendarMode.Year)
                calendar.DisplayMode = CalendarMode.Year;
        }

        //месяцы

        private void InitMonths()
        {
            MonthComboBox.ItemsSource = new List<MonthItem>
            {
                new() { Number = 1, Name = "Январь" },
                new() { Number = 2, Name = "Февраль" },
                new() { Number = 3, Name = "Март" },
                new() { Number = 4, Name = "Апрель" },
                new() { Number = 5, Name = "Май" },
                new() { Number = 6, Name = "Июнь" },
                new() { Number = 7, Name = "Июль" },
                new() { Number = 8, Name = "Август" },
                new() { Number = 9, Name = "Сентябрь" },
                new() { Number = 10, Name = "Октябрь" },
                new() { Number = 11, Name = "Ноябрь" },
                new() { Number = 12, Name = "Декабрь" }
            };
        }

        private void YearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VM == null) return;
            if (MonthComboBox.SelectedItem is not MonthItem month) return;
            if (!int.TryParse(YearTextBox.Text, out int year)) return;

            VM.SelectedMonthDate = new DateTime(year, month.Number, 1);
        }

    }
    public class MonthItem
    {
        public int Number { get; set; }  
        public string Name { get; set; } 
    }

}
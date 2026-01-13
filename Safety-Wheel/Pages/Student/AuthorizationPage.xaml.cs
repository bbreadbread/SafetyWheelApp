using Safety_Wheel.Pages.Teacher;
using Safety_Wheel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Safety_Wheel.Models;
using WPFCustomMessageBox;

namespace Safety_Wheel.Pages.Student
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private readonly StudentService _studentService = new();
        private readonly TeacherService _teacherService = new();

        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonAuth_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string password = TbPassword.Text.Trim();

            _studentService.GetAllStudents();
            var student = _studentService.Students.FirstOrDefault(s =>
                            s.Login == login && s.Password == password);
            if (student != null)
            {
                CurrentUser.Id = student.Id;
                CurrentUser.Name = student.Name ?? string.Empty;
                CurrentUser.UserType = "Student";

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.UpdateUserName(CurrentUser.Name);
                }

                NavigationService.Navigate(new StudHomePage(student.Id));
                return;
            }

            _teacherService.GetAll();
            var teacher = _teacherService.Teachers.FirstOrDefault(t =>
                            t.Login == login && t.Password == password);
            if (teacher != null)
            {
                CurrentUser.Id = teacher.Id;
                CurrentUser.Name = teacher.Name ?? string.Empty;
                CurrentUser.UserType = "Teacher";

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.UpdateUserName(CurrentUser.Name);
                }
                
                NavigationService.Navigate(new TeacherMainPage(_studentService.Students.Count));
                return;
            }

            CustomMessageBox.Show("Неверный логин или пароль.",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }
    }
}

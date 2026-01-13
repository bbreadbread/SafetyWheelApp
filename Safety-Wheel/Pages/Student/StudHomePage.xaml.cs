using Safety_Wheel.Models;
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

namespace Safety_Wheel.Pages.Student
{
    /// <summary>
    /// Логика взаимодействия для StudHomePage.xaml
    /// </summary>
    public partial class StudHomePage : Page
    {
        public static string NameDiscipline;
        StudentService StudentService = new();

        public StudHomePage(int StudentID)
        {
            InitializeComponent();
            if (Application.Current.MainWindow is MainWindow mw)
                mw.HeaderUserNameTextBlock.Text = StudentService.GetCurrentStudent(StudentID).Name ?? string.Empty;
        }

        private void ButtonGoPdd_Click(object sender, RoutedEventArgs e)
        {
            NameDiscipline = "ПДД";
            NavigationService.Navigate(new StudSelectedTestsPage(NameDiscipline));
        }

        private void ButtonGoMedicine_Click(object sender, RoutedEventArgs e)
        {
            NameDiscipline = "Медицина";
            NavigationService.Navigate(new StudSelectedTestsPage(NameDiscipline));
        }
    }
}

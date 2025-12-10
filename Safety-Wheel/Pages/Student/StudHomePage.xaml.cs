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
        string NameDiscipline;
        public StudHomePage()
        {
            InitializeComponent();
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

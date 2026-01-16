using Safety_Wheel.Models;
using Safety_Wheel.Services;
using Safety_Wheel.ViewModels;
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

namespace Safety_Wheel.Pages.Teacher
{
    /// <summary>
    /// Логика взаимодействия для TeacherAllTests.xaml
    /// </summary>
    public partial class TeacherAllTests : UserControl
    {
        public TeacherAllTests(Subject subject)
        {
            InitializeComponent();
            DataContext = new TeacherAllTestViewModel(subject);
        }

        private void RemoveTest_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (sender is Button btn &&
                btn.Tag is Test test &&
                DataContext is TeacherAllTestViewModel vm)
            {
                vm.RemoveTest(test);
            }
        }


        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border &&
                border.Tag is TestListItemViewModel vm)
            {
                if (vm.IsCreateCard)
                {
                    TeacherMainPage.GlobalInnerFrame
                        ?.Navigate(new TeacherCreateTestsPage(null));
                }
                else if (vm.Test != null)
                {
                    TeacherMainPage.GlobalInnerFrame
                        ?.Navigate(new TeacherCreateTestsPage(vm.Test));
                }
            }
        }
    }
}

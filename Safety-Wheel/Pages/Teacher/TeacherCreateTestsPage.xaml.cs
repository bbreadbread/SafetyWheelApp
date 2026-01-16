using Microsoft.Win32;
using Safety_Wheel.Models;
using Safety_Wheel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Safety_Wheel.Pages.Teacher
{
    public partial class TeacherCreateTestsPage : UserControl
    {
        public TeacherCreateTestsPage()
        {
            InitializeComponent();
            DataContext = new TeacherCreateTestViewModel();
        }
        public TeacherCreateTestsPage(Test? test)
        {
            InitializeComponent();
            DataContext = new TeacherCreateTestViewModel(test);
        }

        private void AddQuestionImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is QuestionCreateViewModel qvm)
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp"
                };

                if (dlg.ShowDialog() == true)
                    qvm.SetQuestionImage(dlg.FileName);
            }
        }

        private void AddOptionImage_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OptionCreateViewModel ovm)
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp"
                };

                if (dlg.ShowDialog() == true)
                    ovm.SetOptionImage(dlg.FileName);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TeacherCreateTestViewModel vm)
                vm.Save();
        }
    }
}

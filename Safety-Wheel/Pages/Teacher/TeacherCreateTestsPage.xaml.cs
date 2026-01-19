using Microsoft.Win32;
using Safety_Wheel.Models;
using Safety_Wheel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Safety_Wheel.Pages.Teacher
{
    public partial class TeacherCreateTestsPage : UserControl
    {
        Test? _test = null;
        public TeacherCreateTestsPage()
        {
            InitializeComponent();
        }
        public TeacherCreateTestsPage(Test? test)
        {
            _test = test;
            InitializeComponent();
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

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_test != null)
            {
                DataContext = new TeacherCreateTestViewModel(_test);
            }
            else
            {
                DataContext = new TeacherCreateTestViewModel(null);
            }
        }
    }
}

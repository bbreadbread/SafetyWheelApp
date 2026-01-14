using ControlzEx.Standard;
using Notifications.Wpf;
using Safety_Wheel.Models;
using Safety_Wheel.Pages.Student;
using Safety_Wheel.Services;
using Safety_Wheel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для TeaViewingTests.xaml
    /// </summary>
    public partial class TeacherViewingTestsPage : UserControl, INotifyPropertyChanged
    {
        private TestService _testService = new();
        public TeacherViewingTestsPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as SelectedDateViewModel;

            OnPropertyChanged(string.Empty);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AttemptButton_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is not Attempt attempt) return;

            var test = _testService.GetTestById(attempt.TestId); 
            int? sec = attempt.FinishedAt == null ? null : (int)(attempt.FinishedAt - attempt.StartedAt)?.TotalSeconds;
            int? type = attempt.TestType;

            var studPage = new Safety_Wheel.Pages.Student.StudTest(test, sec, type, true, attempt);

            TeacherMainPage.GlobalInnerFrame?.Navigate(studPage);
        }
    }
}

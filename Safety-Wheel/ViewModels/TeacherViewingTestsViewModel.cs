using Safety_Wheel.Models;
using Safety_Wheel.Services;
using System;
using System.Collections.ObjectModel;

namespace Safety_Wheel.ViewModels
{
    public class TeacherViewingTestsViewModel : ObservableObject
    {
        private Attempt _selectedAttempt;
        private Student _student;
        private DateTime? _selectedDate;
        private ObservableCollection<Attempt> _studentAttempts;

        public Attempt SelectedAttempt
        {
            get => _selectedAttempt;
            set => SetProperty(ref _selectedAttempt, value);
        }

        public Student Student
        {
            get => _student;
            set => SetProperty(ref _student, value);
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public ObservableCollection<Attempt> StudentAttempts
        {
            get => _studentAttempts;
            set => SetProperty(ref _studentAttempts, value);
        }

        public string DateTitle => SelectedDate.HasValue
            ? $"Попытки за {SelectedDate.Value:dd.MM.yyyy}"
            : "Все попытки";

        public TeacherViewingTestsViewModel()
        {
            StudentAttempts = new ObservableCollection<Attempt>();
        }
    }
}
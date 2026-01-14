using Safety_Wheel.Models;
using Safety_Wheel.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Wheel.ViewModels
{
    public class SelectedDateViewModel : ObservableObject
    {
        private DateTime _date;
        private Student _student;
        private ObservableCollection<Attempt> _attempts;
        private AttemptService _attemptService = new();

        public DateTime Date
        {
            get => _date;
            set
            {
                if (SetProperty(ref _date, value))
                {
                    LoadAttempts();
                }
            }
        }

        public Student Student
        {
            get => _student;
            set
            {
                if (SetProperty(ref _student, value))
                {
                    LoadAttempts();
                }
            }
        }

        public ObservableCollection<Attempt> Attempts
        {
            get => _attempts;
            set => SetProperty(ref _attempts, value);
        }

        public string DateTitle => $"Попытки за {Date:dd.MM.yyyy}";

        public SelectedDateViewModel()
        {
            Attempts = new ObservableCollection<Attempt>();
        }

        public SelectedDateViewModel(DateTime date, Student student)
        {
            Attempts = new ObservableCollection<Attempt>();
            _date = date;  
            _student = student;
            LoadAttempts();
        }

        private void LoadAttempts()
        {
            if (Student == null || Student.Id == 0)
                return;

            Attempts.Clear();
            _attemptService.GetAll(studentId: Student.Id, date: Date);

            foreach (var a in _attemptService.Attempts)
                Attempts.Add(a);
        }
    }
}

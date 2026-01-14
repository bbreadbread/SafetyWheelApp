using MahApps.Metro.IconPacks;
using Safety_Wheel.Services;
using Safety_Wheel.Models;
using Safety_Wheel.Pages.Teacher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Safety_Wheel.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<MenuItemViewModel> _menuItems;
        private ObservableCollection<MenuItemViewModel> _menuDatesItems;
        private StudentService _studentService = new();
        private AttemptService _attemptService = new();

        private MenuItemViewModel _selectedStudent;
        private MenuItemViewModel _selectedDate;
        private Student _currentStudent;
        private DateTime? _selectedDateValue;
        private AttemptDetailsViewModel _currentAttemptDetails;
        public AttemptDetailsViewModel CurrentAttemptDetails
        {
            get => _currentAttemptDetails;
            set => SetProperty(ref _currentAttemptDetails, value);
        }


        public MainViewModel(int count)
        {
            _studentService.GetAllStudents(CurrentUser.Id);
            this.CreateMenuItems();
        }

        public void CreateMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel> { };
            MenuDatesItems = new ObservableCollection<MenuItemViewModel> { };

            foreach (var st in _studentService.Students)
            {
                var view = new MenuItemViewModel(this)
                {
                    Icon = new TextBlock
                    {
                        Text = $"{st.Name}",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Label = $"Попыток: {st.Attempts?.Count ?? 0}",
                    ToolTip = $"Студент: {st.Name}",
                    Tag = st
                };

                MenuItems.Add(view);
            }
        }

        private void LoadStudentDates(Student student)
        {
            if (student == null) return;

            MenuDatesItems.Clear();

            List<DateTime> list = _attemptService.GetUniqueAttemptDates(student.Id);

            foreach (var date in list)
            {
                var dateVm = new SelectedDateViewModel(date, student);  

                var menuItem = new MenuItemViewModel(this)
                {
                    Icon = new TextBlock
                    {
                        Text = $"{date:dd.MM}",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Label = $"Попыток: {dateVm.Attempts.Count}", 
                    ToolTip = $"Дата: {date:dd.MM.yyyy}\nПопыток: {dateVm.Attempts.Count}",
                    Tag = dateVm         
                    
                };
                MenuDatesItems.Add(menuItem);
            }

            SelectedDate = null;
        }

        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
            set => SetProperty(ref _menuItems, value);
        }

        public ObservableCollection<MenuItemViewModel> MenuDatesItems
        {
            get => _menuDatesItems;
            set => SetProperty(ref _menuDatesItems, value);
        }

        public MenuItemViewModel SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (SetProperty(ref _selectedStudent, value) && value?.Tag is Student student)
                {
                    LoadStudentDates(student);
                    CurrentStudent = student;
                }
            }
        }
        public MenuItemViewModel SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    if (value?.Tag is SelectedDateViewModel dateViewModel)
                    {
                        SelectedDateValue = dateViewModel.Date;
                    }
                }
            }
        }

        public Student CurrentStudent
        {
            get => _currentStudent;
            set => SetProperty(ref _currentStudent, value);
        }

        public DateTime? SelectedDateValue
        {
            get => _selectedDateValue;
            set => SetProperty(ref _selectedDateValue, value);
        }

    }
}
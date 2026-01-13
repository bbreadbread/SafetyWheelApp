using MahApps.Metro.IconPacks;
using Safety_Wheel.Services;
using Safety_Wheel.Models;
using Safety_Wheel.Pages.Teacher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Safety_Wheel.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private ObservableCollection<MenuItemViewModel> _menuItems;
        private ObservableCollection<MenuItemViewModel> _menuOptionItems;
        private ObservableCollection<MenuItemViewModel> _menuDatesItems;
        private StudentService _studentService = new();
        private AttemptService _attemptService = new();

        private MenuItemViewModel _selectedStudent;
        private Student _currentStudent;

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
                _attemptService.GetAll(st.Id);
                var view = new TeacherSelectedStudViewModel(this)
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
                    Label = $"{st.Attempts}",
                    ToolTip = $"Студент: {st.Name}",
                    Tag = st
                };

                MenuItems.Add(view);
            }

            MenuOptionItems = new ObservableCollection<MenuItemViewModel>
            {
                new TeacherSelectedStudViewModel(this)
                {
                    Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.Cog },
                    Label = "Добавление",
                    ToolTip = "The App settings"
                }
            };
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

        public ObservableCollection<MenuItemViewModel> MenuOptionItems
        {
            get => _menuOptionItems;
            set => SetProperty(ref _menuOptionItems, value);
        }
        public MenuItemViewModel SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (SetProperty(ref _selectedStudent, value) && value?.Tag is Student student)
                {
                    LoadStudentAttempts(student);
                    CurrentStudent = student;
                }
            }
        }

        public Student CurrentStudent
        {
            get => _currentStudent;
            set => SetProperty(ref _currentStudent, value);
        }

        private void LoadStudentAttempts(Student student)
        {
            _attemptService.GetAll(student.Id);

            MenuDatesItems.Clear();

            foreach (var at in _attemptService.Attempts)
            {
                var attemptItem = new MenuItemViewModel(this)
                {
                    Icon = new TextBlock
                    {
                        Text = $"{at.StartedAt:dd.MM.yyyy}",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Label = $"{at.StartedAt:HH:mm}",
                    ToolTip = $"Попытка от {at.StartedAt}",
                    Tag = at
                };

                MenuDatesItems.Add(attemptItem);
            }
        }
    }
}

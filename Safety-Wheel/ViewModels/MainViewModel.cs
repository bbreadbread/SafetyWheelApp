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
        private ObservableCollection<MenuItemViewModel> _mainMenuItems;
        private ObservableCollection<MenuItemViewModel> _menuItems;
        private ObservableCollection<MenuItemViewModel> _menuDatesItems;
        private ObservableCollection<MenuItemViewModel> _menuAttemptsItems;
        private StudentService _studentService = new();
        private AttemptService _attemptService = new();
        private TestService _testService = new();

        private MenuItemViewModel _selectedMainMenuItem;
        private MenuItemViewModel _selectedStudent;
        private MenuItemViewModel _selectedDate;
        private MenuItemViewModel _selectedAttempt;

        private Student _currentStudent;
        private DateTime? _selectedDateValue;
        private object _currentContent;

        public enum MainMenuType
        {
            TestResults,
            Statistics,
            EditCreateTests
        }

        public MainViewModel(int count)
        {
            _studentService.GetAllStudents(CurrentUser.Id);
            CreateMainMenuItems();
            CreateMenuItems();
        }

        public void CreateMainMenuItems()
        {
            MainMenuItems = new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel(this)
                {
                    Icon = new PackIconMaterial { Kind = PackIconMaterialKind.ChartLine },
                    Label = "Статистика",
                    ToolTip = "Статистика по тестам и студентам",
                    Tag = MainMenuType.Statistics
                },
                new MenuItemViewModel(this)
                {
                    Icon = new PackIconMaterial { Kind = PackIconMaterialKind.ChartBar },
                    Label = "Результаты тестирования",
                    ToolTip = "Просмотр результатов тестирования студентов",
                    Tag = MainMenuType.TestResults
                },                
                new MenuItemViewModel(this)
                {
                    Icon = new PackIconMaterial { Kind = PackIconMaterialKind.PlusBox },
                    Label = "Создание тестов",
                    ToolTip = "Создание и редактирование тестов",
                    Tag = MainMenuType.EditCreateTests
                }
            };
        }

        public void CreateMenuItems()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel> { };
            MenuDatesItems = new ObservableCollection<MenuItemViewModel> { };
            MenuAttemptsItems = new ObservableCollection<MenuItemViewModel> { };
        }

        private void LoadStudentDates(Student student)
        {
            if (student == null) return;

            MenuDatesItems.Clear();
            MenuAttemptsItems.Clear();
            CurrentContent = null;

            List<DateTime> list = _attemptService.GetUniqueAttemptDates(student.Id);

            foreach (var date in list)
            {
                var dateItem = new MenuItemViewModel(this)
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
                    Label = $"Дата: {date:dd.MM.yyyy}",
                    ToolTip = $"Кликните, чтобы увидеть попытки за {date:dd.MM.yyyy}",
                    Tag = new DateInfo
                    {
                        Date = date,
                        Student = student
                    }
                };

                MenuDatesItems.Add(dateItem);
            }

            SelectedDate = null;
        }

        private void LoadDateAttempts(DateInfo dateInfo)
        {
            if (dateInfo == null) return;

            MenuAttemptsItems.Clear();
            CurrentContent = null;

            _attemptService.GetAll(studentId: dateInfo.Student.Id, date: dateInfo.Date);

            foreach (var attempt in _attemptService.Attempts.OrderByDescending(a => a.StartedAt))
            {
                var attemptItem = new MenuItemViewModel(this)
                {
                    Icon = new TextBlock
                    {
                        Text = $"{attempt.StartedAt:HH:mm}",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Label = $"Результат: {attempt.Score?.ToString() ?? "Не оценено"}",
                    ToolTip = $"Нажмите для просмотра теста\nВремя: {attempt.StartedAt:HH:mm}\nРезультат: {attempt.Score}",
                    Tag = attempt
                };

                MenuAttemptsItems.Add(attemptItem);
            }

            SelectedAttempt = null;
        }
        private void LoadTestsForEdit()
        {
            MenuItems.Clear();
            CurrentContent = null;

            _testService.GetAll(CurrentUser.Id);

            foreach (var test in _testService.Tests)
            {
                MenuItems.Add(new MenuItemViewModel(this)
                {
                    Icon = new TextBlock
                    {
                        Text = $"{test.Name}",
                        FontSize = 20,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    },
                    Label = test.Name,
                    ToolTip = "Редактировать тест",
                    Tag = test
                });
            }

            MenuItems.Add(new MenuItemViewModel(this)
            {
                Icon = new TextBlock
                {
                    Text = "+",
                    FontSize = 24,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Label = "Создать новый тест",
                ToolTip = "Создание нового теста",
                Tag = "NEW_TEST"
            });
        }

        public ObservableCollection<MenuItemViewModel> MainMenuItems
        {
            get => _mainMenuItems;
            set => SetProperty(ref _mainMenuItems, value);
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

        public ObservableCollection<MenuItemViewModel> MenuAttemptsItems
        {
            get => _menuAttemptsItems;
            set => SetProperty(ref _menuAttemptsItems, value);
        }

        public object CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public MenuItemViewModel SelectedMainMenuItem
        {
            get => _selectedMainMenuItem;
            set
            {
                if (SetProperty(ref _selectedMainMenuItem, value))
                {
                    TeacherMainPage.GlobalInnerFrame?.Navigate(new Page());

                    MenuItems.Clear();
                    MenuDatesItems.Clear();
                    MenuAttemptsItems.Clear();
                    CurrentContent = null;
                    SelectedStudent = null;
                    SelectedDate = null;
                    SelectedAttempt = null;

                    if (value?.Tag is MainMenuType menuType)
                    {
                        switch (menuType)
                        {
                            case MainMenuType.TestResults:
                                LoadStudentsForResults();
                                break;
                            case MainMenuType.Statistics:
                                TeacherMainPage.GlobalInnerFrame?.Navigate(new TeacherStatisticsPage());
                                LoadStudentsForStatistics();
                                break;
                            case MainMenuType.EditCreateTests:
                                LoadTestsForEdit();
                                TeacherMainPage.GlobalInnerFrame?.Navigate(new TeacherCreateTestsPage(null));
                                break;


                        }
                    }
                }
            }
        }

        private void LoadStudentsForResults()
        {
            MenuItems.Clear();

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
        private void LoadStudentsForStatistics()
        {
            MenuItems.Clear();

            MenuItems.Add(new MenuItemViewModel(this)
            {
                Icon = new TextBlock
                {
                    Text = "Все",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                Label = "Общая статистика",
                ToolTip = "Показать статистику по всем студентам",
                Tag = null
            });

            foreach (var st in _studentService.Students)
            {
                MenuItems.Add(new MenuItemViewModel(this)
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
                    Label = "Статистика",
                    ToolTip = $"Статистика по студенту: {st.Name}",
                    Tag = st
                });
            }
        }


        public MenuItemViewModel SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (!SetProperty(ref _selectedStudent, value))
                    return;

                if (SelectedMainMenuItem?.Tag is not MainMenuType menuType)
                    return;

                if (menuType == MainMenuType.Statistics && value?.Tag == null)
                {
                    CurrentStudent = null;
                    CurrentContent = new StatisticsViewModel(null);
                    TeacherStatisticsPage.DataPageTeacher?.LoadStatisticsForStudent(null);
                    return;
                }
                else if (menuType == MainMenuType.EditCreateTests)
                {
                    if (value?.Tag is Test test)
                    {
                        TeacherMainPage.GlobalInnerFrame
                            ?.Navigate(new TeacherCreateTestsPage(test));
                    }
                    else if (value?.Tag?.ToString() == "NEW_TEST")
                    {
                        TeacherMainPage.GlobalInnerFrame
                            ?.Navigate(new TeacherCreateTestsPage(null));
                    }
                }

                if (value?.Tag is Student student)
                {
                    switch (menuType)
                    {
                        case MainMenuType.TestResults:
                            LoadStudentDates(student);
                            CurrentStudent = student;
                            break;

                        case MainMenuType.Statistics:
                            CurrentStudent = student;
                            CurrentContent = new StatisticsViewModel(student);
                            TeacherStatisticsPage.DataPageTeacher?.LoadStatisticsForStudent(student);
                            break;
                    }
                }
            }
        }


        public MenuItemViewModel SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value) && value?.Tag is DateInfo dateInfo)
                {
                    LoadDateAttempts(dateInfo);
                    SelectedDateValue = dateInfo.Date;
                }
            }
        }

        public MenuItemViewModel SelectedAttempt
        {
            get => _selectedAttempt;
            set
            {
                if (SetProperty(ref _selectedAttempt, value))
                {
                    if (value?.Tag is Attempt attempt && CurrentStudent != null)
                    {
                        var test = _testService.GetTestById(attempt.TestId);
                        int? sec = attempt.FinishedAt == null ? null : (int)(attempt.FinishedAt - attempt.StartedAt)?.TotalSeconds;
                        int? type = attempt.TestType;

                        var studPage = new Safety_Wheel.Pages.Student.StudTest(test, sec, type, true, attempt);
                        TeacherMainPage.GlobalInnerFrame?.Navigate(studPage);
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
    public class DateInfo
    {
        public DateTime Date { get; set; }
        public Student Student { get; set; }
    }
}
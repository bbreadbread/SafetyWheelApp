using MahApps.Metro.Controls;
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
using Safety_Wheel.Models;
using Safety_Wheel.Services;

namespace Safety_Wheel.Pages.Teacher
{
    /// <summary>
    /// Логика взаимодействия для TeacherMainPage.xaml
    /// </summary>
    public partial class TeacherMainPage : Page
    {
        public enum ViewTypes
        {
            Open,
            Search
        }

        public ViewTypes ViewType { get; set; }

        private string _Title = string.Empty;

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public TeacherMainPage(int menuItemsCount = 0)
        {
            InitializeComponent();
            DataContext = new MainViewModel(menuItemsCount);
        }
        private void DatesMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (e.InvokedItem is HamburgerMenuIconItem menuItem)
            {
                this.DatesMenuControl.Content = e.InvokedItem;

                switch (menuItem.Tag?.ToString())
                {
                    case "HomePage":
                        break;
                    case "ProfilePage":
                        break;
                    case "SettingsPage":
                        break;
                    case "HelpPage":
                        break;
                    case "Exit":
                        Application.Current.Shutdown();
                        break;
                }

                if (!e.IsItemOptions)
                {
                    this.DatesMenuControl.IsPaneOpen = false;
                }
            }
        }
    }
}

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
using System.Windows.Media.Animation;
using static Safety_Wheel.ViewModels.MainViewModel;

namespace Safety_Wheel.Pages.Teacher
{
    /// <summary>
    /// Логика взаимодействия для TeacherMainPage.xaml
    /// </summary>
    public partial class TeacherMainPage : Page
    {
        public Frame InnerFrame => this.FrameTeacher;

        public static Frame GlobalInnerFrame = new();

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
        int _menuItemsCount;
        public TeacherMainPage(int menuItemsCount = 0)
        {
            _menuItemsCount = menuItemsCount;
            InitializeComponent();
            GlobalInnerFrame = FrameTeacher;
        }

        private void DatesMenuControl_ItemInvoked(object sender, MahApps.Metro.Controls.HamburgerMenuItemInvokedEventArgs e)
        {
            if (DatesMenuControl.IsPaneOpen)
            {
                var anim = new DoubleAnimation(48, TimeSpan.FromMilliseconds(250));
                DatesMenuControl.BeginAnimation(MahApps.Metro.Controls.HamburgerMenu.CompactPaneLengthProperty, anim);
                DatesMenuControl.IsPaneOpen = false;
                DatesMenuControl.SetCurrentValue(HamburgerMenu.SelectedIndexProperty, -1);
            }
            
        }

        private void DataGridRow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && !row.IsSelected)
            {
                row.IsSelected = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainViewModel(_menuItemsCount);
        }

        private void HamburgerMenu_ItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            if (!e.IsItemOptions)
                return;

            if (e.InvokedItem is MenuItemViewModel item &&
                item.Tag is MainMenuType.TeacherManager)
            {
                var menu = sender as MahApps.Metro.Controls.HamburgerMenu;

                if (Application.Current.MainWindow is MainWindow mw)
                {
                    mw.TeacherManagerFlyout.IsOpen = true;
                }
            }
        }
    }
}

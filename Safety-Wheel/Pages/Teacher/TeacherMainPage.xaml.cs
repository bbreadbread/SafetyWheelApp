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

namespace Safety_Wheel.Pages.Teacher
{
    /// <summary>
    /// Логика взаимодействия для TeacherMainPage.xaml
    /// </summary>
    public partial class TeacherMainPage : Page
    {
        public Frame InnerFrame => this.FrameTeacher;

        public static Frame GlobalInnerFrame;

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

    }
}

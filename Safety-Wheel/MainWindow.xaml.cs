using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Safety_Wheel.Pages.Student;

namespace Safety_Wheel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void UpdateUserName(string userName)
        {
            HeaderUserNameTextBlock.Text = userName ?? string.Empty;
        }

        private void BackImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                var currentPage = MainFrame.Content as Page;

                if (currentPage is StudTest)
                {
                    var confirmWindow = new ClosedWindow("Вы намерены вернуться к выбору теста. ", "Тест будет считаться завершенным.")
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    var result = confirmWindow.ShowDialog();

                    if (result == true)
                    {
                        StudTest._isTestActivated = false;
                        MainFrame.Navigate(new StudSelectedTestsPage(StudHomePage.NameDiscipline));
                    }
                }
                else if (currentPage is StudHomePage)
                {
                    MainFrame.GoBack();
                    Clear();
                }
                else
                    MainFrame.GoBack();
            }
        }

        private void ExitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var currentPage = MainFrame.Content as Page;

            if (currentPage is StudTest)
            {
                var confirmWindow = new ClosedWindow("Вы намерены выйти из аккаунта. ", "Тест будет считаться завершенным.")
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var result = confirmWindow.ShowDialog();

                if (result == true)
                {
                    StudTest._isTestActivated = false;
                    Clear();
                    MainFrame.Navigate(new MainPage());
                }
            }
            else
            {
                var confirmWindow = new ClosedWindow("Вы намерены выйти из аккаунта. ", "")
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                var result = confirmWindow.ShowDialog();

                if (result == true)
                {
                    Clear();
                    MainFrame.Navigate(new MainPage());
                }
            }
        }

        private void Clear()
        {
            CurrentUser.Clear();
            UpdateUserName("");
        }

    }
}
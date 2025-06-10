using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.Models;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserViewModel userViewModel;

        public string LoggedUsername { get; private set; }
        public User LoggedUser { get; private set; }
        public bool IsAdmin { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            userViewModel = new UserViewModel();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            var user = userViewModel.GetUser(username, password);
            if (user != null)
            {
                LoggedUser = user;
                LoggedUsername = user.Username;
                IsAdmin = user.IsAdmin;
                DialogResult = true;
                Close();
            }
            else
            {
                ErrorText.Text = "Nieprawidłowe dane logowania.";
            }
        }
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login_Click(LoginButton, new RoutedEventArgs());
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }

    }
}

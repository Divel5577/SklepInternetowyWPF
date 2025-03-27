using System.Windows;
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
    }
}

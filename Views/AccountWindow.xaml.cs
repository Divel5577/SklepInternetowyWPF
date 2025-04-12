using System.Windows;
using System.Windows.Media.Animation;
using SklepInternetowyWPF.ViewModels;

namespace SklepInternetowyWPF.Views
{
    public partial class AccountWindow : Window
    {
        private readonly string username;

        public bool LoggedOut { get; private set; } = false;

        public AccountWindow(string username)
        {
            InitializeComponent();
            this.username = username;
            WelcomeText.Text = $"Zalogowany jako: {username}";
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var history = new OrderHistoryWindow(username);
            history.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoggedOut = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)this.Resources["FadeInStoryboard"];
            storyboard.Begin(this);
        }

    }
}

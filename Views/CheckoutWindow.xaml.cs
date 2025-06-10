using SklepInternetowyWPF.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SklepInternetowyWPF.Views
{
    public partial class CheckoutWindow : Window
    {
        private readonly CartViewModel cart;
        private readonly CheckoutFormViewModel form;

        public CheckoutWindow(CartViewModel cart)
        {
            InitializeComponent();
            this.cart = cart;
            this.form = new CheckoutFormViewModel();
            this.DataContext = new { Form = form, CartItems = cart.CartItems };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)this.Resources["FadeInStoryboard"];
            sb.Begin(this);
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // 1) Włącz walidację w view-modelu
            form.ValidateOnSubmit = true;  // od teraz IDataErrorInfo będzie zgłaszać błędy :contentReference[oaicite:0]{index=0}

            // 2) Sprawdź każdy string-owy property (pomijając ValidateOnSubmit i Notes)
            var stringProps = typeof(CheckoutFormViewModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string) && p.Name != nameof(form.Notes));

            foreach (var prop in stringProps)
            {
                var error = form[prop.Name];  // IDataErrorInfo[propertyName]
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(
                        "Uzupełnij poprawnie wszystkie wymagane pola.",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            // 3) Sprawdź, czy koszyk nie jest pusty
            if (cart.CartItems.Count == 0)
            {
                MessageBox.Show(
                    "Koszyk jest pusty.",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // 4) Wszystko ok – złóż zamówienie
            string payment = ((ComboBoxItem)PaymentBox.SelectedItem).Content.ToString();
            cart.PlaceOrder(
                form.FirstName,
                form.LastName,
                form.Phone,
                form.Street,
                form.PostalCode,
                form.City,
                form.Notes,
                payment
            );

            MessageBox.Show("Zamówienie złożone pomyślnie!");
            Close();
        }
    }
}

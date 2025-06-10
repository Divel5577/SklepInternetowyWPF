using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SklepInternetowyWPF.ViewModels
{
    public class CheckoutFormViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set { if (_firstName != value) { _firstName = value; OnPropertyChanged(); } }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); } }
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set { if (_phone != value) { _phone = value; OnPropertyChanged(); } }
        }

        private string _street;
        public string Street
        {
            get => _street;
            set { if (_street != value) { _street = value; OnPropertyChanged(); } }
        }

        private string _postalCode;
        public string PostalCode
        {
            get => _postalCode;
            set { if (_postalCode != value) { _postalCode = value; OnPropertyChanged(); } }
        }

        private string _city;
        public string City
        {
            get => _city;
            set { if (_city != value) { _city = value; OnPropertyChanged(); } }
        }

        // opcjonalne
        private string _notes;
        public string Notes
        {
            get => _notes;
            set { if (_notes != value) { _notes = value; OnPropertyChanged(); } }
        }

        private bool _validateOnSubmit = false;
        public bool ValidateOnSubmit
        {
            get => _validateOnSubmit;
            set
            {
                if (_validateOnSubmit != value)
                {
                    _validateOnSubmit = value;
                    // odświeżamy walidację wszystkich pól:
                    OnPropertyChanged(string.Empty);
                }
            }
        }

        // IDataErrorInfo
        public string this[string columnName]
        {
            get
            {
                if (!ValidateOnSubmit) return null;

                if (columnName == nameof(FirstName) && string.IsNullOrWhiteSpace(FirstName))
                    return "Wymagane";
                if (columnName == nameof(LastName) && string.IsNullOrWhiteSpace(LastName))
                    return "Wymagane";
                if (columnName == nameof(Phone) && string.IsNullOrWhiteSpace(Phone))
                    return "Wymagane";
                if (columnName == nameof(Street) && string.IsNullOrWhiteSpace(Street))
                    return "Wymagane";
                if (columnName == nameof(PostalCode) && string.IsNullOrWhiteSpace(PostalCode))
                    return "Wymagane";
                if (columnName == nameof(City) && string.IsNullOrWhiteSpace(City))
                    return "Wymagane";

                return null;
            }
        }

        public string Error => null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

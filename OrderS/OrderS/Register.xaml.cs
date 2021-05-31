using System;
using System.ComponentModel.DataAnnotations;
using System.Security.RightsManagement;
using System.Text.RegularExpressions;
using System.Windows;


namespace OrderS
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public User User;
        public string Password;
        public Register()
        {
            InitializeComponent();
        }

        private void Submit_OnClick(object sender, RoutedEventArgs e)
        {
            string fullName = name.Text;
            string phoneNumber = phone.Text;
            string adress = this.adress.Text;
            string email = this.email.Text;
            string password = this.password.Text;
            if (!TryFullName(fullName)) { MessageBox.Show("Your name is empty or not full"); return; }
            if (!TryPhone(phoneNumber)) { MessageBox.Show("Your phone is empty or incorrect"); return; }
            if (!TryIsEmpty(adress)) { MessageBox.Show("Your adress is empty"); return; }
            if (!TryEmail(email)) { MessageBox.Show("Your email is empty or incorrect"); return; }
            if (!TryIsEmpty(password)) { MessageBox.Show("Your password is empty"); return; }

            Password = password;
            User = new User(fullName, phoneNumber, adress, email);
            DialogResult = true;
            Close();
        }

        private bool TryFullName(string name) => name.Split().Length == 3;

        private bool TryPhone(string phone) => Regex.IsMatch(phone,
            @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$");

        private bool TryIsEmpty(string str) => str != "";

        private bool TryEmail(string email) => new EmailAddressAttribute().IsValid(email);
    }
}

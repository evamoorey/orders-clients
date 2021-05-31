using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace OrderS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<User> Users;
        public ObservableCollection<Product> Products;
        public ObservableCollection<Order> Orders;
        public User CurrentUser;
        private bool isSeller;
        public MainWindow()
        {
            InitializeComponent();
            Login.Visibility = Visibility.Hidden;
            Password.Visibility = Visibility.Hidden;
            SignIn.Visibility = Visibility.Hidden;
            SignUp.Visibility = Visibility.Hidden;
            Back.Visibility = Visibility.Hidden;
            Users = new ObservableCollection<User>();
            Products = new ObservableCollection<Product>();
            Orders = new ObservableCollection<Order>();
            RestoreData();
        }

        private void RestoreData()
        {
            try
            {
                if (File.Exists("users.json") && File.Exists("orders.json") && File.Exists("products.json"))
                {
                    string path = "users.json";
                    FileStream sr = new FileStream(path, FileMode.Open);
                    DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(ObservableCollection<User>), new Type[] { typeof(ObservableCollection<Order>), typeof(ObservableCollection<Product>) });
                    Users = (ObservableCollection<User>)formatter.ReadObject(sr);
                    sr.Close();
                    path = "products.json";
                    FileStream sr1 = new FileStream(path, FileMode.Open);
                    DataContractJsonSerializer formatter1 = new DataContractJsonSerializer(typeof(ObservableCollection<Product>));
                    Products = (ObservableCollection<Product>)formatter1.ReadObject(sr1);
                    sr1.Close();
                    path = "orders.json";
                    FileStream sr2 = new FileStream(path, FileMode.Open);
                    DataContractJsonSerializer formatter2 = new DataContractJsonSerializer(typeof(ObservableCollection<Order>), new Type[] { typeof(User), typeof(ObservableCollection<Product>) });
                    Orders = (ObservableCollection<Order>)formatter2.ReadObject(sr2);
                    sr2.Close();
                    RecoverOrders();
                }
            }
            catch { MessageBox.Show("You don't have data to restore."); }

        }

        private void RecoverOrders()
        {
            foreach (var order in Orders)
            {
                foreach (var user in Users)
                    if (order.Login == user.Email)
                        order.Client = user;
            }
        }

        private void Authorize()
        {
            HelloLabel.Visibility = Visibility.Hidden;
            Seller.Visibility = Visibility.Hidden;
            Client.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Hidden;
            Login.Visibility = Visibility.Visible;
            Password.Visibility = Visibility.Visible;
            SignIn.Visibility = Visibility.Visible;
            SignUp.Visibility = Visibility.Visible;
            Back.Visibility = Visibility.Visible;
        }
        
        private void Seller_OnClick(object sender, RoutedEventArgs e)
        {
            isSeller = true;
            Authorize();
        }

        private void Client_OnClick(object sender, RoutedEventArgs e)
        {
            isSeller = false;
            Authorize();
        }

        private void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in Users)
                if (item.IsSeller == isSeller && item.Email == Login.Text && item.SecureData == GetSha256Hash(Password.Text + Login.Text))
                    CurrentUser = item;
            if (CurrentUser is null)
            {
                MessageBoxResult result = MessageBox.Show("Login and/or password - incorrect, do you want to create new user?", "Authorization",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SignUp_OnClick(sender, e);
                }
            }
            else
            {
                if (CurrentUser is { IsSeller: true })
                {
                    Seller window = new Seller(Products, Orders, Users, CurrentUser);
                    Close();
                    window.Show();
                }
                else
                {
                    Client window = new Client(Products, Orders, Users, CurrentUser);
                    Close();
                    window.Show();
                }
            }
            
        }

        private bool TryFindUser(string login)
        {
            foreach (var item in Users)
                if (item.Email == login)
                    return true;
            return false;
        }
        private void SignUp_OnClick(object sender, RoutedEventArgs e)
        {
            Register reg = new Register();
            var info = reg.ShowDialog();
            if (info == true)
            {
                if (!TryFindUser(reg.User.Email))
                {
                    reg.User.IsSeller = isSeller;
                    reg.User.SecureData = GetSha256Hash(reg.Password + reg.User.Email);
                    Users.Add(reg.User);
                }else MessageBox.Show("User with this email is already exists.");
            }
        }

        static string GetSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder hashBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    hashBuilder.Append(bytes[i].ToString("x2"));
                return hashBuilder.ToString();
            }
        }


        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            string path = "users.json";
            FileStream sr = new FileStream(path, FileMode.Create);
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(ObservableCollection<User>), new Type[] { typeof(ObservableCollection<Order>), typeof(ObservableCollection<Product>) });
            formatter.WriteObject(sr, Users);
            sr.Close();
            path = "products.json";
            FileStream sr1 = new FileStream(path, FileMode.Create);
            DataContractJsonSerializer formatter1 = new DataContractJsonSerializer(typeof(ObservableCollection<Product>));
            formatter1.WriteObject(sr1, Products);
            sr1.Close();
            path = "orders.json";
            FileStream sr2 = new FileStream(path, FileMode.Create);
            DataContractJsonSerializer formatter2 = new DataContractJsonSerializer(typeof(ObservableCollection<Order>), new Type[] { typeof(User), typeof(ObservableCollection<Product>) });
            formatter2.WriteObject(sr2, Orders);
            sr2.Close();
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            Login.Visibility = Visibility.Hidden;
            Password.Visibility = Visibility.Hidden;
            SignIn.Visibility = Visibility.Hidden;
            SignUp.Visibility = Visibility.Hidden;
            Back.Visibility = Visibility.Hidden;
            HelloLabel.Visibility = Visibility.Visible;
            Seller.Visibility = Visibility.Visible;
            Client.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Visible;
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Немного для справки" + Environment.NewLine + Environment.NewLine +
            "Покупатель:" + Environment.NewLine +
            "1) Нажатие(ЛКМ)на товар в списке товаров - это добавление товара в корзину." + Environment.NewLine+
            "2) Нажатие(ПКМ) по выделенному товару в корзине - это редактирование количества и удаление товара." + Environment.NewLine+
            "3) Нажатие(ЛКМ)по заказу -список товаров + возможность оплаты заказа." + Environment.NewLine + Environment.NewLine +
            "Продавец:" + Environment.NewLine +
            "1) Нажатие(ПКМ) на таблицу товаров - добавление нового товара. Если товар выбран, то удаление товара." + Environment.NewLine +
            "2) Нажатие(ЛКМ) на таблицу клиентов - все заказы, оплаченная сумма" + Environment.NewLine +
            "3) Нажатие(ПКМ) по таблице заказов - изменение статуса, просмотр содержимого заказа" + Environment.NewLine);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace OrderS
{
    /// <summary>
    /// Interaction logic for Seller.xaml
    /// </summary>
    public partial class Seller : Window
    {
        public ObservableCollection<Product> Products;
        public ObservableCollection<Order> Orders;
        public ObservableCollection<User> Clients;
        public ObservableCollection<User> Users;
        public User CurrentUser;
        public bool isClientButton = true;
        public bool isActive = false;

        public Seller(ObservableCollection<Product> products, ObservableCollection<Order> orders, ObservableCollection<User> users, User currentUser)
        {
            InitializeComponent();
            Products = products;
            CurrentUser = currentUser;
            ProductsGrid.ItemsSource = Products;
            Orders = orders;
            Users = users;
            Clients = GetClients(users);
            SellerGrid.ItemsSource = Clients;
        }

        private ObservableCollection<User> GetClients(ObservableCollection<User> users)
        {
            ObservableCollection<User> temp = new ObservableCollection<User>();
            foreach (var item in users)
                if (!item.IsSeller) temp.Add(item);
            return temp;
        }



        private void ProductsGrid_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem create = new MenuItem();
            create.Header = "Add product";
            create.VerticalContentAlignment = VerticalAlignment.Center;
            create.HorizontalAlignment = HorizontalAlignment.Left;
            create.Click += Create_Click;
            menu.Items.Add(create);
            if (ProductsGrid.SelectedItem != null)
            {
                MenuItem delete = new MenuItem();
                delete.Header = "Delete product";
                delete.VerticalContentAlignment = VerticalAlignment.Center;
                delete.HorizontalAlignment = HorizontalAlignment.Left;
                delete.Click += Delete_Click;
                menu.Items.Add(delete);
            }

            menu.IsOpen = true;
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            NewProduct product = new NewProduct();
            var info = product.ShowDialog();
            if (info == true)
            {
                if (!TryCode(product.NewProductsProduct.Code))
                    MessageBox.Show("Product's code is already exists.");
                else
                    Products.Add(product.NewProductsProduct);
            }

            ProductsGrid.ItemsSource = Products;
            ProductsGrid.Items.Refresh();
            ProductsGrid.UpdateLayout();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Products.Remove((Product)ProductsGrid.CurrentCell.Item);
                foreach (var item in Users)
                {
                    foreach (var product in item.CartProducts)
                    {
                        if (product.Code == ((Product) ProductsGrid.CurrentCell.Item).Code)
                            item.CartProducts.Remove(product);
                    }
                }
                ProductsGrid.Items.Refresh();
                ProductsGrid.UpdateLayout();
            }
            catch { }
        }

        private bool TryCode(string code)
        {
            int count = 0;
            foreach (var item in Products)
                if (item.Code == code)
                    count++;
            return count <= 0;
        }

        private void SellerGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SellerGrid.SelectedItem != null && isClientButton && !isActive)
            {
                ClientData window = new ClientData((User)SellerGrid.CurrentCell.Item);
                window.Show();
            }

        }

        private string ProductsToString(Order item)
        {
            string result = "Name | Code | Price | Count" + Environment.NewLine + "__________________________________________" + Environment.NewLine;
            foreach (var product in item.Products)
                result += product.Name + " | " + product.Code + " | " + product.Price + " | " + product.Count + Environment.NewLine;
            return result;
        }

        private void Completed_Click(object sender, RoutedEventArgs e)
        {
            if (((Order)SellerGrid.CurrentCell.Item).StatusStatus.HasFlag(Status.Shipped))
                ((Order)SellerGrid.CurrentCell.Item).StatusStatus |= Status.Completed;
            else MessageBox.Show("Order not shipped.");
            SellerGrid.ItemsSource = Orders;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
            if (isActive)
                ActiveOrders_OnClick(sender,e);
        }

        private void Shipped_Click(object sender, RoutedEventArgs e)
        {
            if (((Order)SellerGrid.CurrentCell.Item).StatusStatus.HasFlag(Status.Paid))
                ((Order)SellerGrid.CurrentCell.Item).StatusStatus |= Status.Shipped;
            else MessageBox.Show("Order not paid.");
            SellerGrid.ItemsSource = Orders;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
            if (isActive)
                ActiveOrders_OnClick(sender, e);
        }

        private void Processed_Click(object sender, RoutedEventArgs e)
        {
            ((Order)SellerGrid.CurrentCell.Item).StatusStatus |= Status.Processed;
            SellerGrid.ItemsSource = Orders;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
            if (isActive)
                ActiveOrders_OnClick(sender, e);
        }

        private void ClientsButton_OnClick(object sender, RoutedEventArgs e)
        {
            isClientButton = true;
            isActive = false;
            SellerGrid.ItemsSource = Clients;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
        }

        private void OrdersButton_OnClick(object sender, RoutedEventArgs e)
        {
            isClientButton = false;
            isActive = false;
            SellerGrid.ItemsSource = Orders;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
        }

        private void Seller_OnClosing(object sender, CancelEventArgs e)
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

        private void Authorize_OnClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Users = Users;
            main.Products = Products;
            main.Orders = Orders;
            main.Show();
            Close();

        }

        private void SellerGrid_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SellerGrid.SelectedItem != null && !isClientButton)
            {
                ContextMenu menu = new ContextMenu();
                MenuItem processed = new MenuItem();
                processed.Header = "Processed";
                processed.VerticalContentAlignment = VerticalAlignment.Center;
                processed.HorizontalAlignment = HorizontalAlignment.Left;
                processed.Click += Processed_Click;
                menu.Items.Add(processed);
                MenuItem shipped = new MenuItem();
                shipped.Header = "Shipped";
                shipped.VerticalContentAlignment = VerticalAlignment.Center;
                shipped.HorizontalAlignment = HorizontalAlignment.Left;
                shipped.Click += Shipped_Click;
                menu.Items.Add(shipped);
                MenuItem executed = new MenuItem();
                executed.Header = "Completed";
                executed.VerticalContentAlignment = VerticalAlignment.Center;
                executed.HorizontalAlignment = HorizontalAlignment.Left;
                executed.Click += Completed_Click;
                menu.Items.Add(executed);
                if (!((Order) SellerGrid.CurrentCell.Item).StatusStatus.HasFlag(Status.Paid))
                    shipped.IsEnabled = false;
                if (!((Order) SellerGrid.CurrentCell.Item).StatusStatus.HasFlag(Status.Shipped))
                    executed.IsEnabled = false;
                MenuItem contain = new MenuItem();
                contain.Header = "Show products";
                contain.VerticalContentAlignment = VerticalAlignment.Center;
                contain.HorizontalAlignment = HorizontalAlignment.Left;
                contain.Click += Contain_Click;
                menu.Items.Add(contain);
                menu.IsOpen = true;
            }
        }

        private void Contain_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ProductsToString((Order)SellerGrid.CurrentCell.Item));
        }

        private void ActiveOrders_OnClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Order> activeOrders = new ObservableCollection<Order>();
            foreach (var order in Orders)
                if (!order.StatusStatus.HasFlag(Status.Completed)) activeOrders.Add(order);
            isActive = true;
            isClientButton = false;
            SellerGrid.ItemsSource = activeOrders;
            SellerGrid.Items.Refresh();
            SellerGrid.UpdateLayout();
        }

        private void Report_OnClick(object sender, RoutedEventArgs e)
        {
            EditCount window = new EditCount();
            window.Name = "Sum";
            window.Text.Text = "Enter sum for report";
            var info = window.ShowDialog();
            if (info == true)
            {
                ObservableCollection<User> newUsers = new ObservableCollection<User>();
                foreach (var item in Users)
                {
                    double sum = 0;
                    foreach (var order in item.Orders)
                        if (order.StatusStatus.HasFlag(Status.Paid))
                            sum += order.Price;
                    item.PaidPrice = sum;
                    if (sum >= window.NewCount) newUsers.Add(item);
                }
                var sortedUsers = newUsers.OrderBy(u => u.PaidPrice);
                var data = sortedUsers.Reverse();
                ReportData report = new ReportData(data.ToList(), window.NewCount);
                report.Show();
            }
        }

        private void CancelReport_OnClick(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            foreach (var product in Products)
                data.Add("Name: " + product.Name + " |Code: " + product.Code + " |Price: " + product.Price);
            ChooseProduct window = new ChooseProduct(data);
            var info = window.ShowDialog();
            if (info == true)
            {
                Product temp = Products[window.CurrentProduct];
                List<string> dates = new List<string>();
                List<User> usersReport = new List<User>();
                GetDataProducts(temp, usersReport,dates);
                ReportData report = new ReportData(usersReport,dates, "Name: " + temp.Name + " |Code: " + temp.Code + " |Price: " + temp.Price);
                report.Show();
            }
        }

        private void GetDataProducts(Product temp, List<User> usersReport, List<string> dates)
        {
            foreach (var user in Users)
            {
                bool flag = false;
                string date = "";
                foreach (var order in user.Orders)
                {
                    bool inFlag = false;
                    foreach (var product in order.Products)
                    {
                        if (product.Code == temp.Code)
                        {
                            flag = true;
                            inFlag = true;
                        }
                    }

                    if (inFlag)
                        date += order.Data + " | ";
                }

                if (flag)
                {
                    usersReport.Add(user);
                    dates.Add(date);
                }
            }
        }
    }
}

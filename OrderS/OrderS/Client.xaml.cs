using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrderS
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    public partial class Client : Window
    {
        public ObservableCollection<Product> Products;
        public ObservableCollection<Order> Orders;
        public ObservableCollection<User> Users;
        public User CurrentUser;
        private Random random = new Random();

        public Client(ObservableCollection<Product> products, ObservableCollection<Order> orders, ObservableCollection<User> users, User currentUser)
        {
            InitializeComponent();
            Orders = orders;
            Products = products;
            Users = users;
            CurrentUser = currentUser;
            ProductsGrid.ItemsSource = Products;
            if (CurrentUser.CartProducts != null && CurrentUser.CartProducts.Count != 0)
                CartGrid.ItemsSource = CurrentUser.CartProducts;
        }

        private void ProductsGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ProductsGrid.SelectedItem != null)
            {
                if (((Product)ProductsGrid.CurrentCell.Item).Count <= 0)
                {
                    MessageBox.Show("Our shop didn't have this product now.");
                }
                else
                {
                    if (IndexProduct((Product)ProductsGrid.CurrentCell.Item) == -1)
                    {
                        Product tempProduct = new Product()
                        {
                            Name = ((Product)ProductsGrid.CurrentCell.Item).Name,
                            Code = ((Product)ProductsGrid.CurrentCell.Item).Code,
                            Price = ((Product)ProductsGrid.CurrentCell.Item).Price,
                            Count = 1
                        };
                        CurrentUser.CartProducts.Add(tempProduct);
                        CartGrid.ItemsSource = CurrentUser.CartProducts;
                        SumLabel.Content = "Sum: " + GetPrice(CurrentUser.CartProducts);
                        ((Product)ProductsGrid.CurrentCell.Item).Count--;
                    }
                    else
                    {
                        CurrentUser.CartProducts[IndexProduct((Product)ProductsGrid.CurrentCell.Item)].Count++;
                        SumLabel.Content = "Sum: " + GetPrice(CurrentUser.CartProducts);
                        ((Product)ProductsGrid.CurrentCell.Item).Count--;
                    }
                }
                CartGrid.Items.Refresh();
                CartGrid.UpdateLayout();
                ProductsGrid.Items.Refresh();
                ProductsGrid.UpdateLayout();
            }
        }

        private int IndexProduct(Product item)
        {
            for (int i = 0; i < CurrentUser.CartProducts.Count; ++i)
                if (CurrentUser.CartProducts[i].Code == item.Code)
                    return i;
            return -1;
        }

        private void CartGrid_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem editCount = new MenuItem();
            editCount.Header = "Edit count";
            editCount.VerticalContentAlignment = VerticalAlignment.Center;
            editCount.HorizontalAlignment = HorizontalAlignment.Left;
            editCount.Click += EditCount_Click;
            menu.Items.Add(editCount);
            MenuItem delete = new MenuItem();
            delete.Header = "Delete product";
            delete.VerticalContentAlignment = VerticalAlignment.Center;
            delete.HorizontalAlignment = HorizontalAlignment.Left;
            delete.Click += DeleteProduct_Click;
            menu.Items.Add(delete);
            menu.IsOpen = true;
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = ((Product)CartGrid.CurrentCell.Item).Count;
                foreach (var item in Products)
                    if (item.Code == ((Product)CartGrid.CurrentCell.Item).Code) item.Count += count;
                CurrentUser.CartProducts.Remove((Product)CartGrid.CurrentCell.Item);
                SumLabel.Content = "Sum: " + GetPrice(CurrentUser.CartProducts);
                CartGrid.Items.Refresh();
                CartGrid.UpdateLayout();
                ProductsGrid.Items.Refresh();
                ProductsGrid.UpdateLayout();
            }
            catch { }
        }

        private void EditCount_Click(object sender, RoutedEventArgs e)
        {
            if (CartGrid.SelectedItem != null)
            {
                EditCount window = new EditCount();
                var info = window.ShowDialog();
                if (info == true)
                {
                    bool flag = false;
                    foreach (var item in Products)
                        if (item.Code == ((Product)CartGrid.CurrentCell.Item).Code && item.Count + CurrentUser.CartProducts[IndexProduct((Product)CartGrid.CurrentCell.Item)].Count >= window.NewCount)
                        {
                            item.Count -= window.NewCount - CurrentUser.CartProducts[IndexProduct((Product)CartGrid.CurrentCell.Item)].Count;
                            flag = true;
                        }

                    if (flag)
                        CurrentUser.CartProducts[IndexProduct((Product)CartGrid.CurrentCell.Item)].Count = window.NewCount;
                    else MessageBox.Show("Shop doesn't have this count of selected product.");
                }
                SumLabel.Content = "Sum: " + GetPrice(CurrentUser.CartProducts);
                CartGrid.Items.Refresh();
                CartGrid.UpdateLayout();
                ProductsGrid.Items.Refresh();
                ProductsGrid.UpdateLayout();
            }
        }

        private void SubmitOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.CartProducts.Count == 0)
            {
                MessageBox.Show("Your cart is empty! Add smth...");
            }
            else
            {
                SumLabel.Content = "Sum: ";
                Order order = new Order();
                order.Count = GetCount(CurrentUser.CartProducts);
                order.Login = CurrentUser.Email;
                order.Data = DateTime.Now;
                order.Client = CurrentUser;
                order.Number = TryRand();
                order.Price = GetPrice(CurrentUser.CartProducts);
                order.Products = new ObservableCollection<Product>(CurrentUser.CartProducts);
                CurrentUser.CartProducts.Clear();
                CurrentUser.CartProducts = new ObservableCollection<Product>();
                CartGrid.ItemsSource = CurrentUser.CartProducts;
                CartGrid.Items.Refresh();
                CartGrid.UpdateLayout();
                Orders.Add(order);
                CurrentUser.Orders.Add(order);
            }
        }

        private int GetCount(ObservableCollection<Product> cartProducts)
        {
            int sum = 0;
            foreach (var item in cartProducts)
                sum += item.Count;
            return sum;
        }

        private double GetPrice(ObservableCollection<Product> products)
        {
            double sum = 0;
            foreach (var item in products)
                sum += item.Price * item.Count;
            return sum;
        }
        private int TryRand()
        {
            int temp = 0;
            while (true)
            {
                temp = random.Next();
                foreach (var order in Orders)
                    if (order.Number == temp) continue;
                break;
            }

            return temp;
        }

        private void MyOrders_OnClick(object sender, RoutedEventArgs e)
        {
            MyOrders orders = new MyOrders(CurrentUser.Orders);
            orders.Show();
        }

        private void Client_OnClosing(object sender, CancelEventArgs e)
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
            Close();
            main.Show();
        }
    }
}

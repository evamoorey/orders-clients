using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for MyOrders.xaml
    /// </summary>
    public partial class MyOrders : Window
    {
        public ObservableCollection<Order> Orders;
        public MyOrders(ObservableCollection<Order> orders)
        {
            InitializeComponent();
            Orders = orders;
            OrdersGrid.ItemsSource = Orders;
        }

        private void OrdersGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OrdersGrid.SelectedItem != null)
            {
                OrderSummary summary = new OrderSummary((Order)OrdersGrid.CurrentCell.Item);
                var info = summary.ShowDialog();
                if (info == true)
                {
                    OrdersGrid.ItemsSource = Orders;
                    OrdersGrid.Items.Refresh();
                    OrdersGrid.UpdateLayout();
                }
            }
        }
    }
}

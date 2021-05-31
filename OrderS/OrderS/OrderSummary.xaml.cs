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
    /// Interaction logic for OrderSummary.xaml
    /// </summary>
    public partial class OrderSummary : Window
    {
        public Order CurrentOrder;

        public OrderSummary(Order order)
        {
            InitializeComponent();
            Sum.Text = "Sum: " + order.Price;
            CurrentOrder = order;
            ProductsGrid.ItemsSource = CurrentOrder.Products;
        }

        private void Pay_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentOrder.StatusStatus.HasFlag(Status.Processed) && !CurrentOrder.StatusStatus.HasFlag(Status.Paid))
            {
                CurrentOrder.StatusStatus |= Status.Paid;
                DialogResult = true;
                MessageBox.Show("Successful payment!");
                Close();
            }
            else
                MessageBox.Show("Your order isn't processed (or already paid), you can't pay for it.");
        }
    }
}

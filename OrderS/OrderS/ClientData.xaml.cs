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
    /// Interaction logic for ClientData.xaml
    /// </summary>
    public partial class ClientData : Window
    {
        public User CurrentClient;
        public ClientData(User current)
        {
            InitializeComponent();
            CurrentClient = current;
            Sum.Text = "Paid in total: " + GetPaidSum(CurrentClient.Orders);
            OrdersGrid.ItemsSource = CurrentClient.Orders;
        }

        private double GetPaidSum(ObservableCollection<Order> orders)
        {
            double sum = 0;
            foreach (var item in orders)
                if (item.StatusStatus.HasFlag(Status.Paid))
                    sum += item.Price;
            return sum;
        }

    }
}

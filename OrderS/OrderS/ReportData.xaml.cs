using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
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
    /// Interaction logic for ReportData.xaml
    /// </summary>
    public partial class ReportData : Window
    {
        public ReportData(List<User> users, int count)
        {
            InitializeComponent();
            CountLabel.Text = $"Clients that paid {count} and more.";
            ObservableCollection<object> obj = new ObservableCollection<object>();
            foreach (var item in users)
                obj.Add(new { Name = item.Name, PhoneNumber = item.PhoneNumber, Adress = item.Adress, Email = item.Email, PaidPrice = item.PaidPrice });
            ClientsGrid.ItemsSource = obj;
            ClientsGrid.Items.Refresh();
            ClientsGrid.UpdateLayout();
        }

        public ReportData(List<User> users, List<string> dates, string product)
        {
            InitializeComponent();
            CountLabel.Text = $"Users with ({product}) in orders.";
            ObservableCollection<object> obj = new ObservableCollection<object>();
            for (int i = 0; i < users.Count; ++i)
                obj.Add(new { Name = users[i].Name, PhoneNumber = users[i].PhoneNumber, Adress = users[i].Adress, Email = users[i].Email, DatesOfOrders = dates[i] });
            ClientsGrid.ItemsSource = obj;
            ClientsGrid.Items.Refresh();
            ClientsGrid.UpdateLayout();
        }
    }
}

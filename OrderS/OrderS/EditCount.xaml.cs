using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditCount.xaml
    /// </summary>
    partial class EditCount : Window
    {
        public int NewCount;
        public EditCount()
        {
            InitializeComponent();
        }

        private void Submit_OnClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Count.Text, out NewCount) || NewCount <= 0)
                MessageBox.Show("Incorrect, number should be positive.");
            else
            {
                DialogResult = true;
                Close();
            }
        }
    }
}

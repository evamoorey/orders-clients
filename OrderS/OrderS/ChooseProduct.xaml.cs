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
    /// Interaction logic for ChooseProduct.xaml
    /// </summary>
    public partial class ChooseProduct : Window
    {
        public int CurrentProduct;
        public ChooseProduct(List<string> data)
        {
            InitializeComponent();
            foreach (var item in data)
                Selection.Items.Add(new ComboBoxItem { Content = item });
            Selection.SelectedIndex = 0;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentProduct = Selection.SelectedIndex;
            DialogResult = true;
            Close();
        }
    }
}

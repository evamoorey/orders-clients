using System;
using System.Windows;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace OrderS
{
    /// <summary>
    /// Interaction logic for NewProduct.xaml
    /// </summary>
    public partial class NewProduct : Window
    {
        public Product NewProductsProduct;

        /// <summary>
        /// Empty constructor for NewProduct.xaml.
        /// </summary>
        public NewProduct()
        {
            InitializeComponent();
            textName.Text = "";
            textCode.Text = "";
            textPrice.Text = "0";
            textCount.Text = "0";
        }

        /// <summary>
        /// Submit changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            uint count = 0;
            double price = 0;
            if (uint.TryParse(textCount.Text, out count) && double.TryParse(textPrice.Text, out price) && price >= 0 && textName.Text != "" && textCode.Text != "")
            {
                NewProductsProduct = new Product(textName.Text, textCode.Text, price, (int)count);
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("Count or Price is not a number or name/price is empty!");
        }

    }
}

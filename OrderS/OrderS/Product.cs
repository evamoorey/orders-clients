using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;


namespace OrderS
{
    /// <summary>
    /// Product class.
    /// </summary>
    [DataContract]
    public class Product
    {
        /// <summary>
        /// Name of product.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Product's code.
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// Product's price.
        /// </summary>
        [DataMember]
        public double Price { get; set; }
        /// <summary>
        /// Count of product.
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Product()
        {
            Name = "";
            Code = "";
            Price = 0;
            Count = 0;
        }

        public Product(string name, string code, double price, int count)
        {
            Name = name;
            Code = code;
            Price = price;
            Count = count;
        }
    }
}
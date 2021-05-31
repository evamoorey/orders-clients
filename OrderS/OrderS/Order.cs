using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OrderS
{
    [Flags]
    public enum Status
    {
        Def = 0,
        Processed = 1,
        Paid = 2,
        Shipped = 4,
        Completed = 8
    } 
    [DataContract]
    public class Order
    {
        [DataMember]
        public ObservableCollection<Product> Products;
        [DataMember]
        public int Count { set; get; }
        [DataMember]
        public double Price { set; get; }
        [DataMember]
        public int Number { set; get; }
        [DataMember]
        public DateTime Data { set; get; }
        [IgnoreDataMember]
        public string Status
        {
            get
            {
                return StatusStatus.ToString();
            }
        }
        [DataMember]
        public Status StatusStatus;
        [IgnoreDataMember]
        public User Client;
        [DataMember]
        public string Login { set; get; }
        public Order(){}
    }
}

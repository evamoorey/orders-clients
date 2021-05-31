using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace OrderS
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public string PhoneNumber { set; get; }
        [DataMember]
        public string Adress { set; get; }
        [DataMember]
        public string Email { set; get; }
        [DataMember]
        public string SecureData;

        [DataMember]
        public bool IsSeller;
        [DataMember]
        public ObservableCollection<Product> CartProducts;
        [DataMember]
        public ObservableCollection<Order> Orders;

        [IgnoreDataMember] public double PaidPrice;

        public User()
        {
            Name = "";
            PhoneNumber = "";
            Adress = "";
            Email = "";
            IsSeller = false;
            CartProducts = new ObservableCollection<Product>();
            Orders = new ObservableCollection<Order>();
        }

        public User(string name, string phone, string adress, string email)
        {
            Name = name;
            PhoneNumber = phone;
            Adress = adress;
            Email = email;
            CartProducts = new ObservableCollection<Product>();
            Orders = new ObservableCollection<Order>();

        }
    }
}

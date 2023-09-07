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
using ERP_Hicham.ERP.Billing;
using ERP_Hicham.ERP;
using Org.BouncyCastle.Asn1.Cms;
using Google.Protobuf.WellKnownTypes;

namespace ERP_Hicham.Dialog.CreateBill
{
    /// <summary>
    /// Interaktionslogik für CreateBill.xaml
    /// </summary>
    public partial class CreateBill : Window
    {
        private static Customer customer;
        private static Bill? currentbill;
        private List<ProduktBill> produktBills;

        private Dictionary<string, uint> comboboxValue;

        public CreateBill() {
            InitializeComponent();
            comboboxValue = new Dictionary<string, uint>();
            produktBills = new List<ProduktBill>();

            Customer_TextBox.Text = customer.LastName + " " + customer.FirstName;
            Date_TextBox.Text = DateTime.Now.ToString("dd-MM-yyyy");

            List<Product> _products = MainWindow.instance.DB.QueryAllProducts();
            List<String> _values = new List<string>();

            foreach (Product _p in _products) {
                string _strVal = _p.ID.ToString() + ": " + _p.Name + " - " + _p.Cost + "Fr";
                _values.Add(_strVal);
                comboboxValue.Add(_strVal, _p.ID);
            }

            Product_ComboBox.ItemsSource = _values;
        }

        public static Bill? Dialog(Grid _parent, Customer _customer) {
            _parent.IsEnabled = false;

            customer = _customer;
            currentbill = null;
            CreateBill dialog = new CreateBill();
            dialog.ShowDialog();

            _parent.IsEnabled = true;

            return currentbill;
        }

        private void UpdateList() {
            List<ProductListItem> _items = new List<ProductListItem>();

            float _total = 0;

            foreach (ProduktBill _pb in produktBills) {
                Product _product = MainWindow.instance.DB.QueryProduct(_pb.produktID);
                if (_product == null) return;

                _total += (float)_pb.count * _product.Cost;

                _items.Add(new ProductListItem(
                    _pb,
                    _product.Name,
                    _pb.count,
                    _product.Cost));
            }

            Cost_TextBox.Text = _total.ToString() + " Fr";
            lvProducts.ItemsSource = _items;
        }

        private void AddItem_Button_Click(object sender, RoutedEventArgs e) {
            uint _id;
            bool res = comboboxValue.TryGetValue(Product_ComboBox.Text, out _id);
            if (!res) return;

            uint _count;
            if (!uint.TryParse(Count_Input.Text, out _count)) return;

            ProduktBill _pb;
            _pb.count = _count;
            _pb.produktID = _id;
            _pb.ID = 0;
            produktBills.Add(_pb);

            Count_Input.Text = "1";

            UpdateList();
        }

        private void Create_Click(object sender, RoutedEventArgs e) {
            bool res = Confirm.Confirm.PromptQuestion(MainGrid, "Wollen sie wirklich diese Rechnung erstellen?", "Erstellen");
            if (!res) return;
            
            currentbill = new Bill(0, customer.ID, DateTime.Now);

            foreach (ProduktBill _pb in produktBills) {
                currentbill.AddProdukt(0, _pb.produktID, _pb.count);
            }

            currentbill.UpdateValues();

            Close();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e) {
            currentbill = null;
            Close();
        }

        private void RemoveItem_Button_Copy_Click(object sender, RoutedEventArgs e) {
            if (lvProducts.SelectedItem == null) return;

            ProductListItem _pli = (ProductListItem)lvProducts.SelectedItem;
            produktBills.Remove(_pli.pb);

            UpdateList();
        }

    }

    class ProductListItem
    {
        public ProductListItem(ProduktBill _pb, string product, uint count, float cost) {
            Product = product;
            Count = count;
            Cost = cost;
            Total = cost * (float)count;
            pb = _pb;
        }

        public ProduktBill pb;
        public string Product { get; private set; }
        public uint Count { get; private set; }
        public float Cost { get; private set; }
        public float Total { get; private set; }
    }
}
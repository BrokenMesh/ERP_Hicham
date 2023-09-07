using ERP_Hicham.Dialog.Confirm;
using ERP_Hicham.ERP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ERP_Hicham.Dialog.CreateCustomer;
using ERP_Hicham.Dialog.EditCustomer;
using ERP_Hicham.Dialog.SearchCustomer;
using ERP_Hicham.ERP.Billing;
using ERP_Hicham.ERP.Billing.BillingExport;
using System.Printing;
using ERP_Hicham.Dialog.CreateBill;
using ERP_Hicham.Dialog.EditBill;
using ERP_Hicham.Dialog.EditProducts;

namespace ERP_Hicham
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public DBManager DB;

        public MainWindow() {
            instance = this;

            InitializeComponent();

            // Edit here
            DB = new DBManager("localhost", "Hicham", "Hallosaid1", "erp_db");
            UpdateLists();

            CustomerListButtons_Grid.IsEnabled = false;
        }

        public void SetActive(bool _value) {
            Main_Grid.IsEnabled = _value;
        }

        #region lvUser_Buttons
        private void Delete_Button_Click(object sender, RoutedEventArgs e) {
            if (lvUsers.SelectedItem == null) return;

            Customer _customer = (Customer)lvUsers.SelectedItem;

            bool res = Confirm.PromptQuestion(Main_Grid, $"Wollen sie wirklich den Kunden '{_customer.LastName} {_customer.FirstName}' löschen?", "Löschen");
            if (!res) return;

            DB.RemoveCustomer(_customer.ID);

            UpdateLists();
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e) {
            Customer? _customer = CreateCustomer.CreateCustomerDialog(Main_Grid);
            if (_customer == null) return;

            DB.AddCustomer(_customer);

            UpdateLists();
        }
        private void Edit_Button_Click(object sender, RoutedEventArgs e) {
            if (lvUsers.SelectedItem == null) return;

            Customer _oldcustomer = (Customer)lvUsers.SelectedItem;

            Customer? _customer = EditCustomer.EditCustomerDialog(Main_Grid, _oldcustomer);
            if (_customer == null) return;

            DB.UpdateCustomer(_oldcustomer.ID, _customer);

            UpdateLists();
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e) {
            List<Customer> _customers = DB.QueryAllCustomer();
            lvUsers.ItemsSource = _customers;

            Customer _resCustomer = SearchCustomer.Dialog(Main_Grid, _customers);

            lvUsers.SelectedItem = _resCustomer;
        }
        #endregion

        #region lvBill_Buttons
        private void Create_Bill_Button(object sender, RoutedEventArgs e) {
            Customer _customer = (Customer)lvUsers.SelectedItem;
            Bill _bill = CreateBill.Dialog(Main_Grid, _customer);
            if (_bill == null) return;

            DB.AddBill(_bill);

            UpdateBillList();
        }

        private void Delete_Bill_Click(object sender, RoutedEventArgs e) {
            if (lvBill.SelectedItem == null) return;

            bool _res = Confirm.PromptQuestion(Main_Grid, "Wollen sie wiklich diese Rechnung löschen?", "Löschen");
            if (!_res) return;

            BillListItem _bill = (BillListItem)lvBill.SelectedItem;
            DB.RemoveBill(_bill.ID);

            UpdateBillList();
        }

        private void Edit_Bill_Click(object sender, RoutedEventArgs e) {
            if (lvBill.SelectedItem == null) return;

            BillListItem _billitem = (BillListItem)lvBill.SelectedItem;
            Customer _customer = (Customer)lvUsers.SelectedItem;

            Bill? _bill = DB.QueryBill(_billitem.ID);
            if (_bill == null) return;

            Bill? _newbill = EditBill.Dialog(Main_Grid, _bill, _customer);
            if (_newbill == null) return;

            DB.UpdateBill(_bill.ID, _bill);

            UpdateBillList();
        }

        private void Edit_Products_Button_Click(object sender, RoutedEventArgs e) {
            EditProduct.Open(Main_Grid);

            UpdateLists();
            UpdateBillList();
        }
        #endregion

        #region Menu_Update
        private void lvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lvUsers.SelectedItem == null) {
                CustomerListButtons_Grid.IsEnabled = false;
                Bill_Grid.IsEnabled = false;
                BillListButtons_Grid.IsEnabled = false;

                ID_Textbox.Text = "";
                Firstname_Textbox.Text = "";
                Lastname_Textbox.Text = "";
                Cooperation_TextBox.Text = "";
                Address_Textbox.Text = "";
                City_Textbox.Text = "";
                State_Textbox.Text = "";

                UpdateBillList();
            }
            else {
                CustomerListButtons_Grid.IsEnabled = true;
                Bill_Grid.IsEnabled = true;

                Customer _customer = (Customer)lvUsers.SelectedItem;
                ID_Textbox.Text = _customer.ID.ToString();
                Firstname_Textbox.Text = _customer.FirstName;
                Lastname_Textbox.Text = _customer.LastName;
                Cooperation_TextBox.Text = _customer.Cooperation;
                Address_Textbox.Text = _customer.Address;
                City_Textbox.Text = _customer.City;
                State_Textbox.Text = _customer.State;

                UpdateBillList();
            }

            lvBill_SelectionChanged(null, null);
        }

        private void UpdateLists() {
            List<Customer> _customers = DB.QueryAllCustomer();
            lvUsers.ItemsSource = _customers;
        }

        private void UpdateBillList() {
            if (lvUsers.SelectedItem == null) {
                lvBill.ItemsSource = null;
                return;
            }
            
            Customer _customer = (Customer)lvUsers.SelectedItem;

            List<Bill> _bills = DB.QueryAllBills(_customer);

            List<BillListItem> _items = new List<BillListItem>();
            foreach (Bill _b in _bills) {
                _items.Add(new BillListItem(
                    _b.ID,
                    _b.date.ToShortDateString(),
                    _b.cost,
                    _b.orderNames
                    ));
            }

            lvBill.ItemsSource = _items;
        }
           

        private void lvBill_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lvBill.SelectedItem == null) {
                BillListButtons_Grid.IsEnabled = false;
            }
            else {
                BillListButtons_Grid.IsEnabled = true;
            }
        }
        #endregion
    }
}


class BillListItem { 
    public uint ID { get; private set; }
    public string Date { get; private set; }
    public float Cost { get; private set; }
    public string Ordernames { get; private set; }

    public BillListItem(uint iD, string date, float cost, string ordernames) {
        ID = iD;
        this.Date = date;
        this.Cost = cost;
        this.Ordernames = ordernames;
    }
}

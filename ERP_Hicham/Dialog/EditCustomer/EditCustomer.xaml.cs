using ERP_Hicham.ERP;
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

namespace ERP_Hicham.Dialog.EditCustomer
{
    /// <summary>
    /// Interaktionslogik für EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : Window
    {
        private static Customer? customer;

        public EditCustomer(Customer _customer)
        {
            InitializeComponent();

            Cooperation_input.Text = _customer.Cooperation;
            Firstname_input.Text = _customer.FirstName;
            Lastname_input.Text = _customer.LastName;
            Address_input.Text = _customer.Address;
            City_input.Text = _customer.City;
            State_input.Text = _customer.State;
        }

        public static Customer? EditCustomerDialog(Grid _parent, Customer _customer) {
            _parent.IsEnabled = false;

            customer = null;
            EditCustomer dialog = new EditCustomer(_customer);
            dialog.ShowDialog();

            _parent.IsEnabled = true;

            return customer;
        }
  
        private void Button_Click(object sender, RoutedEventArgs e) {
            customer = null;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Customer _customer = new Customer(0,
                Cooperation_input.Text,
                Firstname_input.Text,
                Lastname_input.Text,
                Address_input.Text,
                City_input.Text,
                State_input.Text);

            bool res = Confirm.Confirm.PromptQuestion(MainGrid, $"Wollen sie wirklich den Kunden '{_customer.LastName} {_customer.FirstName}' bearbeiten?", "Erstellen");

            if (res) {
                customer = _customer;
                Close();
                return;
            }
        }
    }
}

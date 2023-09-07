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
using ERP_Hicham.Dialog.Confirm;

namespace ERP_Hicham.Dialog.CreateCustomer
{
    /// <summary>
    /// Interaktionslogik für CreateCustomer.xaml
    /// </summary>
    public partial class CreateCustomer : Window
    {
        private static Customer? customer;

        public CreateCustomer()
        {
            InitializeComponent();
        }

        public static Customer? CreateCustomerDialog(Grid _parent) {
            _parent.IsEnabled = false;

            customer = null;
            CreateCustomer dialog = new CreateCustomer();
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

            bool res = Confirm.Confirm.PromptQuestion(MainGrid, $"Wollen sie wirklich den Kunden '{_customer.LastName} {_customer.FirstName}' erstellen?", "Erstellen");

            if (res) {
                customer = _customer;
                Close();
                return;
            }
        }

    }
}

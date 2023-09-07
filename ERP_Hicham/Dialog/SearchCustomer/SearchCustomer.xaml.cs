using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ERP_Hicham.ERP;
using ERP_Hicham.ERP.Billing;

namespace ERP_Hicham.Dialog.SearchCustomer
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class SearchCustomer : Window
    {
        private static List<Customer> customers;
        private static Customer? currentCustomer;

        public SearchCustomer()
        {
            InitializeComponent();
        }

        public static Customer? Dialog(Grid _parent, List<Customer> _customers) {
            customers = _customers;

            _parent.IsEnabled = false;

            currentCustomer = null;
            SearchCustomer dialog = new SearchCustomer();
            dialog.ShowDialog();

            _parent.IsEnabled = true;

            return currentCustomer;
        }

        private void lvUsers_SelectionChanged(object sender, SelectionChangedEventArgs e){
            if (lvUsers.SelectedItem == null) {
                Search_Button.IsEnabled = false;
            } else { 
                Search_Button.IsEnabled = true;
                currentCustomer = (Customer)lvUsers.SelectedItem;
            }
        }

        private void Search_Input_TextChanged(object sender, TextChangedEventArgs e) {
            if (lvUsers == null) return;
            string _input = Search_Input.Text.ToLower();
            string _searchType = SearchType_ComboBox.Text;

            List<Customer> _searchResult = new List<Customer>();   

            if (_searchType == "Name") {
                foreach (Customer _c in customers) {
                    if (_c.FirstName.ToLower().StartsWith(_input) || _c.LastName.ToLower().StartsWith(_input)) {
                        _searchResult.Add(_c);
                    }
                }          
            } 
            else if (_searchType == "Firma") {
                foreach (Customer _c in customers) {
                    if (_c.Cooperation.ToLower().StartsWith(_input)) {
                        _searchResult.Add(_c);
                    }
                }
            }
            else if (_searchType == "ID") {
                foreach (Customer _c in customers) {
                    if (_c.ID.ToString().StartsWith(_input)) {
                        _searchResult.Add(_c);
                    }
                }
            }
            else if (_searchType == "BillID")  {
                foreach (Customer _c in customers) {
                    List<Bill> _bills = MainWindow.instance.DB.QueryAllBills(_c);

                    bool hasBill = false;
                    foreach (Bill _b in _bills) {
                        if (_b.ID.ToString().StartsWith(_input)) {
                            hasBill = true;
                            break;
                        }
                    }
                    if (hasBill) _searchResult.Add(_c);     
                }
            }

            lvUsers.ItemsSource = _searchResult;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs e) {
            if (currentCustomer == null || lvUsers.SelectedItem == null) return;

            Close();
        }
    }
}

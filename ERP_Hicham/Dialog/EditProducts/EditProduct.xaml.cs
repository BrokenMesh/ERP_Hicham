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

namespace ERP_Hicham.Dialog.EditProducts
{
    /// <summary>
    /// Interaktionslogik für EditProduct.xaml
    /// </summary>
    public partial class EditProduct : Window
    {
        public EditProduct()
        {
            InitializeComponent();

            UpdateList();
            lvProduct_SelectionChanged(null,null);
        }
       
        public static void Open(Grid _parent) {
            _parent.IsEnabled = false;

            EditProduct _window = new EditProduct();
            _window.ShowDialog();

            _parent.IsEnabled = true;
        }

        public void UpdateList() {
            List<Product> _products = MainWindow.instance.DB.QueryAllProducts();
            lvProduct.ItemsSource = _products;
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e) {
            Product? _product = CreateProduct.Dialog(Main_Grid, null);
            if (_product == null) return;

            MainWindow.instance.DB.CreateProduct(_product);

            UpdateList();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e) {
            Product? _product = lvProduct.SelectedItem as Product;
            if (_product == null) return; ;

            bool _res = Confirm.Confirm.PromptQuestion(Main_Grid, $"Wollen sie wirklich das Produkt {_product.Name}", "Löschen");
            if (!_res) return;

            string _depSTR = "";
            _res = MainWindow.instance.DB.RemoveProduct(_product.ID, ref _depSTR);

            if (!_res) {
                Confirm.Confirm.PromptQuestion(Main_Grid, $"Des Produkt kann nicht gelöscht werden. Es ist noch an\n Rechnungen gebunden. billID: {_depSTR}", "Löschen");
            }

            UpdateList();
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e) {
            Product? _curProduct = lvProduct.SelectedItem as Product;
            if (_curProduct == null) return;

            Product? _product = CreateProduct.Dialog(Main_Grid, _curProduct);
            if (_product == null) return;

            MainWindow.instance.DB.UpdateProduct(_product, _curProduct.ID);

            UpdateList();
        }


        private void Close_Button_Click(object sender, RoutedEventArgs e) {
            Close();
        }


        private void lvProduct_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (lvProduct.SelectedItem == null) {
                Delete_Button.IsEnabled = false;
                Edit_Button.IsEnabled = false;
            }
            else {
                Delete_Button.IsEnabled = true;
                Edit_Button.IsEnabled = true;
            }

        }
    }
}

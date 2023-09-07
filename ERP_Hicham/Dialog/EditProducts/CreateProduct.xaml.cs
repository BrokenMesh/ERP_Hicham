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
    /// Interaktionslogik für CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        public static Product? product;

        public CreateProduct(Product? _original)
        {
            InitializeComponent();

            if (_original != null) {
                Cost_Input.Text = _original.Cost.ToString();
                Name_Input.Text = _original.Name;

                Title = "Bearbeiten";
                WindowTitle_Lable.Content = "Bearbeiten";
                Create_Button.Content = "Bearbeiten";
            }
        }

        public static Product? Dialog(Grid _parent, Product? _original) {
            _parent.IsEnabled = false;

            product = null;

            CreateProduct _dialog = new CreateProduct(_original);
            _dialog.ShowDialog();

            _parent.IsEnabled = true;

            return product;
        }


        private void Create_Button_Click(object sender, RoutedEventArgs e) {
            float _cost;
            if (!float.TryParse(Cost_Input.Text, out _cost)) return;

            product = new Product(0, Name_Input.Text, _cost);

            bool res = Confirm.Confirm.PromptQuestion(Main_Grid, $"Wollen sie das Produkt {product.Name} Erstellen/Bearbeiten.", "Erstellen");
            if (!res) return;

            Close();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e) {
            product = null;

            Close();
        }
    }
}

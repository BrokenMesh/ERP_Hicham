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

namespace ERP_Hicham.Dialog.Confirm
{
    /// <summary>
    /// Interaktionslogik für Confirm.xaml
    /// </summary>
    public partial class Confirm : Window
    {
        public static bool response;

        public static bool PromptQuestion(Grid _parent, string _question, string _title) {
            _parent.IsEnabled = false;

            Confirm confirm = new Confirm(_question, _title);
            confirm.ShowDialog();

            _parent.IsEnabled = true;

            return response;
        }

        public Confirm(string _question, string _title)
        {
            InitializeComponent();

            Title = _title;
            Dialog_Text.Content = _question;
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e) {
            response = true;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e) {
            response = false;
            Close();
        }
    }
}

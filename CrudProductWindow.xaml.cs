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

namespace ADO_202
{
    /// <summary>
    /// Interaction logic for CrudProductWindow.xaml
    /// </summary>
    public partial class CrudProductWindow : Window
    {
        public Entity.Product EditedProduct { get; private set; }

        public CrudProductWindow(Entity.Product EditedProduct)
        {
            InitializeComponent();
            this.EditedProduct = EditedProduct;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EditedProduct is null)  // Create - створення нового товару
            {
                EditedProduct = new() { Id = Guid.NewGuid() };
                DeleteButton.IsEnabled = false;
            }
            else  // є товар, переданий на редагування/видалення
            {               
                ViewName.Text  = EditedProduct.Name;
                ViewPrice.Text = EditedProduct.Price.ToString();
                DeleteButton.IsEnabled = true;
            }
            ViewId.Text = EditedProduct.Id.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            EditedProduct.Name = ViewName.Text;
            EditedProduct.Price = Convert.ToDouble(ViewPrice.Text);
            this.DialogResult = true;   // встановлює результат ShowDialog() та закриває вікно
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            EditedProduct = null!;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

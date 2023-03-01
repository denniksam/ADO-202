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
    /// Interaction logic for CrudSaleWindow.xaml
    /// </summary>
    public partial class CrudSaleWindow : Window
    {
        public Entity.Sale? Sale { get; set; }

        public CrudSaleWindow(Entity.Sale? Sale)
        {
            this.Sale = Sale;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Owner;
            if (Sale is null)
            {
                Sale = new();  // Id, SaleDt, Cnt  створюється у конструкторі 
                DeleteButton.IsEnabled = false;
            }
            else
            {
                if (Owner is OrmWindow owner)
                {
                    ProductCombobox.SelectedItem =
                        owner.Products
                        .FirstOrDefault(p => p.Id == Sale.ProductId);
                    ManagerCombobox.SelectedItem =
                        owner.Managers
                        .FirstOrDefault(m => m.Id == Sale.ManagerId);
                }
                else
                {
                    MessageBox.Show("Owner is NOT OrmWindow");
                    Close();
                }
            }
            ViewId.Text     = Sale.Id.ToString();
            ViewSaleDt.Text = Sale.SaleDt.ToString();
            ViewCnt.Text    = Sale.Cnt.ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Sale is null) return;
            if( ViewCnt.Text.Equals(String.Empty) )
            {
                MessageBox.Show("Зазначте кількість проданого товару");
                ViewCnt.Focus();
                return;
            }
            int cnt;
            try { cnt = Convert.ToInt32(ViewCnt.Text); }
            catch
            {
                MessageBox.Show("Кількість не розпізнана. Очікується число");
                ViewCnt.Focus();
                return;
            }
            if( ProductCombobox.SelectedItem is null )
            {
                MessageBox.Show("Виберіть товар");
                ProductCombobox.Focus();
                return;
            }
            if (ManagerCombobox.SelectedItem is null)
            {
                MessageBox.Show("Виберіть продавця");
                ManagerCombobox.Focus();
                return;
            }
            this.Sale.Cnt = cnt;

            if(ProductCombobox.SelectedItem is Entity.Product product)
            {
                this.Sale.ProductId = product.Id;
            }
            else
            {
                MessageBox.Show("Помилка вибору товару");
                return;
            }

            if (ManagerCombobox.SelectedItem is Entity.Manager man)
            {
                this.Sale.ManagerId = man.Id;
            }
            else
            {
                MessageBox.Show("Помилка вибору продавця");
                return;
            }

            this.DialogResult = true;   // встановлює результат ShowDialog() та закриває вікно
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

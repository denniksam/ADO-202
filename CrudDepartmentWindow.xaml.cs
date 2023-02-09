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
    /// Interaction logic for CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        public Entity.Department EditedDepartment { get; private set; }

        public CrudDepartmentWindow( Entity.Department department )
        {
            InitializeComponent();
            this.EditedDepartment = department;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(EditedDepartment != null) 
            { 
                ViewId.Text = EditedDepartment.Id.ToString();
                ViewName.Text = EditedDepartment.Name;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            EditedDepartment.Name = ViewName.Text;
            this.DialogResult = true;   // встановлює результат ShowDialog() та закриває вікно
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            EditedDepartment = null!;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

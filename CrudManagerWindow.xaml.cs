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
    /// Interaction logic for CrudManagerWindow.xaml
    /// </summary>
    public partial class CrudManagerWindow : Window
    {
        public Entity.Manager? EditedManager;

        public CrudManagerWindow(Entity.Manager? EditedManager)
        {
            InitializeComponent();
            this.EditedManager = EditedManager;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Owner;
            if(EditedManager is null)
            {
                EditedManager = new();  // новий ід створюється у конструкторі Manager
                DeleteButton.IsEnabled = false;
            }
            else
            {
                ViewSurname.Text = EditedManager.Surname;
                ViewName.Text = EditedManager.Name;
                ViewSecname.Text = EditedManager.Secname;

                if(Owner is OrmWindow owner)
                {
                    MainDepCombobox.SelectedItem = 
                        owner
                        .Departments
                        .FirstOrDefault(dep => dep.Id == EditedManager.IdMainDep);
                    SecDepCombobox.SelectedItem =
                        owner
                        .Departments
                        .FirstOrDefault(dep => dep.Id == EditedManager.IdSecDep);
                    ChiefCombobox.SelectedItem =
                        owner
                        .Managers
                        .FirstOrDefault(man => man.Id == EditedManager.IdChief);
                }
                else
                {
                    MessageBox.Show("Owner is NOT OrmWindow");
                    Close();
                }
                
                
            }
            ViewId.Text = EditedManager.Id.ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
/* Д.З. Доробити CRUD співробітників
 * - додати кнопки "скинути" для комбобоксів, що дозволяють NULL
 * - реалізувати SQL запити у параметричній формі
 * - додати поле FiredDt (звільнення замість видалення з БД),
 *    вживати його як маркер видалення
 * 
 */
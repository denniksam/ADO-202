using ADO_202.DAL;
using ADO_202.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ADO_202.View
{
    /// <summary>
    /// Interaction logic for DalWindow.xaml
    /// </summary>
    public partial class DalWindow : Window
    {
        public ObservableCollection<Entity.Department> DepartmentsList { get; set; }
        public ObservableCollection<Entity.Manager> ManagersList { get; set; }

        private readonly DataContext _context;

        public DalWindow()
        {
            InitializeComponent();
            _context = new();
            DepartmentsList = new(_context.Departments.GetAll());
            ManagersList = new(_context.Managers.GetAll());
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // MessageBox.Show(DepartmentsList.Count.ToString());
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            dialog.ShowDialog();
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Department department)
                {
                    CrudDepartmentWindow dialog = new(department);
                    if (dialog.ShowDialog() == true)  // виконана (підтверджена) дія
                    {
                        if (dialog.EditedDepartment is null)  // дія - видалення
                        {
                            DepartmentsList.Remove(department);
                            MessageBox.Show("Видалення: " + department.Name);
                        }
                        else  // дія - збереження
                        {
                            int index = DepartmentsList.IndexOf(department);
                            DepartmentsList.Remove(department);
                            DepartmentsList.Insert(index, department);
                            MessageBox.Show("Оновлення: " + department.Name);
                        }
                    }
                    else  // вікно закрите або натиснуто Cancel
                    {
                        MessageBox.Show("Дію скасовано");
                    }
                }
            }
        }
        
        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    MessageBox.Show(manager.Name);
                }
            }
        }

        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

using ADO_202.EFCore;
using Microsoft.EntityFrameworkCore;
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

namespace ADO_202.View
{
    /// <summary>
    /// Interaction logic for EfWindow.xaml
    /// </summary>
    public partial class EfWindow : Window
    {
        private EfContext efContext;

        public EfWindow()
        {
            InitializeComponent();
            efContext = new();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // У EFCore - Lazy load - дані не завантажені до запитів
            efContext.Departments.Load();   // завантаження даних
            DepartmentsList.ItemsSource = 
                efContext.Departments.Local.ToObservableCollection();

            MonitorBlock.Text = "Departments: " +
                efContext.Departments.Count();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                // dialog.EditedDepartment - Entity.Department
                efContext.Departments.Add(
                    new Department()   // EfCore.Department
                    {
                        Id = dialog.EditedDepartment.Id,
                        Name = dialog.EditedDepartment.Name
                    });

                // !! Зміна колекцій контексту не вносить зміни у БД (і у контекст) - 
                //    тільки планує ці зміни
                efContext.SaveChanges();   // фіксація усіх змін

                MonitorBlock.Text += "\nDepartments: " +
                    efContext.Departments.Count();
            }
        }
    }
}
/*
 * Д.З. 
 * - Додати до контексту даних сутності Manager та Sale,
 * - Створити та застосувати міграції (можна однією міграцією після 
 *     додавання обох сутностей)
 * - Реалізувати Update та Delete для Department
 */
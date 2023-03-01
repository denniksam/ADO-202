using ADO_202.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
    /// Interaction logic for OrmWindow.xaml
    /// </summary>
    public partial class OrmWindow : Window
    {
        public ObservableCollection<Entity.Department> Departments { get; set; }
        public ObservableCollection<Entity.Manager> Managers { get; set; }
        public ObservableCollection<Entity.Product> Products { get; set; }
        public ObservableCollection<Entity.Sale> Sales { get; set; }

        private readonly SqlConnection _connection;

        public OrmWindow()
        {
            InitializeComponent();
            Departments = new();
            Managers = new();
            Products = new();
            Sales = new();
            this.DataContext = this;   // місце пошуку {Binding Departments}
            _connection = new(App.ConnectionString);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();
                SqlCommand cmd = new() { Connection = _connection };

                #region Load Departments
                cmd.CommandText = "SELECT D.Id, D.Name FROM Departments D";
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Departments.Add(
                        new Entity.Department
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                }
                reader.Close();
                #endregion

                #region Load Products
                cmd.CommandText = "SELECT P.* FROM Products P WHERE P.DeleteDt IS NULL";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Products.Add( new(reader) );
                }
                reader.Close();
                #endregion

                #region Load Managers
                cmd.CommandText = "SELECT M.Id, M.Surname, M.Name, M.Secname, M.Id_main_Dep, M.Id_sec_dep, M.Id_chief FROM Managers M";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Managers.Add(
                        new Entity.Manager
                        {
                            Id = reader.GetGuid(0),
                            Surname = reader.GetString(1),
                            Name = reader.GetString(2),
                            Secname = reader.GetString(3),
                            IdMainDep = reader.GetGuid(4),
                            IdSecDep = reader.GetValue(5) == DBNull.Value
                                        ? null
                                        : reader.GetGuid(5),
                            IdChief = reader.IsDBNull(6)
                                        ? null
                                        :reader.GetGuid(6)
                        });
                }
                reader.Close();
                #endregion

                #region Load Sales
                cmd.CommandText = "SELECT S.* FROM Sales S";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Sales.Add(new(reader));
                }
                reader.Close();
                #endregion

                cmd.Dispose();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Window will be closed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)
            {
                if(item.Content is Entity.Department department) 
                {
                    CrudDepartmentWindow dialog = new(department);
                    if (dialog.ShowDialog() == true)  // виконана (підтверджена) дія
                    {
                        if(dialog.EditedDepartment is null)  // дія - видалення
                        {
                            Departments.Remove(department);
                            MessageBox.Show("Видалення: " + department.Name);
                        }
                        else  // дія - збереження
                        {
                            int index = Departments.IndexOf(department);
                            Departments.Remove(department);
                            Departments.Insert(index, department);
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
        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            CrudDepartmentWindow dialog = new(null!);
        }

        private void ManagersItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Manager manager)
                {
                    CrudManagerWindow dialog = new(manager) { Owner = this };
                    if(dialog.ShowDialog() == true)
                    {
                        
                    }

                    // MessageBox.Show(manager.Surname);
                }
            }
        }
        
        private void ProductsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Product product)
                {
                    CrudProductWindow dialog = new(product);
                    if (dialog.ShowDialog() == true)  // виконана (підтверджена) дія
                    {
                        if (dialog.EditedProduct is null)  // дія - видалення
                        {
                            using SqlCommand cmd = new() { Connection = _connection };
                            cmd.CommandText = "UPDATE Products SET DeleteDt = CURRENT_TIMESTAMP WHERE Id = @id ";
                            cmd.Parameters.AddWithValue("@id", product.Id);
                            try
                            {
                                cmd.ExecuteNonQuery();
                                Products.Remove(product);
                                MessageBox.Show("Видалення: " + product.Name);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            
                        }
                        else  // дія - збереження
                        {
                            int index = Products.IndexOf(product);
                            Products.Remove(product);
                            Products.Insert(index, product);
                            MessageBox.Show("Оновлення: " + product.Name);
                        }
                    }
                    else  // вікно закрите або натиснуто Cancel
                    {
                        MessageBox.Show("Дію скасовано");
                    }
                    // MessageBox.Show(product.Name + " " + product.Price);
                }
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            CrudProductWindow dialog = new(null!);
            if (dialog.ShowDialog() == true)
            {
                if(dialog.EditedProduct is not null)
                {
                    /* String sql = "INSERT INTO Products(Id, Name, Price) VALUES" +
                        $"('{dialog.EditedProduct.Id}', N'{dialog.EditedProduct.Name}', {dialog.EditedProduct.Price})";
                    using SqlCommand cmd = new(sql, _connection);
                    */
                    String sql = "INSERT INTO Products(Id, Name, Price) VALUES(@id, @name, @price)";
                    using SqlCommand cmd = new(sql, _connection);
                    cmd.Parameters.AddWithValue("@id", dialog.EditedProduct.Id);
                    cmd.Parameters.AddWithValue("@name", dialog.EditedProduct.Name);
                    cmd.Parameters.AddWithValue("@price", dialog.EditedProduct.Price);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Products.Add(dialog.EditedProduct);
                        MessageBox.Show("Додано: " + dialog.EditedProduct.Name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else  // вікно закрите або натиснуто Cancel
            {
                MessageBox.Show("Дію скасовано");
            }
        }

        private void SalesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem item)
            {
                if (item.Content is Entity.Sale sale)
                {
                   //CrudManagerWindow dialog = new(manager) { Owner = this };
                   //if (dialog.ShowDialog() == true)
                   //{
                   //
                   //}

                    // MessageBox.Show(manager.Surname);
                }
            }
        }
        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            CrudSaleWindow dialog = new(null) { Owner = this };
            if (dialog.ShowDialog() == true && dialog.Sale is not null)
            {
                using SqlCommand cmd = new(
                    "INSERT INTO Sales(Id, ProductId, ManagerId, Cnt, SaleDt) " +
                    "VALUES (@Id, @ProductId, @ManagerId, @Count, @SaleDt)", 
                    _connection);

                cmd.Parameters.AddWithValue("@Id",dialog.Sale.Id);
                cmd.Parameters.AddWithValue("@ProductId", dialog.Sale.ProductId);
                cmd.Parameters.AddWithValue("@ManagerId", dialog.Sale.ManagerId);
                cmd.Parameters.AddWithValue("@Count", dialog.Sale.Cnt);
                cmd.Parameters.AddWithValue("@SaleDt", dialog.Sale.SaleDt);

                try
                {
                    cmd.ExecuteNonQuery();
                    Sales.Add(dialog.Sale);
                    MessageBox.Show("Додано успішно");
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("Помилка додавання: " + ex.Message);
                }
            }
        }
    }
}
/* Д.З. Реалізувати роботу CRUD :
 * за натиском кнопки Save
 * - перевірити нову назву на порожність, видати повідомлення
 * - перевірити нову назву на збіг з попередньою (немає змін) - Cancel
 * - у разі OK подати команду БД на зміну назви відділу, повідомити про її
 *     результат (успіх/помилка)
 * за кнопкою Delete
 * - запитати підтвердження (Певні? так/ні)
 * - у разі OK подати команду БД на зміну назви відділу, повідомити про її
 *     результат (успіх/помилка)
 * у OrmWindow поруч з переліком відділів додати кнопку "Створити новий відділ",
 * реалізувати її роботу
 * - згенерувати новий Id
 * - запитати назву, перевірити а) на порожність, б) на наявність у БД такої ж
 * - ...
 */
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SqlClient;   // не забути NuGet
using System.Data;
using MySql.Data.MySqlClient;  // окремий NuGet
using System.IO;

namespace ADO_202
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // об'єкт-підключення, основа ADO
        private SqlConnection _connection;  // MS SQL ADO
        private MySqlConnection _myConnection;  // MySQL ADO
        public MainWindow()
        {
            InitializeComponent();
            // !! створення об'єкта не відкиває підключення
            _connection = new();
            // головний параметр - рядок підключення
            _connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\_dns_\source\repos\ADO-202\ADO-202.mdf;Integrated Security=True";
            _myConnection = new(File.ReadAllText("my_con.txt"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try  // відкриваємо підключення
            {
                _connection.Open();
                // відображаємо статус підключення на вікні
                StatusConnection.Content = "Connected";
                StatusConnection.Foreground = Brushes.Green;
            }
            catch(SqlException ex)
            {
                // відображаємо статус підключення на вікні
                StatusConnection.Content = "Disconnected";
                StatusConnection.Foreground = Brushes.Red;

                MessageBox.Show(ex.Message);
                this.Close();
            }
            try  // відкриваємо підключення
            {
                // _myConnection.Open();
                // відображаємо статус підключення на вікні
                StatusMyConnection.Content = "Connected";
                StatusMyConnection.Foreground = Brushes.Green;
            }
            catch (MySqlException ex)
            {
                // відображаємо статус підключення на вікні
                StatusMyConnection.Content = "Disconnected";
                StatusMyConnection.Foreground = Brushes.Red;

                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowMonitor();
            ShowDepartmentsView();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(_connection?.State == ConnectionState.Open ) 
            { 
                _connection.Close();
            }
        }

        #region Запити без повернення результатів
        private void CreateDepartments_Click(object sender, RoutedEventArgs e)
        {
            // команда - ресурс для передачі SQL запиту до СУБД
            SqlCommand cmd = new();
            // Обов'язкові параметри команди:
            cmd.Connection = _connection;  // відкрите підключення
            cmd.CommandText =              // та текст SQL запиту
                @"CREATE TABLE Departments (
	                Id	 UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                Name NVARCHAR(50) NOT NULL
                )";
            /* MySql: CREATE TABLE Departments(
                 Id          CHAR(36) NOT NULL PRIMARY KEY,
                 Name        VARCHAR(50) NOT NULL
             ) ENGINE = INNODB DEFAULT CHARSET = UTF8 */
            try
            {
                cmd.ExecuteNonQuery();  // NonQuery - без повернення рез-ту
                MessageBox.Show("Create OK");
            }
            catch(SqlException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Create error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
            }
            cmd.Dispose();  // команда - unmanaged, потрібно вивільняти ресурс
        }

        private void CreateProducts_Click(object sender, RoutedEventArgs e)
        {
            String sql = 
                @"CREATE TABLE Products (
	                Id			UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	                Name		NVARCHAR(50) NOT NULL,
	                Price		FLOAT  NOT NULL
                )";
            using SqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Create Products OK");
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillProducts_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/fill_products.sql") ;
            using SqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fill Products OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateManagers_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/create_managers.sql");
            using SqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Create Managers OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillManagers_Click(object sender, RoutedEventArgs e)
        {
            String sql = File.ReadAllText("sql/fill_managers.sql");
            using SqlCommand cmd = new(sql, _connection);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Fill Managers OK");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Запити з одним (скалярним) результатом
        private void ShowMonitor()
        {
            ShowMonitorDepartments();
            ShowMonitorProducts();
            ShowMonitorManagers();
        }
        private void ShowMonitorDepartments()
        {
            using SqlCommand cmd = new("SELECT COUNT(1) FROM Departments", _connection);
            try
            {
                object res = cmd.ExecuteScalar();   // Виконання запиту + повернення
                // "лівого-верхнього" результату з поверненої таблиці
                // повертає типізовані дані (число, рядок, дату-час, тощо), але
                // у формі object, відповідно для використання потрібне перетворення
                int cnt = Convert.ToInt32(res);
                StatusDepartments.Content = cnt.ToString();
            }
            catch(SqlException ex)  // помилка запиту
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusDepartments.Content = "---";
            }
            catch(Exception ex)  // інші помилки (перетворення типів)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusDepartments.Content = "---";
            }
        }

        private void ShowMonitorProducts()
        {
            using SqlCommand cmd = new("SELECT COUNT(1) FROM Products", _connection);
            try
            {
                object res = cmd.ExecuteScalar();
                int cnt = Convert.ToInt32(res);
                StatusProducts.Content = cnt.ToString();
            }
            catch (SqlException ex)  // помилка запиту
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusDepartments.Content = "---";
            }
            catch (Exception ex)  // інші помилки (перетворення типів)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusDepartments.Content = "---";
            }
        }
        private void ShowMonitorManagers()
        {
            using SqlCommand cmd = new("SELECT COUNT(1) FROM Managers", _connection);
            try
            {
                object res = cmd.ExecuteScalar();
                int cnt = Convert.ToInt32(res);
                StatusManagers.Content = cnt.ToString();
            }
            catch (SqlException ex)  // помилка запиту
            {
                MessageBox.Show(ex.Message, "SQL error",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
                StatusManagers.Content = "---";
            }
            catch (Exception ex)  // інші помилки (перетворення типів)
            {
                MessageBox.Show(ex.Message, "Cast error",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                StatusManagers.Content = "---";
            }
        }
        #endregion

        #region Запити із табличними результатами
        private void ShowDepartmentsView()
        {
            using SqlCommand cmd = new("SELECT * FROM Departments", _connection);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                String str = String.Empty;
                // Передача даних відбувається по одному рядку
                while (reader.Read())  // зчитує рядок, якщо немає - false
                {
                    // рядок зчитується у сам reader, дані з нього можна дістати
                    // а) через гет-тери
                    // б) через індексатори
                    str += reader.GetGuid(0)    // типізований Get-тер: рекомендовано
                        + "  "                  // 
                        + reader[1]             // індексатор - object
                        + "\n";                 // відлік від 0 по порядку полів у результаті
                    // TODO: реалізувати скорочене відображення id типу a8f2...2c
                }
                ViewDepartments.Text = str;
                reader.Close();   // !! Незакритий reader блокує інші команди до БД
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }
}

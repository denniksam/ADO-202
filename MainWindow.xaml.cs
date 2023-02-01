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

namespace ADO_202
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // об'єкт-підключення, основа ADO
        private SqlConnection _connection;

        public MainWindow()
        {
            InitializeComponent();
            // !! створення об'єкта не відкиває підключення
            _connection = new();
            // головний параметр - рядок підключення
            _connection.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\_dns_\source\repos\ADO-202\ADO-202.mdf;Integrated Security=True";
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
             )*/
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
        #endregion
    }
}

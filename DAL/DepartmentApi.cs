using ADO_202.Entity;
using ADO_202.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.DAL
{
    internal class DepartmentApi  // DAO - Data Access Object
    {
        private readonly SqlConnection _connection;
        private readonly ILogger _logger;

        public DepartmentApi(SqlConnection connection)
        {
            _connection = connection;
            _logger = App.Logger;
        }

        public List<Entity.Department> GetAll()
        {
            var departments = new List<Department>();
            try
            {
                using SqlCommand command = new(
                    "SELECT D.* FROM Departments D", 
                    _connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new(reader));
                }
            }
            catch (Exception ex)  // Logging - правильний спосіб оброблення виключень
            {
                _logger.Log(ex.Message, "SEVERE",
                    this.GetType().Name, MethodInfo.GetCurrentMethod()?.Name ?? "");
            }
            return departments;
        }
    }
}
/* Д.З. Реалізувати методи DepartmentApi
 *  Add(Entity.Department department)
 *  Update(Entity.Department department) // id - ключ, інше - на оновлення
 *  Delete(Entity.Department department)
 */
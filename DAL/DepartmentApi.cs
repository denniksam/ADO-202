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
        private readonly DataContext _context;

        private List<Department> departments;

        public DepartmentApi(SqlConnection connection, DataContext context)
        {
            _connection = connection;
            _logger = App.Logger;
            _context = context;
            departments = null!;  // Lazy - колекція буде побудована з першим запитом
        }
        /// <summary>
        /// Returns list of Departments from DB
        /// </summary>
        /// <param name="forceReload">Defines use of cached list or to reload DB</param>
        /// <returns></returns>
        public List<Entity.Department> GetAll(bool forceReload = false)
        {
            if(departments is not null    // якщо раніше колекція створювалась
                && !forceReload)          // та не вимагається примусового перечитування
            {
                return departments;       //  то повертаємо її
            }

            departments = new();
            try
            {
                using SqlCommand command = new(
                    "SELECT D.* FROM Departments D", 
                    _connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    departments.Add(new(reader) { _dataContext = _context });
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
﻿using ADO_202.Entity;
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
    internal class ManagerApi
    {
        private readonly SqlConnection _connection;
        private readonly ILogger _logger;
        private readonly DataContext _context;

        public ManagerApi(SqlConnection connection, DataContext context)
        {
            _connection = connection;
            _logger = App.Logger;
            _context = context;
        }

        public List<Entity.Manager> GetAll(bool includeDeleted = false)
        {
            var list = new List<Manager>();
            String sql = "SELECT M.* FROM Managers M" +
                (includeDeleted ? "" : " WHERE M.FiredDt IS NULL" );
            try
            {
                using SqlCommand command = new(sql, _connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new(reader) { _dataContext = _context });
                }
            }
            catch (Exception ex) 
            {
                _logger.Log(ex.Message, "SEVERE",
                    this.GetType().Name, 
                    MethodInfo.GetCurrentMethod()?.Name ?? "");
            }
            return list;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Entity
{
    public class Product
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public double Price { get; set; }
        public DateTime? DeleteDt { get; set; }

        public Product()
        {
            Id = Guid.NewGuid();
            Name = null!;
            DeleteDt = null;
        }
        public Product(SqlDataReader reader)
        {
            Id = reader.GetGuid("Id");
            Name = reader.GetString("Name");
            Price = reader.GetDouble("Price");
            DeleteDt = reader.IsDBNull("DeleteDt")
                        ? null
                        : reader.GetDateTime("DeleteDt");
        }
    }
}

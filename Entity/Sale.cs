using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Entity
{
    public class Sale
    {
        public Guid       Id           { get; set; }
        public Guid       ProductId    { get; set; }
        public Guid       ManagerId    { get; set; }
        public Int32      Cnt          { get; set; }
        public DateTime   SaleDt       { get; set; }
        public DateTime?  DeleteDt     { get; set; }

        public Sale()
        {
            Id = Guid.NewGuid();
            Cnt = 1;
            SaleDt = DateTime.Now;
        }
        public Sale(SqlDataReader reader)
        {
            Id        = reader.GetGuid(nameof(Id));
            ProductId = reader.GetGuid("ProductId"); 
            ManagerId = reader.GetGuid("ManagerId"); 
            Cnt       = reader.GetInt32("Cnt"); 
            SaleDt    = reader.GetDateTime("SaleDt");
            DeleteDt  = reader.IsDBNull("DeleteDt") 
                        ? null 
                        : reader.GetDateTime("DeleteDt");
        }
    }
}
/* CREATE TABLE Sales (
	Id		  UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	ProductId UNIQUEIDENTIFIER NOT NULL REFERENCES Products(Id),
	ManagerId UNIQUEIDENTIFIER NOT NULL REFERENCES Managers(Id),
	Cnt       INT              NOT NULL DEFAULT 1,
	SaleDt    DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
	DeleteDt  DATETIME         NULL
)
 */
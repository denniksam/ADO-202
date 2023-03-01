using ADO_202.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Entity
{
    public class Manager
    {
        public Guid      Id        { get; set; }
        public String    Surname   { get; set; }
        public String    Name      { get; set; }
        public String    Secname   { get; set; }
        public Guid      IdMainDep { get; set; }    // NOT NULL
        public Guid?     IdSecDep  { get; set; }    // NULL
        public Guid?     IdChief   { get; set; }    // NULL
        public DateTime? FiredDt   { get; set; }    // NULL

        public Manager()
        {
            Id = Guid.NewGuid();
            Surname = null!;
            Name = null!;
            Secname = null!;
        }

        public Manager(DbDataReader reader)
        {
            Id        = reader.GetGuid("Id");
            Name      = reader.GetString("Name");
            Surname   = reader.GetString("Surname");
            Secname   = reader.GetString("Secname");
            IdMainDep = reader.GetGuid("Id_main_dep");
            IdSecDep  = reader.GetValue("Id_sec_dep") == DBNull.Value 
                        ? null 
                        : reader.GetGuid("Id_sec_dep");
            IdChief   = reader.IsDBNull("Id_chief") 
                        ? null 
                        : reader.GetGuid("Id_chief");
            FiredDt   = reader.IsDBNull("FiredDt") 
                        ? null 
                        : reader.GetDateTime("FiredDt");
        }

        //////////////////// NAVIGATION PROPERTIES //////////////////////

        internal DataContext? _dataContext { get; init; }  // залежність - посилання на контекст даних

        public Department? MainDep    // навігаційна властивість
        {
            get
            {
                return _dataContext?  // TODO: реалізувати у DepartmentsApi
                    .Departments      // метод пошуку відділу за Id
                    .GetAll()         // (!! не через SQL, а зі своєї колекції)
                    .Find(dep => dep.Id == this.IdMainDep);
            }
        }
    }
}

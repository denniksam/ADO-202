using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.EFCore
{
    public class Manager
    {
        public Guid Id { get; set; }
        public String Surname { get; set; }
        public String Name { get; set; }
        public String Secname { get; set; }
        public Guid IdMainDep { get; set; }    // NOT NULL
        public Guid? IdSecDep { get; set; }    // NULL
        public Guid? IdChief { get; set; }    // NULL
        public DateTime? FiredDt { get; set; }    // NULL


        /////////////////// NAVIGATION PROPERTIES ////////////////////////

        public Department MainDep { get; set; }  // Reference prop
        public Department SecDep { get; set; }  // Reference prop
        public List<Sale> Sales { get; set; }
        public List<Product> Products { get; set; }
    }
}

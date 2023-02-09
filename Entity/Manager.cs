using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Entity
{
    public class Manager
    {
        public Guid   Id        { get; set; }
        public String Surname   { get; set; }
        public String Name      { get; set; }
        public String Secname   { get; set; }
        public Guid   IdMainDep { get; set; }    // NOT NULL
        public Guid?  IdSecDep  { get; set; }    // NULL
        public Guid?  IdChief   { get; set; }    // NULL
    }
}

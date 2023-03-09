using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.EFCore
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ManagerId { get; set; }
        public Int32 Cnt { get; set; }
        public DateTime SaleDt { get; set; }
        public DateTime? DeleteDt { get; set; }
    }
}

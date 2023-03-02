using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.EFCore
{
    public class EfContext : DbContext
    {
        public DbSet<EFCore.Department> Departments { get; set; }
        public DbSet<EFCore.Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ef202ado;Integrated Security=True"
            );
        }
    }
}

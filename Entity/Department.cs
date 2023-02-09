using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_202.Entity
{
    public class Department                  // Entity - клас, що відображає таблицю БД (рядок таблиці)
    {                                        // дана сутність - для таблиці Departments
        public Guid Id { get; set; }         // відбраження поля Id	 UNIQUEIDENTIFIER
        public String Name { get; set; }     // відбраження поля Name NVARCHAR(50)
    }                                        // ! типи БД та мови як правило відрізняються
}

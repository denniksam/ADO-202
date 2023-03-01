using ADO_202.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ADO_202
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly String ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\_dns_\source\repos\ADO-202\ADO-202.mdf;Integrated Security=True";
        internal static readonly ILogger Logger = new FileLogger();
    }
}

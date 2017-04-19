using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GBCService.Models
{
    class DbConn :DbContext
    {
        public static SqlConnection scon;

        public DbSet<HrMaster> Db_hr { get; set; }
    }
}

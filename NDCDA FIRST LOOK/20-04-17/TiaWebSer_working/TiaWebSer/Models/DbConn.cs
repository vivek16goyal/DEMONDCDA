using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TiaWebSer.Models
{
    public class DbConn : DbContext
    {
        public static SqlConnection scon;

        public DbSet<HrMaster> Db_hr { get; set; }
        public DbSet<MultipleFirm> Db_Multi { get; set; }
        public DbSet<PO> Db_PO { get; set; }
        public DbSet<statemaster> Db_state { get; set; }
        public DbSet<citymaster> Db_city { get; set; }
        public DbSet<areamst> Db_area { get; set; }
    }
}
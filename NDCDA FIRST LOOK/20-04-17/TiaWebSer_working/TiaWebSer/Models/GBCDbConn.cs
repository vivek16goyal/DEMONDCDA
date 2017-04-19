using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TiaWebSer.Models
{
    public class GBCDbConn : DbContext
    {
        public DbSet<ACPMST> Db_acpmst { get; set; }
        public DbSet<ACPMST_temp> ACPMST_temp { get; set; }
        public DbSet<AreaMaster> Db_areamst { get; set; }
        public DbSet<IM> Db_itm { get; set; }
        public DbSet<RM> Db_rm { get; set; }
        public DbSet<OB> Db_ob { get; set; }
        public DbSet<OI> Db_OI { get; set; }
        public DbSet<CO> Db_CO { get; set; }
        public DbSet<CR> Db_Cr { get; set; }
        public DbSet<PRINTQ> Db_printQ { get; set; }
        public DbSet<VC> Db_vc { get; set; }
        public DbSet<TmpSI> Db_tmpSI { get; set; }
    }
}
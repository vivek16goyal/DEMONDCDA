using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;


namespace TiaWebSer.Models
{
    class GBCCon : DbContext
    {
        public DbSet<ServiceReg> db_SerReg { get; set; }

    }
}

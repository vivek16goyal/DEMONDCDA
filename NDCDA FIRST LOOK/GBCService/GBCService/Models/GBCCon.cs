using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web; 


namespace GBCService.Models
{
    class GBCCon:DbContext
    {
       public DbSet<ServiceReg> db_SerReg { get; set; }
       public DbSet<AppSMSStatus> db_AppSms { get; set; }
       public DbSet<PatientMaster> db_patient { get; set; }
       public DbSet<OB> db_ob { get; set; }
       public DbSet<OI> db_oi { get; set; }
       public DbSet<statemaster> Db_state { get; set; }
       public DbSet<citymaster> Db_city { get; set; }
       public DbSet<areamst> Db_area { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace TiaWebSer.Models
{
    class DAl
    {
        //change vivek
        public DataTable getDataNew_code(string StrSql)
        {
            string Conn, str;
            Conn = System.Configuration.ConfigurationManager.ConnectionStrings["GBCDbConn"].ToString();
            str = Conn;
            SqlDataAdapter adp = new SqlDataAdapter(StrSql, str);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        //change up
        
    }
}

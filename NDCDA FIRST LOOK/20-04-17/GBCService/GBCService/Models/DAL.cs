using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GBCService.Models
{
    class DAL
    {
        public bool ExecuteQuery(string StrSql)
        {
            string Conn = System.Configuration.ConfigurationManager.ConnectionStrings["GBCCon"].ToString();
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(Conn);
            con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = StrSql;
            int intRows = cmd.ExecuteNonQuery();
            con.Close();
            if (intRows >= 0)
                return true;
            else
                return false;
        }
        public DataTable getDataTable(string StrSql)
        {
            string Conn, str;
            Conn = System.Configuration.ConfigurationManager.ConnectionStrings["GBCCon"].ToString();
            str = Conn;
            SqlDataAdapter adp = new SqlDataAdapter(StrSql, str);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
    }
}

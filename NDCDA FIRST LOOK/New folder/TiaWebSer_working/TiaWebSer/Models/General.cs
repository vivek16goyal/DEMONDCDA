using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using TiaWebSer.Models;
using TIA3T.BLL;
using TIA3T.DAL;
using TIA3T.NEW.BLL;

namespace TiaWebSer.Models
{
    [Table("TiaERPAppReg")]
    public class ServiceReg
    {
        [Key]
        public string Pcode { get; set; }
        public string PType { get; set; }
        public string ServicePath { get; set; }
        public string InetAddress { get; set; }
        public string MCAddress { get; set; }
        public string Port { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? ValidUpToDate { get; set; }
        public DateTime? ServiceLastActiveDate { get; set; }
        public DateTime? AppLastActiveDate { get; set; }
        public int? PingCount { get; set; }
        public bool? IsActive { get; set; }
        public bool? ServiceStatus { get; set; }
        public string SqlServerName { get; set; }
        public bool? SqlServerAuth { get; set; }
        public string SqlUserId { get; set; }
        public string SqlPassword { get; set; }
        public string DataBaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FDName { get; set; }
        public bool? IsDBConnected { get; set; }
        public string Address { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string EmailID { get; set; }
        public string pretailordQty { get; set; }
    }
    public class Global
    {

        //
        public static bool PRoundOff;
        public static int CEILINGROUNDOFF;
        DbConn Con = new DbConn();
        GBCDbConn GbcCon = new GBCDbConn();
        public List<Flag> listFlag = new List<Flag>();
        public bool CheckStarterService()
        {
            DataRow dr = getDataRow(false, "select appActiveDateTime from multiplefirm");
            if (dr != null)
            {
                DateTime activeTime = (dr["appActiveDateTime"] == null) ? DateTime.Now.AddMinutes(-30) : Convert.ToDateTime(dr["appActiveDateTime"]);
                int Result = DateTime.Compare(activeTime, DateTime.Now.AddMinutes(-30));
                if (Result < 0)
                {
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, string> RateTypeIdentification = new Dictionary<string, string>()
        {        
        {"", ""}, {"MRP", "RP"}, {"PURCPRICE", "PR"}, {"SALEPRICE", "SP"}, {"LOCALPRICE", "LR"}, {"OUTSTPRICE", "OS"}, {"TRADEPRICE", "TR"}, {"ADPRICE", "AD"}, {"COSTPRICE", "CP"}
        };

        public void DeleteTmpSI(string vrno, string slno, string flag)
        {
            string SQL = "";
            if (flag == "A")
            {
                SQL = "delete from tmpsi where vrno='" + vrno + "' and slno='" + slno + "'";
            }
            else
            {
                SQL = "delete from tmpsi where vrno='" + vrno + "'";
            }
            ExecuteQuery(true, SQL);
        }

        public string ChangeDate(string date)
        {
            date = date.Replace("-", "/");
            string[] str = date.Split('/');
            date = str[1] + "/" + str[0] + "/" + str[2];
            return date;
        }


        public void InsertLoginLog(string LogId, string Device)
        {
            LogId = LogId.ToUpper();
            string SQL = "INSERT INTO UserLoginLock ([Logid],[NodeName],[LogInTime],[LogActiveTime],[LogDate],[LastActiveTime]) VALUES ('" + LogId + "', '" + Device + "', convert(varchar(10),Getdate(),108) , convert(varchar(10),Getdate(),108) , GetDate(), GetDate())";
            ExecuteQuery(false, SQL);
        }
        public void InsertLogout(string LogId)
        {
            string SQL = "Update UserLoginLock set LogOutTime = GetDate() Where LogId='" + LogId + "' And LogOutTime is NULL And datediff(s, Isnull(LastActiveTime,''), GetDate()) > 1 ";
            ExecuteQuery(false, SQL);
        }
        public void DBConnInfo(string UserId)
        {

            GBCDbConn gbc_con = new GBCDbConn();
            DbConn con = new DbConn();
            TIA3T.NEW.BLL.GlobalVariableBLL objGblVrbBll = new TIA3T.NEW.BLL.GlobalVariableBLL();
            string constr = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
            string constrGBC = ConfigurationManager.ConnectionStrings["GBCDbConn"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            ArrayList arrDBInfoBll = new ArrayList();
            arrDBInfoBll.Add(con.Database.Connection.DataSource);
            arrDBInfoBll.Add(con.Database.Connection.Database);
            arrDBInfoBll.Add(System.DateTime.Now);
            arrDBInfoBll.Add(System.DateTime.Now.Year);
            arrDBInfoBll.Add(gbc_con.Database.Connection.Database + ".DBO.");
            arrDBInfoBll.Add(UserId);
            arrDBInfoBll.Add("");
            arrDBInfoBll.Add("");
            arrDBInfoBll.Add(constr);
            arrDBInfoBll.Add(IsDatabaseExit());
            arrDBInfoBll.Add(GBCDatabase());
            arrDBInfoBll.Add(GBCLastYear());
            TIA3T.BLL.GlobalVariablesBLL.SetVariables(arrDBInfoBll);
            objGblVrbBll.SetVariables(arrDBInfoBll);

        }
        public string PreGBC()
        {
            try
            {
                GBCDbConn gbc_con = new GBCDbConn();
                string PreYearGBC = gbc_con.Database.Connection.Database;
                string lastYearGBC = PreYearGBC.Substring(0, PreYearGBC.Length - 4);
                return lastYearGBC;
            }
            catch
            {
                return "";
            }
        }
        public int GBCLastYear()
        {
            try
            {
                DateTime Years = System.DateTime.Now;
                int PreYear = Years.Year - 1;
                return PreYear;
            }
            catch
            {
                return 0;
            }
        }
        public string GBCDatabase()
        {
            try
            {
                string lastYearGBC = PreGBC();
                int PreYear = GBCLastYear();
                string GetPreYearGbc = lastYearGBC + PreYear;
                return GetPreYearGbc;
            }
            catch
            {
                return "";
            }
        }
        public Boolean IsDatabaseExit()
        {
            try
            {
                string GetPreYearGbc = GBCDatabase();
                string constr = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
                SqlConnection sqlcon = new SqlConnection(constr);
                string sql = "select name from master.dbo.sysdatabases where name =N'" + GetPreYearGbc + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlcon);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public DataRow GetUserLoginsSql(int flag, string LogId)
        {
            try
            {
                string sel_column = "";
                string condition = "";
                string Sql;
                if (flag == 1)
                {
                    sel_column = " Distinct count(*) as Cnt ";
                    condition = " Convert(varchar(10),Isnull(LogDate,''),103) = Convert(varchar(10),GetDate(),103) And ";
                }
                else
                {
                    sel_column = " * ";
                    condition = " Isnull(Logid,'')='" + LogId + "' And  ";
                }

                Sql = "Select " + sel_column + " from UserLoginLock Where " + condition + " " +
                              "(datediff(ss, Isnull(LastActiveTime,''), GetDate()) < 40 or Isnull(LogOutTime,'') <>'') " +
                              "And Isnull(LogOutTime,'')='' ";
                return getDataRow(false, Sql);
            }
            catch
            {
                return null;
            }
        }

        public DataRow SelectDemo(string vrno)
        {
            string sql;
            sql = "select * from ENTGBC2016.DBO.SB where VrNo='" + vrno + "'";
            return getDataRow(false, sql);
        }
        public bool ExecuteQuery(bool IsGBCConn, string StrSql)
        {
            string Conn, GbcCon;
            Conn = System.Configuration.ConfigurationManager.ConnectionStrings["DbConn"].ToString();
            GbcCon = System.Configuration.ConfigurationManager.ConnectionStrings["GBCDbConn"].ToString();

            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(IsGBCConn ? GbcCon : Conn);
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

        public DataTable getDataTable(bool IsGBCConn, string StrSql)
        {
            string Conn, GbcCon, str;
            Conn = System.Configuration.ConfigurationManager.ConnectionStrings["DbConn"].ToString();
            GbcCon = System.Configuration.ConfigurationManager.ConnectionStrings["GBCDbConn"].ToString();
            str = (IsGBCConn ? GbcCon : Conn);
            SqlDataAdapter adp = new SqlDataAdapter(StrSql, str);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        public DataRow getDataRow(bool IsGBCConn, string StrSql)
        {
            DataTable dt = getDataTable(IsGBCConn, StrSql);
            if (dt.Rows.Count != 0 && dt != null)
                return dt.Rows[0];
            else
                return null;

        }

        public string GenrateVrNo(string cFdName, string cTbName, string VrSeries, DateTime VrDate, string FldVrNo = "")
        {
            SqlCommand cmdForNewVrnoN = new SqlCommand();
            SqlConnection con = new SqlConnection();
            string DMLForNewVrno = "";
            try
            {
                if (cFdName.Trim() == "" || VrSeries.Trim() == "")
                {
                    return "";
                }
                GBCDbConn GbcCon = new GBCDbConn();
                string name = GbcCon.Database.Connection.Database;
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DbConn"].ToString();
                con.Open();
                cmdForNewVrnoN.Connection = con;
                cmdForNewVrnoN.CommandTimeout = 600;
                cmdForNewVrnoN.CommandType = CommandType.StoredProcedure;
                cmdForNewVrnoN.CommandText = "Pr_NewVrNoN";
                cmdForNewVrnoN.Parameters.Add("@cFdName", SqlDbType.NVarChar, 5);
                cmdForNewVrnoN.Parameters.Add("@cTbName", SqlDbType.NVarChar, 50);
                cmdForNewVrnoN.Parameters.Add("@VrSeries", SqlDbType.NVarChar, 5);
                cmdForNewVrnoN.Parameters.Add("@VrLength", SqlDbType.Int, 5);
                cmdForNewVrnoN.Parameters.Add("@VrDate", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@FldVrNo", SqlDbType.NVarChar, 50);
                cmdForNewVrnoN.Parameters.Add("@DbForVc ", SqlDbType.NVarChar, 50);
                cmdForNewVrnoN.Parameters.Add("@AcYr", SqlDbType.Int);
                cmdForNewVrnoN.Parameters.Add("@VrNoNew2", SqlDbType.NVarChar, 20);
                cmdForNewVrnoN.Parameters["@VrNoNew2"].Direction = ParameterDirection.Output;
                cmdForNewVrnoN.Parameters["@cFdName"].Value = cFdName;
                cmdForNewVrnoN.Parameters["@cTbName"].Value = cTbName;
                cmdForNewVrnoN.Parameters["@VrSeries"].Value = VrSeries;
                cmdForNewVrnoN.Parameters["@VrLength"].Value = GetVrLength();
                cmdForNewVrnoN.Parameters["@VrDate"].Value = VrDate;
                cmdForNewVrnoN.Parameters["@FldVrNo"].Value = FldVrNo;
                cmdForNewVrnoN.Parameters["@DbForVc "].Value = name + ".dbo.VC";
                cmdForNewVrnoN.Parameters["@AcYr"].Value = ACYR();
                cmdForNewVrnoN.ExecuteNonQuery();
                DMLForNewVrno = cmdForNewVrnoN.Parameters["@VrNoNew2"].Value.ToString();
                DMLForNewVrno = DMLForNewVrno.Trim();

                cmdForNewVrnoN.Dispose();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            return DMLForNewVrno;
        }

        public void AdjustSlNo_TmpSI(string vrno)
        {
            string SQL = "update  Tmpsi set slno = t1.Newslno from ( Select VRNO, RIGHT( '000' + LTRIM(STR(ROW_NUMBER () OVER (PARTITION BY VRNO ORDER BY VRNO,slno ),3)),3) AS NewSLNO, slno from TMPSI Where VrNo='" + vrno + "'  ) as t1 where  Tmpsi.vrno = t1.vrno  and  Tmpsi.slno=t1.slno";
            ExecuteQuery(true, SQL);
        }

        public string SaveInvoiceBill(SaveInvoice obj_SaveInvoice, string isPrint)
        {
            SqlCommand cmdForNewVrnoN = new SqlCommand();
            SqlConnection con = new SqlConnection();
            string Vrno_ = "", result = "";
            try
            {
                AdjustSlNo_TmpSI(obj_SaveInvoice.TempVrNo);
                GBCDbConn GbcCon = new GBCDbConn();
                string name = GbcCon.Database.Connection.Database;
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GBCDbConn"].ToString();
                con.Open();
                cmdForNewVrnoN.Connection = con;
                cmdForNewVrnoN.CommandTimeout = 0;
                cmdForNewVrnoN.CommandType = CommandType.StoredProcedure;
                cmdForNewVrnoN.CommandText = "SP_Invoice_Save_Part1";
                cmdForNewVrnoN.Parameters.Add("@TempVrNo", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@VrSeries", SqlDbType.NChar, 2);
                cmdForNewVrnoN.Parameters.Add("@WthVrno", SqlDbType.VarChar, 11);
                cmdForNewVrnoN.Parameters.Add("@VRNO", SqlDbType.VarChar, 12);
                cmdForNewVrnoN.Parameters.Add("@VRDATE", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@PCODE", SqlDbType.VarChar, 9);
                cmdForNewVrnoN.Parameters.Add("@VRTYPE", SqlDbType.VarChar, 1);
                cmdForNewVrnoN.Parameters.Add("@TRCODE", SqlDbType.VarChar, 4);
                cmdForNewVrnoN.Parameters.Add("@CCODE", SqlDbType.VarChar, 3);
                cmdForNewVrnoN.Parameters.Add("@GROSS", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@FRGAMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADDPERC", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADDAMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADDDESC", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@LESSPERC", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@LESS", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@LESSDESC", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@ROUNDOFF", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@NETAMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@SKCODE", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@USERNO", SqlDbType.VarChar, 30);
                cmdForNewVrnoN.Parameters.Add("@USRID", SqlDbType.VarChar, 15);
                cmdForNewVrnoN.Parameters.Add("@VrTime", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@ISEST", SqlDbType.Bit);
                cmdForNewVrnoN.Parameters.Add("@SBVRNO", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@NOP", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@DMNO", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@DMDATE", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@ADMDATE", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@SRCODE", SqlDbType.VarChar, 6);
                cmdForNewVrnoN.Parameters.Add("@ORDERNO", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@ORDERDATE", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@CHEQUENO", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@CHEQUEDATE", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@LRNO", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@LRDATE", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@CARRIER", SqlDbType.VarChar, 100);
                cmdForNewVrnoN.Parameters.Add("@CARRTYPE", SqlDbType.VarChar, 1);
                cmdForNewVrnoN.Parameters.Add("@DISCOUNT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@MST", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@PTAXVAT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@STAXVAT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@OCTPERC", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@OCTROI", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@SDISCOUNT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@EXCISE", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@SPDISCOUNT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@RETVRNO", SqlDbType.VarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@GDNCODE", SqlDbType.VarChar, 6);
                cmdForNewVrnoN.Parameters.Add("@NameP", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@Addr", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@DCode", SqlDbType.VarChar, 8);
                cmdForNewVrnoN.Parameters.Add("@DrName", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@DrAddr", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@TCS", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@UserId", SqlDbType.Char, 6);
                cmdForNewVrnoN.Parameters.Add("@SBNARR1", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@SBNARR2", SqlDbType.VarChar, 250);
                cmdForNewVrnoN.Parameters.Add("@BAL_AMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@PAID", SqlDbType.Char, 1);
                cmdForNewVrnoN.Parameters.Add("@REFNO", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@PatientID", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@RoundoffYN", SqlDbType.Char, 1);
                cmdForNewVrnoN.Parameters.Add("@CashrUser", SqlDbType.VarChar, 6);
                cmdForNewVrnoN.Parameters.Add("@CashrNode", SqlDbType.VarChar, 50);
                cmdForNewVrnoN.Parameters.Add("@CashrDt", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@PresImg", SqlDbType.VarChar, 2000);
                cmdForNewVrnoN.Parameters.Add("@PtPhone", SqlDbType.VarChar, 200);
                cmdForNewVrnoN.Parameters.Add("@CarAmt", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@DiscPer", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@MstAmt", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@DrAmt", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@NodeId", SqlDbType.NVarChar, 200);
                cmdForNewVrnoN.Parameters.Add("@TaxType", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@Cst", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Weight", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@StdCase", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@MixCase", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@CFormNo", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@CFormDate", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@SbcCode", SqlDbType.VarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@VehicleNo", SqlDbType.NVarChar, 25);
                cmdForNewVrnoN.Parameters.Add("@PostInCode", SqlDbType.NVarChar, 25);
                cmdForNewVrnoN.Parameters.Add("@TrfVrno", SqlDbType.NVarChar, 15);
                cmdForNewVrnoN.Parameters.Add("@CARDNO", SqlDbType.NVarChar, 20);
                cmdForNewVrnoN.Parameters.Add("@CARDHOLDERNAME", SqlDbType.NVarChar, 500);
                cmdForNewVrnoN.Parameters.Add("@BANKERNAME", SqlDbType.NVarChar, 200);
                cmdForNewVrnoN.Parameters.Add("@CARDEXPIRY", SqlDbType.DateTime);
                cmdForNewVrnoN.Parameters.Add("@CVVNO", SqlDbType.NVarChar, 5);
                cmdForNewVrnoN.Parameters.Add("@RecdAmt", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@RtnAmt", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@VATAPPLIED", SqlDbType.NVarChar, 91);
                cmdForNewVrnoN.Parameters.Add("@BILLNO", SqlDbType.NVarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@BILLDATE", SqlDbType.SmallDateTime);
                cmdForNewVrnoN.Parameters.Add("@DEDUCT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADD", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@CRAMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADJVRNO", SqlDbType.NVarChar, 10);
                cmdForNewVrnoN.Parameters.Add("@ADJAMT", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ADJDAYS", SqlDbType.Int);
                cmdForNewVrnoN.Parameters.Add("@GROSSMRP", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@ENTTIME", SqlDbType.NVarChar, 8);
                cmdForNewVrnoN.Parameters.Add("@BRNARR", SqlDbType.NVarChar, 96);
                cmdForNewVrnoN.Parameters.Add("@BRNARR2", SqlDbType.NVarChar, 96);
                cmdForNewVrnoN.Parameters.Add("@YN", SqlDbType.NVarChar, 1);
                cmdForNewVrnoN.Parameters.Add("@ADJUSTIN", SqlDbType.NVarChar, 1);
                cmdForNewVrnoN.Parameters.Add("@DEDMRP", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@VATTYPE", SqlDbType.NVarChar, 25);
                cmdForNewVrnoN.Parameters.Add("@OneThosand", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@FiveH", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@OneH", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Fifty", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Twenty", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Ten", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Coins", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@Paise", SqlDbType.Float);
                cmdForNewVrnoN.Parameters.Add("@GBCName", SqlDbType.VarChar, 50);
                cmdForNewVrnoN.Parameters.Add("@AcYr", SqlDbType.VarChar, 4);
                cmdForNewVrnoN.Parameters.Add("@VrNoNew2", SqlDbType.NVarChar, 15);
                cmdForNewVrnoN.Parameters.Add("@Result", SqlDbType.VarChar, 50);

                cmdForNewVrnoN.Parameters["@VrNoNew2"].Direction = ParameterDirection.Output;
                cmdForNewVrnoN.Parameters["@Result"].Direction = ParameterDirection.Output;

                cmdForNewVrnoN.Parameters["@TempVrNo"].Value = obj_SaveInvoice.TempVrNo;
                cmdForNewVrnoN.Parameters["@VrSeries"].Value = obj_SaveInvoice.VrSeries;
                cmdForNewVrnoN.Parameters["@WthVrno"].Value = GetVrLength();
                cmdForNewVrnoN.Parameters["@VRNO"].Value = obj_SaveInvoice.VRNO;
                cmdForNewVrnoN.Parameters["@VRDATE"].Value = obj_SaveInvoice.VRDATE;
                cmdForNewVrnoN.Parameters["@PCODE"].Value = obj_SaveInvoice.PCODE;
                cmdForNewVrnoN.Parameters["@VRTYPE"].Value = obj_SaveInvoice.VRTYPE;
                cmdForNewVrnoN.Parameters["@TRCODE"].Value = obj_SaveInvoice.TRCODE;
                cmdForNewVrnoN.Parameters["@CCODE"].Value = obj_SaveInvoice.CCODE;
                cmdForNewVrnoN.Parameters["@GROSS"].Value = obj_SaveInvoice.GROSS;
                cmdForNewVrnoN.Parameters["@FRGAMT"].Value = obj_SaveInvoice.FRGAMT;
                cmdForNewVrnoN.Parameters["@ADDPERC"].Value = obj_SaveInvoice.ADDPERC;
                cmdForNewVrnoN.Parameters["@ADDAMT"].Value = obj_SaveInvoice.ADDAMT;
                cmdForNewVrnoN.Parameters["@ADDDESC"].Value = obj_SaveInvoice.ADDDESC;
                cmdForNewVrnoN.Parameters["@LESSPERC"].Value = obj_SaveInvoice.LESSPERC;
                cmdForNewVrnoN.Parameters["@LESS"].Value = obj_SaveInvoice.LESS;
                cmdForNewVrnoN.Parameters["@LESSDESC"].Value = obj_SaveInvoice.LESSDESC;
                cmdForNewVrnoN.Parameters["@ROUNDOFF"].Value = obj_SaveInvoice.ROUNDOFF;
                cmdForNewVrnoN.Parameters["@NETAMT"].Value = obj_SaveInvoice.NETAMT;
                cmdForNewVrnoN.Parameters["@SKCODE"].Value = obj_SaveInvoice.SKCODE;
                cmdForNewVrnoN.Parameters["@USERNO"].Value = obj_SaveInvoice.USERNO;
                cmdForNewVrnoN.Parameters["@USRID"].Value = obj_SaveInvoice.USRID;
                cmdForNewVrnoN.Parameters["@VrTime"].Value = obj_SaveInvoice.VrTime;
                cmdForNewVrnoN.Parameters["@ISEST"].Value = obj_SaveInvoice.ISEST;
                cmdForNewVrnoN.Parameters["@SBVRNO"].Value = obj_SaveInvoice.SBVRNO;
                cmdForNewVrnoN.Parameters["@NOP"].Value = obj_SaveInvoice.NOP;
                cmdForNewVrnoN.Parameters["@DMNO"].Value = obj_SaveInvoice.DMNO;
                cmdForNewVrnoN.Parameters["@DMDATE"].Value = obj_SaveInvoice.DMDATE;
                cmdForNewVrnoN.Parameters["@ADMDATE"].Value = obj_SaveInvoice.ADMDATE;
                cmdForNewVrnoN.Parameters["@SRCODE"].Value = obj_SaveInvoice.SRCODE;
                cmdForNewVrnoN.Parameters["@ORDERNO"].Value = obj_SaveInvoice.ORDERNO;
                cmdForNewVrnoN.Parameters["@ORDERDATE"].Value = obj_SaveInvoice.ORDERDATE;
                cmdForNewVrnoN.Parameters["@CHEQUENO"].Value = obj_SaveInvoice.CHEQUENO;
                cmdForNewVrnoN.Parameters["@CHEQUEDATE"].Value = obj_SaveInvoice.CHEQUEDATE;
                cmdForNewVrnoN.Parameters["@LRNO"].Value = obj_SaveInvoice.LRNO;
                cmdForNewVrnoN.Parameters["@LRDATE"].Value = obj_SaveInvoice.LRDATE;
                cmdForNewVrnoN.Parameters["@CARRIER"].Value = obj_SaveInvoice.CARRIER;
                cmdForNewVrnoN.Parameters["@CARRTYPE"].Value = obj_SaveInvoice.CARRTYPE;
                cmdForNewVrnoN.Parameters["@DISCOUNT"].Value = obj_SaveInvoice.DISCOUNT;
                cmdForNewVrnoN.Parameters["@MST"].Value = obj_SaveInvoice.MST;
                cmdForNewVrnoN.Parameters["@PTAXVAT"].Value = obj_SaveInvoice.PTAXVAT;
                cmdForNewVrnoN.Parameters["@STAXVAT"].Value = obj_SaveInvoice.STAXVAT;
                cmdForNewVrnoN.Parameters["@OCTPERC"].Value = obj_SaveInvoice.OCTPERC;
                cmdForNewVrnoN.Parameters["@OCTROI"].Value = obj_SaveInvoice.OCTROI;
                cmdForNewVrnoN.Parameters["@SDISCOUNT"].Value = obj_SaveInvoice.SDISCOUNT;
                cmdForNewVrnoN.Parameters["@EXCISE"].Value = obj_SaveInvoice.EXCISE;
                cmdForNewVrnoN.Parameters["@SPDISCOUNT"].Value = obj_SaveInvoice.SPDISCOUNT;
                cmdForNewVrnoN.Parameters["@RETVRNO"].Value = obj_SaveInvoice.RETVRNO;
                cmdForNewVrnoN.Parameters["@GDNCODE"].Value = obj_SaveInvoice.GDNCODE;
                cmdForNewVrnoN.Parameters["@NameP"].Value = obj_SaveInvoice.NameP;
                cmdForNewVrnoN.Parameters["@Addr"].Value = obj_SaveInvoice.Addr;
                cmdForNewVrnoN.Parameters["@DCode"].Value = obj_SaveInvoice.DCode;
                cmdForNewVrnoN.Parameters["@DrName"].Value = obj_SaveInvoice.DrName;
                cmdForNewVrnoN.Parameters["@DrAddr"].Value = obj_SaveInvoice.DrAddr;
                cmdForNewVrnoN.Parameters["@TCS"].Value = obj_SaveInvoice.TCS;
                cmdForNewVrnoN.Parameters["@UserId"].Value = obj_SaveInvoice.UserId;
                cmdForNewVrnoN.Parameters["@SBNARR1"].Value = obj_SaveInvoice.SBNARR1;
                cmdForNewVrnoN.Parameters["@SBNARR2"].Value = obj_SaveInvoice.SBNARR2;
                cmdForNewVrnoN.Parameters["@BAL_AMT"].Value = obj_SaveInvoice.BAL_AMT;
                cmdForNewVrnoN.Parameters["@PAID"].Value = obj_SaveInvoice.PAID;
                cmdForNewVrnoN.Parameters["@REFNO"].Value = obj_SaveInvoice.REFNO;
                cmdForNewVrnoN.Parameters["@PatientID"].Value = obj_SaveInvoice.PatientID;
                cmdForNewVrnoN.Parameters["@RoundoffYN"].Value = obj_SaveInvoice.RoundoffYN;
                cmdForNewVrnoN.Parameters["@CashrUser"].Value = obj_SaveInvoice.CashrUser;
                cmdForNewVrnoN.Parameters["@CashrNode"].Value = obj_SaveInvoice.CashrNode;
                cmdForNewVrnoN.Parameters["@CashrDt"].Value = obj_SaveInvoice.CashrDt;
                cmdForNewVrnoN.Parameters["@PresImg"].Value = obj_SaveInvoice.PresImg;
                cmdForNewVrnoN.Parameters["@PtPhone"].Value = obj_SaveInvoice.PtPhone;
                cmdForNewVrnoN.Parameters["@CarAmt"].Value = obj_SaveInvoice.CarAmt;
                cmdForNewVrnoN.Parameters["@DiscPer"].Value = obj_SaveInvoice.DiscPer;
                cmdForNewVrnoN.Parameters["@MstAmt"].Value = obj_SaveInvoice.MstAmt;
                cmdForNewVrnoN.Parameters["@DrAmt"].Value = obj_SaveInvoice.DrAmt;
                cmdForNewVrnoN.Parameters["@NodeId"].Value = obj_SaveInvoice.NodeId;
                cmdForNewVrnoN.Parameters["@TaxType"].Value = obj_SaveInvoice.TaxType;
                cmdForNewVrnoN.Parameters["@Cst"].Value = obj_SaveInvoice.Cst;
                cmdForNewVrnoN.Parameters["@Weight"].Value = obj_SaveInvoice.Weight;
                cmdForNewVrnoN.Parameters["@StdCase"].Value = obj_SaveInvoice.StdCase;
                cmdForNewVrnoN.Parameters["@MixCase"].Value = obj_SaveInvoice.MixCase;
                cmdForNewVrnoN.Parameters["@CFormNo"].Value = obj_SaveInvoice.CFormNo;
                cmdForNewVrnoN.Parameters["@CFormDate"].Value = obj_SaveInvoice.CFormDate;
                cmdForNewVrnoN.Parameters["@SbcCode"].Value = obj_SaveInvoice.SbcCode;
                cmdForNewVrnoN.Parameters["@VehicleNo"].Value = obj_SaveInvoice.VehicleNo;
                cmdForNewVrnoN.Parameters["@PostInCode"].Value = obj_SaveInvoice.PostInCode;
                cmdForNewVrnoN.Parameters["@TrfVrno"].Value = obj_SaveInvoice.TrfVrno;
                cmdForNewVrnoN.Parameters["@CARDNO"].Value = obj_SaveInvoice.CARDNO;
                cmdForNewVrnoN.Parameters["@CARDHOLDERNAME"].Value = obj_SaveInvoice.CARDHOLDERNAME;
                cmdForNewVrnoN.Parameters["@BANKERNAME"].Value = obj_SaveInvoice.BANKERNAME;
                cmdForNewVrnoN.Parameters["@CARDEXPIRY"].Value = obj_SaveInvoice.CARDEXPIRY;
                cmdForNewVrnoN.Parameters["@CVVNO"].Value = obj_SaveInvoice.CVVNO;
                cmdForNewVrnoN.Parameters["@RecdAmt"].Value = obj_SaveInvoice.RecdAmt;
                cmdForNewVrnoN.Parameters["@RtnAmt"].Value = obj_SaveInvoice.RtnAmt;
                cmdForNewVrnoN.Parameters["@VATAPPLIED"].Value = obj_SaveInvoice.VATAPPLIED;
                cmdForNewVrnoN.Parameters["@BILLNO"].Value = obj_SaveInvoice.BILLNO;
                cmdForNewVrnoN.Parameters["@BILLDATE"].Value = obj_SaveInvoice.BILLDATE;
                cmdForNewVrnoN.Parameters["@DEDUCT"].Value = obj_SaveInvoice.DEDUCT;
                cmdForNewVrnoN.Parameters["@ADD"].Value = obj_SaveInvoice.ADD;
                cmdForNewVrnoN.Parameters["@CRAMT"].Value = obj_SaveInvoice.CRAMT;
                cmdForNewVrnoN.Parameters["@ADJVRNO"].Value = obj_SaveInvoice.ADJVRNO;
                cmdForNewVrnoN.Parameters["@ADJAMT"].Value = obj_SaveInvoice.ADJAMT;
                cmdForNewVrnoN.Parameters["@ADJDAYS"].Value = obj_SaveInvoice.ADJDAYS;
                cmdForNewVrnoN.Parameters["@GROSSMRP"].Value = obj_SaveInvoice.GROSSMRP;
                cmdForNewVrnoN.Parameters["@ENTTIME"].Value = obj_SaveInvoice.ENTTIME;
                cmdForNewVrnoN.Parameters["@BRNARR"].Value = obj_SaveInvoice.BRNARR;
                cmdForNewVrnoN.Parameters["@BRNARR2"].Value = obj_SaveInvoice.BRNARR2;
                cmdForNewVrnoN.Parameters["@YN"].Value = obj_SaveInvoice.YN;
                cmdForNewVrnoN.Parameters["@ADJUSTIN"].Value = obj_SaveInvoice.ADJUSTIN;
                cmdForNewVrnoN.Parameters["@DEDMRP"].Value = obj_SaveInvoice.DEDMRP;
                cmdForNewVrnoN.Parameters["@VATTYPE"].Value = obj_SaveInvoice.VATTYPE;
                cmdForNewVrnoN.Parameters["@OneThosand"].Value = obj_SaveInvoice.OneThosand;
                cmdForNewVrnoN.Parameters["@FiveH"].Value = obj_SaveInvoice.FiveH;
                cmdForNewVrnoN.Parameters["@OneH"].Value = obj_SaveInvoice.OneH;
                cmdForNewVrnoN.Parameters["@Fifty"].Value = obj_SaveInvoice.Fifty;
                cmdForNewVrnoN.Parameters["@Twenty"].Value = obj_SaveInvoice.Twenty;
                cmdForNewVrnoN.Parameters["@Ten"].Value = obj_SaveInvoice.Ten;
                cmdForNewVrnoN.Parameters["@Coins"].Value = obj_SaveInvoice.Coins;
                cmdForNewVrnoN.Parameters["@Paise"].Value = obj_SaveInvoice.Paise;
                cmdForNewVrnoN.Parameters["@GBCName"].Value = name + ".dbo.";
                cmdForNewVrnoN.Parameters["@AcYr"].Value = ACYR();
                cmdForNewVrnoN.ExecuteNonQuery();
                Vrno_ = cmdForNewVrnoN.Parameters["@VrNoNew2"].Value.ToString();
                result = cmdForNewVrnoN.Parameters["@Result"].Value.ToString();
                Vrno_ = Vrno_.Trim();
                cmdForNewVrnoN.Dispose();
                if (isPrint == "1")
                {
                    try
                    {
                        Add_PrintQ(obj_SaveInvoice.TRCODE, obj_SaveInvoice.UserId, Vrno_);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Vrno_ = "$" + ex.Message + " " + ex.InnerException.Message;
            }
            finally
            {
                con.Close();
            }
            return Vrno_;
        }



        public string GetGBCDbName(SqlConnection conn)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter("select databaseName from multiplefirm", conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    DataRow dr = dt.Rows[0];
                    return dr["databaseName"].ToString();
                }
            }
            catch
            {
                return "";
            }
            return "";
        }

        public int ACYR()
        {
            DateTime curr_date = DateTime.Now.Date;
            int YR;
            try
            {
                YR = (curr_date.Month > 3) ? curr_date.Year : curr_date.Year - 1;
            }
            catch
            {
                YR = DateTime.Now.Date.Year;
            }
            return YR;
        }

        public int GetVrLength()
        {
            int length = 0;
            DbConn con = new DbConn();
            string flag = con.Db_Multi.First(p => p.DatabaseName != "").SaleFlag;
            string[] Flag_arry = flag.Split('|');
            for (int i = 0; i <= Flag_arry.Length; i++)
            {
                string[] FlagVal = Flag_arry[i].Split(',');
                if (FlagVal[0] == "wthVrNo")
                {
                    length = Convert.ToInt16(FlagVal[1]);
                    break;
                }
            }
            return length;
        }
        //public int GetretailOrdQty()
        //{
        //    int result = 0;
        //    DbConn con = new DbConn();
        //    string flag = con.Db_Multi.SingleOrDefault(p => p.DatabaseName != "").SaleFlag;
        //    string[] Flag_arry = flag.Split('|');
        //    for (int i = 0; i <= Flag_arry.Length; i++)
        //    {
        //        string[] FlagVal = Flag_arry[i].Split(',');
        //        if (FlagVal[0].ToLower() == "pretailordqty")
        //        {
        //            result = Convert.ToInt16(FlagVal[1]);
        //            break;
        //        }
        //    }
        //    return result;
        //}

        public Order SaveOrder(Order Ord)
        {
            try
            {
                double amt = 0;
                GBCDbConn gbc_con = new GBCDbConn();
                Global objGlobal = new Global();
                string name = GbcCon.Database.Connection.Database;
                //Get VrNo
                string vrno = GenrateVrNo(Ord.FDName, name + ".dbo.OB", (string.IsNullOrEmpty(Ord.series) ? "OS" : Ord.series), DateTime.Now.Date, "");

                //delete from OI
                Delete_OI(vrno);

                //delete from CO
                delete_CO(vrno);

                //delete from PO
                delete_PO(vrno);

                //insert into OI
                amt = Insert_OI(vrno, Ord.items, Ord.pcode, Ord.Iwqty, Ord.AppType);

                //insert into CO
                Insert_CO(vrno, Ord.items, Ord.pcode);

                //insert into PO
                Insert_PO(vrno, Ord.items, Ord.pcode);

                //Update OB                
                 amt=insert_OB(vrno, Ord.pcode, amt, "", Ord.PatientID, Ord.NameP, Ord.Addr, Ord.DCode, Ord.DrName, Ord.DrAddr, Ord.FDName, Convert.ToDouble(Ord.DelCharges), Ord.AppType, Ord.EWD, Ord.Disc, Ord.pWallet);

                amt = amt + Convert.ToDouble(Ord.DelCharges);

                GbcCon.SaveChanges();
                Con.SaveChanges();
                Ord.vrno = vrno;
                Ord.vrdate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                Ord.TotalAmt = amt.ToString();
                Ord.pcode = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == Ord.pcode).PNAME;
                //int iswqty = GetretailOrdQty();
                if (Ord.Iwqty == "1" && (Ord.AppType == "$" || Ord.AppType == "@"))
                {
                    foreach (var item in Ord.items)
                    {
                        string pursize;
                        DataRow dr = getDataRow(true, "select pursize from im where icode='" + item.ICODE + "'");
                        if (dr != null)
                        {
                            pursize = (dr["pursize"] == null ? "1" : dr["pursize"].ToString());
                        }
                        else
                        {
                            pursize = "1";
                        }
                        item.Rate = (Convert.ToDouble(item.Rate) * Convert.ToDouble(pursize)).ToString();
                    }
                }
                return Ord;
            }
            catch (Exception ex)
            {
                Order ord1 = new Order();
                ord1.pcode = ex.Message + " Inner Exception Message " + ex.InnerException.InnerException.ToString().Split('\n')[0].ToString();
                return ord1;
            }
        }
        public Order1 SaveOrder1(Order1 Ord)
        {
            try
            {
                double amt = 0;
                GBCDbConn gbc_con = new GBCDbConn();
                Global objGlobal = new Global();
                string name = GbcCon.Database.Connection.Database;
                //Get VrNo
                string vrno = GenrateVrNo(Ord.FDName, name + ".dbo.OB", (string.IsNullOrEmpty(Ord.series) ? "OS" : Ord.series), DateTime.Now.Date, "");

                //delete from OI
                Delete_OI(vrno);

                //delete from CO
                delete_CO(vrno);

                //delete from PO
                delete_PO(vrno);

                //insert into OI
                amt = 0.0;//  Insert_OI(vrno, Ord.items, Ord.pcode);

                //insert into CO
                // Insert_CO(vrno, Ord.items, Ord.pcode);

                //insert into PO
                //  Insert_PO(vrno, Ord.items, Ord.pcode);

                //Update OB                
                insert_OB(vrno, Ord.pcode, amt, "", Ord.PatientID, Ord.NameP, Ord.Addr, Ord.DCode, Ord.DrName, Ord.DrAddr, Ord.FDName, Convert.ToDouble(Ord.DelCharges), Ord.AppType, Ord.EWD);
                amt = amt + Convert.ToDouble(Ord.DelCharges);

                GbcCon.SaveChanges();
                Con.SaveChanges();
                Ord.vrno = vrno;
                Ord.vrdate = DateTime.Now.Date.ToString("dd/MM/yyyy");
                Ord.TotalAmt = amt.ToString();
                Ord.pcode = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == Ord.pcode).PNAME;

                return Ord;
            }
            catch (Exception ex)
            {
                Order1 ord1 = new Order1();
                ord1.pcode = ex.Message + " Inner Exception Message " + ex.InnerException.InnerException.ToString().Split('\n')[0].ToString();
                return ord1;
            }
        }
        public void Add_PrintQ(string trcode, string userId, string vrno)
        {
            PRINTQ obj_print = new PRINTQ();
            obj_print.InsDateTime = DateTime.Now.Date;
            obj_print.IP = GetInetIP();
            obj_print.NodeId = "";
            obj_print.TrCode = trcode;
            obj_print.UserId = userId;
            obj_print.VrCode = vrno;
            GbcCon.Db_printQ.Add(obj_print);
            GbcCon.SaveChanges();
        }

        public string GetInetIP()
        {
            try
            {
                WebClient client = new WebClient();
                return client.DownloadString("http://icanhazip.com/");
            }
            catch
            {
                return "0.0.0.0";
            }
        }
        public void delete_PO(string vrno)
        {
            PO obj_po_ = Con.Db_PO.SingleOrDefault(p => p.VRNO == vrno);
            if (obj_po_ != null)
            {
                Con.Db_PO.Remove(obj_po_);
            }
        }

        public void Insert_PO(string vrno, List<ItemMaster> im, string pcode)
        {
            int i = 1;
            foreach (var item in im)
            {
                string slno = "00" + i;
                PO obj_po = new PO();
                obj_po.VRNO = vrno;
                obj_po.VRDATE = DateTime.Now.Date;
                obj_po.SLNO = slno.Substring(slno.Length - 3, 3);
                obj_po.ICODE = item.ICODE;
                obj_po.ORDERQTY = Convert.ToDouble(item.Qty);
                obj_po.FREEQTY = Convert.ToDouble(item.free);
                obj_po.CANCQTY = 0;
                obj_po.ORDMRP = Convert.ToDouble(item.Mrp);
                obj_po.ORDRATE = Convert.ToDouble(item.Rate);
                obj_po.ORAMT = Convert.ToDouble(item.Rate) * Convert.ToDouble(item.Qty);
                obj_po.ORDVALUE = Convert.ToDouble(item.Rate) * Convert.ToDouble(item.Qty);
                obj_po.PCODE = pcode;
                obj_po.ORDYN = "";
                Con.Db_PO.Add(obj_po);
                i++;
            }
        }
        public void delete_CO(string vrno)
        {
            CO obj_co_ = GbcCon.Db_CO.SingleOrDefault(p => p.VRNO == vrno);
            if (obj_co_ != null)
            {
                GbcCon.Db_CO.Remove(obj_co_);
            }
        }


        public void Insert_CO(string vrno, List<ItemMaster> im, string pcode)
        {
            int i = 1;
            foreach (var item in im)
            {
                string slno = "00" + i;
                CO obj_oi = new CO();
                obj_oi.VRNO = vrno;
                obj_oi.VRDATE = DateTime.Now.Date;
                obj_oi.SLNO = slno.Substring(slno.Length - 3, 3);
                obj_oi.ICODE = item.ICODE;
                obj_oi.ORDERQTY = Convert.ToDouble(item.Qty);
                obj_oi.FREEQTY = Convert.ToDouble(item.free);
                obj_oi.CANCQTY = 0;
                obj_oi.ORDMRP = Convert.ToDouble(item.Mrp);
                obj_oi.ORDRATE = Convert.ToDouble(item.Rate);
                obj_oi.ORAMT = Convert.ToDouble(item.Rate) * Convert.ToDouble(item.Qty);
                obj_oi.ORDVALUE = Convert.ToDouble(item.Rate) * Convert.ToDouble(item.Qty);
                obj_oi.PCODE = pcode;
                obj_oi.ORDYN = "";
                obj_oi.Remark = "";
                obj_oi.SCHMPER = 0;
                GbcCon.Db_CO.Add(obj_oi);
                i++;
            }
        }
        public void Delete_OI(string vrno)
        {
            OI obj_oi_ = GbcCon.Db_OI.SingleOrDefault(p => p.VRNO == vrno);
            if (obj_oi_ != null)
            {
                GbcCon.Db_OI.Remove(obj_oi_);
            }
        }

        public double Insert_OI(string vrno, List<ItemMaster> im, string pcode, string iswqty, string Apptype)
        {
            double value = 0;
            int i = 1;
            foreach (var item in im)
            {
                string pursize;
                DataRow dr = getDataRow(true, "select pursize from im where icode='" + item.ICODE + "'");
                if (dr != null)
                {
                    pursize = (dr["pursize"] == null ? "1" : dr["pursize"].ToString());
                }
                else
                {
                    pursize = "1";
                }
                string slno = "00" + i;
                OI obj_oi = new OI();
                obj_oi.VRNO = vrno;
                obj_oi.VRDATE = DateTime.Now.Date;
                obj_oi.SLNO = slno.Substring(slno.Length - 3, 3);
                obj_oi.ICODE = item.ICODE;
                if (iswqty == "1" && (Apptype == "$" || Apptype == "@"))
                {
                    obj_oi.ORDERWQTY = Convert.ToDouble(item.Qty);
                    obj_oi.FREEWQTY = Convert.ToDouble(item.free);
                    obj_oi.ORDERQTY = Convert.ToDouble(item.Qty) * Convert.ToDouble(pursize);
                    obj_oi.FREEQTY = Convert.ToDouble(item.free) * Convert.ToDouble(pursize);
                }
                else
                {
                    obj_oi.ORDERQTY = Convert.ToDouble(item.Qty);
                    obj_oi.FREEQTY = Convert.ToDouble(item.free);
                }
                //obj_oi.ORDERQTY = Convert.ToDouble(item.Qty);
                //obj_oi.FREEQTY = Convert.ToDouble(item.free);
                obj_oi.CANCQTY = 0;
                obj_oi.ORDMRP = Convert.ToDouble(item.Mrp);
                obj_oi.ORDRATE = Convert.ToDouble(item.Rate);
                obj_oi.ORAMT = Convert.ToDouble(item.Rate) * Convert.ToDouble(obj_oi.ORDERQTY);
                obj_oi.ORDVAL = Convert.ToDouble(item.Rate) * Convert.ToDouble(obj_oi.ORDERQTY);
                obj_oi.ORDBATCH = "";
                obj_oi.PCODE = pcode;
                obj_oi.ORDYN = "";
                obj_oi.Remark = "";
                obj_oi.SCHMPER = 0;
                obj_oi.CSMAPICODE = "";
                GbcCon.Db_OI.Add(obj_oi);
                value = value + Convert.ToDouble(item.Rate) * Convert.ToDouble(obj_oi.ORDERQTY);
                i++;
            }
            return value;
        }
        public double insert_OB(string vrno, string pcode, double amt, string FilePath, string PatientID, string NameP, string Addr, string DCode, string DrName, string DrAddr, string Fdanme = "ORDS", double charge = 0, string Apptype = "", string EWD = "0", string Disc = "0", string pWallet = "0")
        {
            double Amt = 0;
            OB obj_Ob = GbcCon.Db_ob.SingleOrDefault(p => p.VRNO == vrno);
            if (obj_Ob != null)
            {
                obj_Ob.VRDATE = DateTime.Now;
                obj_Ob.PCODE = pcode;
                obj_Ob.TRCode = Fdanme;
                obj_Ob.PresImg = FilePath;
                obj_Ob.PatientID = PatientID;
                obj_Ob.NameP = NameP;
                obj_Ob.Addr = Addr;
                obj_Ob.DCode = DCode;
                obj_Ob.DrName = DrName;
                obj_Ob.DrAddr = DrAddr;
                obj_Ob.OrderFrom = Apptype;
                obj_Ob.Status = "OPL";
                obj_Ob.ORDADDAMT = charge;
                obj_Ob.ORDADDDESC = "Delivery Charge.";
                obj_Ob.ORDLESS = Convert.ToDouble(Disc);
                obj_Ob.ORDGROSS = amt;
                obj_Ob.ORAMT = ((amt + charge) - Convert.ToDouble(EWD)) - Convert.ToDouble(Disc);
                Amt = Convert.ToDouble(obj_Ob.ORAMT);
                obj_Ob.EWD = Convert.ToDouble(EWD);
            }
            else
            {
                OB obj_Ob1 = new OB();
                obj_Ob1.VRDATE = DateTime.Now;
                obj_Ob1.PCODE = pcode;
                obj_Ob1.TRCode = Fdanme;
                obj_Ob1.VRNO = vrno;
                obj_Ob1.PresImg = FilePath;
                obj_Ob.PatientID = PatientID;
                obj_Ob.NameP = NameP;
                obj_Ob.Addr = Addr;
                obj_Ob.DCode = DCode;
                obj_Ob.DrName = DrName;
                obj_Ob.DrAddr = DrAddr;
                obj_Ob.OrderFrom = Apptype;
                obj_Ob.Status = "OPL";
                obj_Ob.ORDADDAMT = charge;
                obj_Ob.ORDADDDESC = "Delivery Charge.";
                obj_Ob.EWD = Convert.ToDouble(EWD);
                obj_Ob.ORDLESS = Convert.ToDouble(Disc);
                obj_Ob1.ORDGROSS = amt;
                obj_Ob1.ORAMT = ((amt + charge) - Convert.ToDouble(EWD)) - Convert.ToDouble(Disc);
                Amt = Convert.ToDouble(obj_Ob1.ORAMT);
                GbcCon.Db_ob.Add(obj_Ob1);
            }
            if (Apptype == "$" && pWallet == "1")
            {
                OrderWallet(vrno, PatientID, "S", "Estimate Wallete Amount  is " + EWD + " on Order Amount " + amt, EWD);
                // SendNotification(PatientID, vrno);
            }
            return Amt;
        }

        public void SendNotification(string Ptcode, string OrderId)
        {
            clsSendMobNotification obj = new clsSendMobNotification();
            DataRow dr = getDataRow(false, "select deviceid from patientmaster where ptcode='" + Ptcode + "'");
            string deviceId = "";
            if (dr != null)
            {
                deviceId = (dr["deviceid"] == null) ? "" : dr["deviceid"].ToString();
            }
            if (deviceId != "")
            {
                obj.Notification("Thank You. Your Order Received! Your Order Id is " + OrderId + ". We will Notify You For Further Order Details.", deviceId, "Shree Medical");
            }
        }

        public DateTime ACYR_START_DATE(DateTime date)
        {

            int YR = DateTime.Now.Date.Year;
            try
            {
                DateTime AccDate;
                try
                {
                    AccDate = date;
                }
                catch
                {
                    AccDate = DateTime.Now.Date;
                }
                YR = (AccDate.Month > 3) ? AccDate.Year : AccDate.Year - 1;
            }
            catch
            {

            }
            if (CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.StartsWith("d"))
            {
                return Convert.ToDateTime("01/04/" + YR);
            }
            else
            {
                return Convert.ToDateTime("04/01/" + YR);
            }
        }
        public DateTime ACYR_END_DATE(DateTime date)
        {
            int YR = DateTime.Now.Date.Year;
            try
            {
                DateTime AccDate;
                try
                {
                    AccDate = date;
                }
                catch
                {
                    AccDate = DateTime.Now.Date;
                }
                YR = (AccDate.Month > 3) ? AccDate.Year : AccDate.Year - 1;
            }
            catch
            {

            }
            YR = YR + 1;
            if (CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.StartsWith("d"))
            {
                return Convert.ToDateTime("31/03/" + YR);
            }
            else
            {
                return Convert.ToDateTime("03/31/" + YR);
            }

        }
        public void ReadFlagValue()
        {
            string value = "";
            DbConn con = new DbConn();
            string flag = con.Db_Multi.First(p => p.DatabaseName != "").SaleFlag;
            string[] Flag_arry = flag.Split('|');
            for (int i = 0; i <= Flag_arry.Length - 1; i++)
            {
                string[] FlagVal = Flag_arry[i].Split(',');
                string Flag = "pRetailFreeQty";
                if (FlagVal[0].ToLower() == Flag.ToLower())
                {
                    Flag objFlag = new Flag();
                    value = FlagVal[1].ToString();
                    objFlag.Key = Flag;
                    objFlag.Value = value;
                    listFlag.Add(objFlag);
                }

                Flag = "CEILINGROUNDOFF";
                if (FlagVal[0].ToLower() == Flag.ToLower())
                {
                    Flag objFlag = new Flag();
                    value = FlagVal[1].ToString();
                    try
                    {
                        if (value.ToLower() == "true")
                        {
                            CEILINGROUNDOFF = 1;
                        }
                        else if (value.ToLower() == "false")
                        {
                            CEILINGROUNDOFF = 0;
                        }
                        else
                        {
                            CEILINGROUNDOFF = Convert.ToInt16(value);
                        }
                    }
                    catch
                    {
                        CEILINGROUNDOFF = 0;
                    }
                    objFlag.Key = Flag;
                    objFlag.Value = value;
                    listFlag.Add(objFlag);
                }

                Flag = "PROUNDOFF";
                if (FlagVal[0].ToLower() == Flag.ToLower())
                {
                    Flag objFlag = new Flag();
                    value = FlagVal[1].ToString();
                    PRoundOff = Convert.ToBoolean(value);
                    objFlag.Key = Flag;
                    objFlag.Value = value;
                    listFlag.Add(objFlag);
                }
            }

        }
        public void OrderWallet(string OrdVrno, string PatientId, string Flag, string Remark, string EWD)
        {
            string sql = "insert into ORD_WALLETE(OrdVrno,Date,PatientId,Flag,Remark,EWD) values('" + OrdVrno + "',getdate(),'" + PatientId + "','" + Flag + "','" + Remark + "'," + EWD + ")";
            ExecuteQuery(true, sql);
        }

        public string getWalletBalance(string Ptcode)
        {
            string Bal = "";
            try
            {
                string SQl = "select cast( sum(isnull(addamt,0))-sum(case when isnull(sbvrno,'')='' then isnull(ewd,0) else isnull(dedamt,0) end )as varchar(50))  Amt " +
                             " from ord_wallete where flag <>'D' and patientId='" + Ptcode + "'" +
                             " union" +
                             " select * from(" +
                             " select top 1 case" +
                             " when flag='A' then 'Credit '+ cast(Addamt as varchar(50))+ ' Rs'" +
                             " when flag='S' AND isnull(sbvrno,'') =''  then 'Estimate '+ cast(ewd as varchar(50))+ ' Rs'" +
                             " when flag='S' AND isnull(sbvrno,'') <>'' then 'Debit '+ cast(Dedamt as varchar(50))+ ' Rs'" +
                             " else '0 Rs' end as Amt from Ord_Wallete" +
                             " Where Isnull(PatientId,'')='" + Ptcode + "'  order by  [Date] desc ) as T ";

                //string SQl = "Select  cast(cast( Sum(Isnull(AddAmt,0)-Isnull(DedAmt,0)-Isnull(ewd,0)) as  Decimal(16,2)) as varchar(50))  As Amt from (" +
                //            " Select Sum(Isnull(AddAmt,0)) As AddAmt, 0 As DedAmt,0 As EWD,  PatientId from Ord_Wallete Where Isnull(Flag,'')='A' Group By PatientId" +
                //            " Union " +
                //            " Select 0 As AddAmt, Sum(Isnull(ewd,0)) As DedAmt,0 As EWD,  PatientId from Ord_Wallete Where Isnull(Flag,'')='S' Group By PatientId " +
                //            "Union "+
                //            " Select top 1 0 As AddAmt, 0 As DedAmt,case when isnull(SbVrNo,'')='' then ewd else  0 end ewd, PatientId from Ord_Wallete Where Isnull(Flag,'')='S'  order by  [Date] desc"+							 
                //            " ) as t1" +
                //            " Where Isnull(PatientId,'')='" + Ptcode + "'" +
                //            " union" +
                //            " select * from(" +
                //            " select top 1 case" +
                //            " when flag='A' then 'Credit '+ cast(Addamt as varchar(50))+ ' Rs'" +
                //            " when flag='S' AND isnull(sbvrno,'') =''  then 'Estimate '+ cast(ewd as varchar(50))+ ' Rs'" +
                //            " when flag='S' AND isnull(sbvrno,'') <>'' then 'Debit '+ cast(Dedamt as varchar(50))+ ' Rs'" +
                //            " else '0 Rs' end as Amt from Ord_Wallete" +
                //            " Where Isnull(PatientId,'')='" + Ptcode + "'  order by  [Date] desc ) as T ";
                DataTable dt = getDataTable(true, SQl);
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    Bal = dt.Rows[i]["Amt"].ToString() + "|" + Bal;
                }
                return Bal;
            }
            catch
            {
                throw;
            }

        }

        public DataTable GetLastTransc(string ptcode)
        {
            try
            {
                string sql = "select case when SbVrNo='' then 'Estimate '+ cast(ewd as varchar(50)) else 'Debit '+ cast(dedamt as varchar(50)) end as dedamt,ordvrno,addamt,SbVrNo,billamt,status  from (select w.ordvrno, cast( sum( isnull( w.addamt,0) )as Decimal(16,2))addamt , cast(  isnull( s.netamt,0) as Decimal(16,2))billamt , isnull(s.vrno,'') SbVrNo,cast( sum(isnull(w.dedamt,0))as Decimal(16,2))dedamt 	,cast( sum(isnull(w.ewd,0))as Decimal(16,2))ewd ,o.status			 from Ord_Wallete w  left join ob o on o.vrno=w.ordvrno   left join sb s on s.vrno	= o.orderno where w.patientid='" + ptcode + "' group by w.ordvrno,s.vrno,s.netamt,o.status	 	) T  order by Ordvrno desc";
                DataTable dt = getDataTable(true, sql);
                return dt;
            }
            catch
            {
                throw;
            }
        }

    }


    [Table("HrMaster")]
    public class HrMaster
    {
        [Key]
        public string HrCode { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string address { get; set; }
        public string Title { get; set; }
        public int? UserType { get; set; }
        // public double Maxdisc { get; set; }
    }

    public class AutoList
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Desr { get; set; }
        public string Address { get; set; }
        public string Company { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string HospPtId { get; set; }
        public string HrCode { get; set; }
    }



    public class ItemMaster
    {
        [Key]
        public string ICODE { get; set; }
        public string INAME { get; set; }
        public string Mrp { get; set; }
        public decimal stk { get; set; }
        public string WStk { get; set; }
        public string packing { get; set; }
        public string shelf { get; set; }
        public string pursize { get; set; }
        public string Rate { get; set; }
        public string WRate { get; set; }
        public string free { get; set; }
        public string Qty { get; set; }
        public string Wfree { get; set; }
        public string WQty { get; set; }
        public string GNAme { get; set; }
    }
    public class ItemMasterWsale
    {
        [Key]
        public string ICODE { get; set; }
        public string INAME { get; set; }
        public string Mrp { get; set; }
        public string stk { get; set; }
        public string WStk { get; set; }
        public string packing { get; set; }
        public string shelf { get; set; }
        public string pursize { get; set; }
        public string Rate { get; set; }
        public string WRate { get; set; }
        public string free { get; set; }
        public string Qty { get; set; }
        public string GNAme { get; set; }
    }

    [Table("IM")]
    public class IM
    {
        [Key]
        public string ICODE { get; set; }
        public string INAME { get; set; }
        public string packing { get; set; }
        public double? MSTSALE { get; set; }
        public double? MSTWSALE { get; set; }
        public string MSTSFLAG { get; set; }
        public string MSTWSFLAG { get; set; }
        public string GNRCODE { get; set; }
        public string Strength { get; set; }
        public double? PURSIZE { get; set; }
        public string GCODE { get; set; }
    }

    public class Order
    {
        public string vrno { get; set; }
        public string vrdate { get; set; }
        public List<ItemMaster> items { get; set; }
        public string series { get; set; }
        public string pcode { get; set; }
        public string TotalAmt { get; set; }
        public string isPrint { get; set; }
        public string Userid { get; set; }
        public string Batch { get; set; }
        public string PatientID { get; set; }
        public string NameP { get; set; }
        public string Addr { get; set; }
        public string DCode { get; set; }
        public string DrName { get; set; }
        public string DrAddr { get; set; }
        public string imgName { get; set; }
        public string FDName { get; set; }
        public string DelCharges { get; set; }
        public string AppType { get; set; }
        public string Iwqty { get; set; }
        public string EWD { get; set; }
        public string Disc { get; set; }
        public string pWallet { get; set; }
        public string status { get; set; }
    }
    public class Order1
    {
        public string vrno { get; set; }
        public string vrdate { get; set; }
        public string series { get; set; }
        public string pcode { get; set; }
        public string TotalAmt { get; set; }
        public string isPrint { get; set; }
        public string Userid { get; set; }
        public string Batch { get; set; }
        public string PatientID { get; set; }
        public string NameP { get; set; }
        public string Addr { get; set; }
        public string DCode { get; set; }
        public string DrName { get; set; }
        public string DrAddr { get; set; }
        public string imgName { get; set; }
        public string FDName { get; set; }
        public string DelCharges { get; set; }
        public string AppType { get; set; }
        public string Iwqty { get; set; }
        public string EWD { get; set; }
    }


    [Table("rm")]
    public class RM
    {
        public string SlNo { get; set; }
        [Key]
        public string ICODE { get; set; }
        public string RCODE { get; set; }
        public double? MRP { get; set; }
        public decimal? PURCPRICE { get; set; }
        public decimal? SALEPRICE { get; set; }
        public double? SURCHARGE { get; set; }
        public double? EXCISE { get; set; }
        public double? CST { get; set; }
        public double? MST { get; set; }
        public double? OCTROI { get; set; }
        public decimal? TRADEPRICE { get; set; }
        public decimal? LOCALPRICE { get; set; }
        public decimal? OUTSTPRICE { get; set; }
        public decimal? ADPRICE { get; set; }
        public decimal? COSTPRICE { get; set; }
        public decimal? CALSRATE { get; set; }
        public decimal? MARGINSALEP { get; set; }
        public decimal? MARGINLOCALR { get; set; }
        public double? VAT { get; set; }

    }

    public class Batch
    {
        public string BatchNo { get; set; }
        public string packing { get; set; }
        public decimal Mrp { get; set; }
        public DateTime? Expiry { get; set; }
        public string stock { get; set; }
        public string Wstock { get; set; }
        public string Rate { get; set; }
        public string Expirydate { get; set; }
        public string VrNo { get; set; }
        public string slno { get; set; }
        public string TotStk { get; set; }
        public DateTime? Vrdate { get; set; }
        public string Vrdate_ { get; set; }
    }

    [Table("ACPMST")]
    public class ACPMST
    {
        [Key]
        public string PCODE { get; set; }
        public string PTYPE { get; set; }
        public string SCODE { get; set; }
        public string PNAME { get; set; }
        public string salerate { get; set; }
        public string areacode { get; set; }
        public string type { get; set; }
        public string Add1 { get; set; }
        public double? discount { get; set; }
        public string ConsumerMob { get; set; }
        public string PtCode { get; set; }
       // public do DISCOUNT { get; set; }
    }

    [Table("AreaMaster")]
    public class AreaMaster
    {
        [Key]
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string Pin { get; set; }

    }

    [Table("OB")]
    public class OB
    {
        [Key]
        public string VRNO { get; set; }
        public DateTime? VRDATE { get; set; }
        public string PCODE { get; set; }
        public double? ORAMT { get; set; }
        public string TRCode { get; set; }
        public string PresImg { get; set; }
        public string PatientID { get; set; }
        public string NameP { get; set; }
        public string Addr { get; set; }
        public string DCode { get; set; }
        public string DrName { get; set; }
        public string DrAddr { get; set; }
        public string OrderFrom { get; set; }
        public string Status { get; set; }
        public string BillDt { get; set; }
        public double? BillAmt { get; set; }
        public string BillRemark { get; set; }
        public string CourierName { get; set; }
        public string DisDt { get; set; }
        public string ExpDelDt { get; set; }
        public string DeliverdTo { get; set; }
        public string DelDt { get; set; }
        public double? PaidAmt { get; set; }
        public string DispRemark { get; set; }
        public string DelvRemark { get; set; }
        public string ORDERNO { get; set; }
        public double? ORDADDAMT { get; set; }
        public double? ORDLESS { get; set; }
        public double? ORDGROSS { get; set; }
        public string ORDADDDESC { get; set; }
        public double? EWD { get; set; }
    }

    [Table("OI")]
    public class OI
    {
        [Key, Column(Order = 0)]
        public string VRNO { get; set; }
        public DateTime? VRDATE { get; set; }

        [Key, Column(Order = 1)]
        public string SLNO { get; set; }
        public string ICODE { get; set; }
        public double? ORDERQTY { get; set; }
        public double? FREEQTY { get; set; }
        public double? ORDERWQTY { get; set; }
        public double? FREEWQTY { get; set; }
        public double? CANCQTY { get; set; }
        public double? ORDMRP { get; set; }
        public double? ORDRATE { get; set; }
        public double? ORAMT { get; set; }
        public double? ORDVAL { get; set; }
        public string ORDBATCH { get; set; }
        public string PCODE { get; set; }
        public string ORDYN { get; set; }
        public string CSMAPICODE { get; set; }
        public string Remark { get; set; }
        public double? SCHMPER { get; set; }


    }

    [Table("CO")]
    public class CO
    {
        [Key, Column(Order = 0)]
        public string VRNO { get; set; }
        public DateTime VRDATE { get; set; }

        [Key, Column(Order = 1)]
        public string SLNO { get; set; }
        public string ICODE { get; set; }
        public double? ORDERQTY { get; set; }
        public double? FREEQTY { get; set; }
        public double? CANCQTY { get; set; }
        public double? ORDMRP { get; set; }
        public double? ORDRATE { get; set; }
        public double? ORAMT { get; set; }
        public double? ORDVALUE { get; set; }
        public string PCODE { get; set; }
        public string ORDYN { get; set; }
        public string Remark { get; set; }
        public double? SCHMPER { get; set; }


    }
    //change vivek
    [Table("ACPMST_temp")]
    public class ACPMST_temp
    {
        [Key]
        public string PtCode { get; set; }
        public string PtName { get; set; }
        public string Address { get; set; }
        public string PhNo { get; set; }
        public string Email { get; set; }
        public string ServicePath { get; set; }
        public string PCODE { get; set; }
        public DateTime RegDate { get; set; }
        public string DeviceId { get; set; }
        public string StCode { get; set; }
        public string CtCode { get; set; }
        public string Area { get; set; }
        public string PREPTCODE { get; set; }
    }


    // change up

    [Table("PO")]
    public class PO
    {
        [Key, Column(Order = 0)]
        public string VRNO { get; set; }
        public DateTime VRDATE { get; set; }

        [Key, Column(Order = 1)]
        public string SLNO { get; set; }
        public string ICODE { get; set; }
        public double? ORDERQTY { get; set; }
        public double? FREEQTY { get; set; }
        public double? CANCQTY { get; set; }
        public double? ORDMRP { get; set; }
        public double? ORDRATE { get; set; }
        public double? ORAMT { get; set; }
        public double? ORDVALUE { get; set; }
        public string PCODE { get; set; }
        public string ORDYN { get; set; }


    }

    [Table("MultipleFirm")]
    public class MultipleFirm
    {
        [Key]
        public string DatabaseName { get; set; }
        public string SaleFlag { get; set; }
    }

    [Table("CR")]
    public class CR
    {
        [Key, Column(Order = 0)]
        public string VRNO { get; set; }

        [Key, Column(Order = 1)]
        public string SLNO { get; set; }
        public DateTime VRDATE { get; set; }
        public string ICODE { get; set; }
        public string TRCODE { get; set; }
        public double? RATE { get; set; }
        public double? COSTRATE { get; set; }
        public decimal? MSTAMT { get; set; }
        public double? MRP { get; set; }
    }

    [Table("PRINTQ")]
    public class PRINTQ
    {
        [Key]
        public int Id { get; set; }
        public string TrCode { get; set; }
        public string VrCode { get; set; }
        public string UserId { get; set; }
        public string NodeId { get; set; }
        public string IP { get; set; }
        public DateTime InsDateTime { get; set; }
        public DateTime? PrintDateTime { get; set; }
    }

    public class PartyLdgr
    {
        public string Dt { get; set; }
        public string Description { get; set; }
        public double? dramt { get; set; }
        public double? cramt { get; set; }
        public double? Balance { get; set; }
        public double? OPBalance { get; set; }
        public string Error { get; set; }
    }

    public class PartyLedger
    {
        public string Name { get; set; }
        public string Pname { get; set; }
        public double? Debit { get; set; }
        public double? Credit { get; set; }
        public double? OPBalance { get; set; }
        public string Ptype { get; set; }
        public string Code { get; set; }
        public string CodeCr { get; set; }
        public string AcVrno { get; set; }
        public string AcSlno { get; set; }
        public DateTime? Vrdate { get; set; }
        public DateTime? vrtime { get; set; }
        public string InsType { get; set; }
        public string Insno { get; set; }
        public DateTime? InsDate { get; set; }
        public string Mdate { get; set; }
        public string Narr1 { get; set; }
        public string Narr2 { get; set; }
        public string RefNo { get; set; }
        public string facode { get; set; }
        public string PatientId { get; set; }
        public int ord { get; set; }
    }

    public class PartyOpBal
    {
        public string PCode { get; set; }
        public string PartyName { get; set; }
        public double? OpBalance { get; set; }
    }

    [Table("VC")]
    public class VC
    {
        [Key, Column(Order = 0)]
        public string FDNAME { get; set; }

        [Key, Column(Order = 1)]
        public string SER { get; set; }
        public string SerRate { get; set; }
        public int? SerPerc { get; set; }

    }

    public class VCList
    {
        public string FDNAME { get; set; }
        public List<VC> vcli { get; set; }
    }


    [Table("TmpSI")]
    public class TmpSI
    {

        [Key, Column(Order = 0)]
        public string VRNO { get; set; }

        [Key, Column(Order = 1)]
        public string SLNO { get; set; }
        public string ICODE { get; set; }
        public string BATCHNO { get; set; }
        public double MRP { get; set; }
        public double RATE { get; set; }
        public double VALUE { get; set; }
        public double QTYISSUED { get; set; }
        public double FREEISSUED { get; set; }
        public DateTime EXPIRY { get; set; }
        public double PRATE { get; set; }
        public string CONVRNO { get; set; }
        public string CONSLNO { get; set; }
        public double MST { get; set; }
        public string MSTFLAG { get; set; }
        public double UNITTAX { get; set; }
        public string VATAPPLIED { get; set; }
        public double EXCISE { get; set; }
        public double EXCISEAMT { get; set; }
        public double SPDISCOUNT { get; set; }
        public double SPDISCAMT { get; set; }
        public double SDISCOUNT { get; set; }
        public double SDISCAMT { get; set; }
        public string ORDVRNO { get; set; }
        public string ORDSLNO { get; set; }
        public string DMVRNO { get; set; }
        public double ESTQTY { get; set; }
        public string SireMark { get; set; }
        public double MSTAMT { get; set; }
        public double MSTVAL { get; set; }
        public double REVMST { get; set; }
        public string REVMSTFLAG { get; set; }
        public double REVMSTAMT { get; set; }
        public double REVMSTVAL { get; set; }
        public double RateNet { get; set; }
        public double Cst { get; set; }
        public string CstFlag { get; set; }
        public double CSTAMT { get; set; }
        public double CSTVAL { get; set; }
        public string Narr1 { get; set; }
        public string Narr2 { get; set; }
        public double WQTYISSUED { get; set; }
        public double WFREEISSUED { get; set; }
        public double WRATE { get; set; }
        public string RATETYPE { get; set; }
        public string Packing { get; set; }
        public double PURSIZE { get; set; }
        // public double MaxDisc { get; set; }

        public ArrayList mArrlist = new ArrayList();
        Global objGbl = new Global();
        GBCDbConn gbcCon = new GBCDbConn();
        public void SaveDataTMPSI(TmpSI objSI)
        {
            string strList = "", Mode;

            mArrlist.Clear();
            DataTable dt = objGbl.getDataTable(true, "select * from tmpsi where vrno='" + VRNO + "' and slno='" + SLNO + "'");

            if (dt.Rows.Count == 0)
            {
                Mode = "INS";
            }
            else
            {
                Mode = "UPD";
            }
            mArrlist.Add("@Mode = " + Mode);
            mArrlist.Add("@VRNO = " + "'" + objSI.VRNO + "'");
            mArrlist.Add("@SLNO = " + "'" + objSI.SLNO + "'");
            mArrlist.Add("@ICODE = " + "'" + objSI.ICODE.Replace("'", "''") + "'");
            mArrlist.Add("@BATCHNO = " + "'" + objSI.BATCHNO + "'");
            mArrlist.Add("@MRP = " + objSI.MRP);
            mArrlist.Add("@RATE = " + objSI.RATE);
            mArrlist.Add("@VALUE = " + objSI.VALUE);
            mArrlist.Add("@QTYISSUED = " + objSI.QTYISSUED);
            mArrlist.Add("@FREEISSUED = " + objSI.FREEISSUED);
            string dt1 = objSI.EXPIRY == Convert.ToDateTime("01/01/0001 12:00:00 AM") ? "" : objSI.EXPIRY.ToString("MM/dd/yyyy");
            mArrlist.Add("@EXPIRY = " + "'" + dt1 + "'");
            mArrlist.Add("@PRATE = " + objSI.PRATE);
            mArrlist.Add("@CONVRNO = " + "'" + objSI.CONVRNO + "'");
            mArrlist.Add("@CONSLNO = " + "'" + objSI.CONSLNO + "'");
            mArrlist.Add("@MST = " + objSI.MST);
            mArrlist.Add("@MSTFLAG = " + "'" + objSI.MSTFLAG + "'");
            mArrlist.Add("@UNITTAX = " + objSI.UNITTAX);
            mArrlist.Add("@VATAPPLIED = " + "'" + objSI.VATAPPLIED + "'");
            mArrlist.Add("@EXCISE = " + objSI.EXCISE);
            mArrlist.Add("@EXCISEAMT = " + objSI.EXCISEAMT);
            mArrlist.Add("@SPDISCOUNT = " + objSI.SPDISCOUNT);
            mArrlist.Add("@SPDISCAMT = " + objSI.SPDISCAMT);
            mArrlist.Add("@SDISCOUNT = " + objSI.SDISCOUNT);
            mArrlist.Add("@SDISCAMT = " + objSI.SDISCAMT);
            mArrlist.Add("@ORDVRNO = " + "'" + objSI.ORDVRNO + "'");
            mArrlist.Add("@ORDSLNO = " + "'" + objSI.ORDSLNO + "'");
            mArrlist.Add("@DMVRNO = " + "'" + objSI.DMVRNO + "'");
            mArrlist.Add("@ESTQTY = " + objSI.ESTQTY);
            mArrlist.Add("@SireMark = " + "'" + objSI.SireMark + "'");
            mArrlist.Add("@MSTAMT = " + "'" + objSI.MSTAMT + "'");
            mArrlist.Add("@MSTVAL = " + "'" + objSI.MSTVAL + "'");
            mArrlist.Add("@REVMST = " + "'" + objSI.REVMST + "'");
            mArrlist.Add("@REVMSTFLAG = " + "'" + objSI.REVMSTFLAG + "'");
            mArrlist.Add("@REVMSTAMT = " + "'" + objSI.REVMSTAMT + "'");
            mArrlist.Add("@REVMSTVAL = " + "'" + objSI.REVMSTVAL + "'");
            mArrlist.Add("@RateNet = " + "'" + objSI.RateNet + "'");
            mArrlist.Add("@Cst = " + "'" + objSI.Cst + "'");
            mArrlist.Add("@CstFlag = " + "'" + objSI.CstFlag + "'");
            mArrlist.Add("@CSTAMT = " + "'" + objSI.CSTAMT + "'");
            mArrlist.Add("@CSTVAL = " + "'" + objSI.CSTVAL + "'");
            mArrlist.Add("@Narr1 = " + "'" + objSI.Narr1 + "'");
            mArrlist.Add("@Narr2 = " + "'" + objSI.Narr2 + "'");
            mArrlist.Add("@WRATE = " + objSI.WRATE);
            mArrlist.Add("@WQTYISSUED = " + objSI.WQTYISSUED);
            mArrlist.Add("@WFREEISSUED = " + objSI.WFREEISSUED);
            mArrlist.Add("@RATETYPE = " + "'" + objSI.RATETYPE + "'");
            mArrlist.Add("@Pursize = " + objSI.PURSIZE);
            mArrlist.Add("@Packing = " + "'" + objSI.Packing.Replace("'", "''") + "'");
            for (int i = 0; i <= mArrlist.Count - 1; i++)
            {
                strList = (string.IsNullOrEmpty(strList) ? "" : strList + ", ") + mArrlist[i];
            }

            string SQL = "EXEC Pr_TMPSI " + strList;
            objGbl.ExecuteQuery(true, SQL);

        }

        public string GetTMPTBVrno(string fdname, string Ser, string formNo, string _vrno, string NodeName, string LogId)
        {
            string SQL;
            DataRow dr;
            try
            {
                if (fdname.Trim() != "" && Ser.Trim() != "" && formNo.Trim() != "")
                {
                    if (_vrno == "")
                        SQL = "Declare @strvrno  as varchar (10),@Cnt as varchar (2); " + "\r\n" +
                       "set @strvrno =Right('0'+ cast( DATEPART(DAY, GETDATE ()) as varchar (2)),2)+Right('0'+ cast( DATEPART(HOUR, GETDATE ()) as varchar (2)),2)+Right('0'+ cast( DATEPART(MINUTE, GETDATE ()) as varchar (2)),2)+Right('0'+ cast( DATEPART(second, GETDATE ()) as varchar (2)),2); " + "\r\n" +
                       "SET @Cnt=  (select '0'+cast(Isnull(Max(Right(Isnull(TMPVRNO,''),2)),0)+1 as varchar (2)) from TMPTB where Isnull(TMPVRNO,'') like ''+@strvrno+'%' ); " + "\r\n" +
                        "set @strvrno =@strvrno+@Cnt " + "\r\n" +
                        "Insert Into TMPTB (TMPVRNO,INSDATE,TRCODE,NODENAME,USERID,SALEFORMNO,SER) values( @strvrno,GETDATE (),'" + fdname + "','" + NodeName + "','" + LogId + "','" + formNo + "','" + Ser + "'); " + "\r\n" +
                       "select @strvrno as tmpvrno;";
                    else
                        SQL = "Declare @strvrno  as varchar (10); " + "\r\n" +
                           "set @strvrno = '" + _vrno + "'" + "\r\n" +
                            "IF NOT EXISTS ( SELECT tmpvrno from TMPTB Where TmpVrno = @strvrno )  Insert Into TMPTB (TMPVRNO,INSDATE,TRCODE,NODENAME,USERID,SALEFORMNO,SER) Values( @strvrno,GETDATE (),'" + fdname + "','" + NodeName + "','" + LogId + "','" + formNo + "','" + Ser + "' ); " + "\r\n" +
                           "select @strvrno as tmpvrno;";


                    dr = objGbl.getDataRow(true, SQL);
                    if (dr != null)
                        return (dr.IsNull("tmpvrno") ? "" : dr["tmpvrno"].ToString());
                    else
                        return "";
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

    }


    public class SaveInvoice
    {
        public string TempVrNo { get; set; }
        public string VrSeries { get; set; }
        public string WthVrno { get; set; }
        public string VRNO { get; set; }
        public string VRDATE { get; set; }
        public string PCODE { get; set; }
        public string VRTYPE { get; set; }
        public string TRCODE { get; set; }
        public string CCODE { get; set; }
        public double GROSS { get; set; }
        public double FRGAMT { get; set; }
        public double ADDPERC { get; set; }
        public double ADDAMT { get; set; }
        public string ADDDESC { get; set; }
        public double LESSPERC { get; set; }
        public double LESS { get; set; }
        public string LESSDESC { get; set; }
        public double ROUNDOFF { get; set; }
        public double NETAMT { get; set; }
        public string SKCODE { get; set; }
        public string USERNO { get; set; }
        public string USRID { get; set; }
        public string VrTime { get; set; }
        public bool ISEST { get; set; }
        public string SBVRNO { get; set; }
        public double NOP { get; set; }
        public string DMNO { get; set; }
        public DateTime? DMDATE { get; set; }
        public DateTime? ADMDATE { get; set; }
        public string SRCODE { get; set; }
        public string ORDERNO { get; set; }
        public DateTime? ORDERDATE { get; set; }
        public string CHEQUENO { get; set; }
        public DateTime? CHEQUEDATE { get; set; }
        public string LRNO { get; set; }
        public DateTime? LRDATE { get; set; }
        public string CARRIER { get; set; }
        public string CARRTYPE { get; set; }
        public double DISCOUNT { get; set; }
        public double MST { get; set; }
        public double PTAXVAT { get; set; }
        public double STAXVAT { get; set; }
        public double OCTPERC { get; set; }
        public double OCTROI { get; set; }
        public double SDISCOUNT { get; set; }
        public double EXCISE { get; set; }
        public double SPDISCOUNT { get; set; }
        public string RETVRNO { get; set; }
        public string GDNCODE { get; set; }
        public string NameP { get; set; }
        public string Addr { get; set; }
        public string DCode { get; set; }
        public string DrName { get; set; }
        public string DrAddr { get; set; }
        public double TCS { get; set; }
        public string UserId { get; set; }
        public string SBNARR1 { get; set; }
        public string SBNARR2 { get; set; }
        public double BAL_AMT { get; set; }
        public string PAID { get; set; }
        public string REFNO { get; set; }
        public string PatientID { get; set; }
        public string RoundoffYN { get; set; }
        public string CashrUser { get; set; }
        public string CashrNode { get; set; }
        public DateTime? CashrDt { get; set; }
        public string PresImg { get; set; }
        public string PtPhone { get; set; }
        public double CarAmt { get; set; }
        public double DiscPer { get; set; }
        public double MstAmt { get; set; }
        public double DrAmt { get; set; }
        public string NodeId { get; set; }
        public string TaxType { get; set; }
        public double Cst { get; set; }
        public double Weight { get; set; }
        public double StdCase { get; set; }
        public double MixCase { get; set; }
        public double CFormNo { get; set; }
        public string CFormDate { get; set; }
        public string SbcCode { get; set; }
        public string VehicleNo { get; set; }
        public string PostInCode { get; set; }
        public string TrfVrno { get; set; }
        public string CARDNO { get; set; }
        public string CARDHOLDERNAME { get; set; }
        public string BANKERNAME { get; set; }
        public DateTime? CARDEXPIRY { get; set; }
        public string CVVNO { get; set; }
        public string RecdAmt { get; set; }
        public string RtnAmt { get; set; }
        public string VATAPPLIED { get; set; }
        public string BILLNO { get; set; }
        public string BILLDATE { get; set; }
        public double DEDUCT { get; set; }
        public double ADD { get; set; }
        public double CRAMT { get; set; }
        public string ADJVRNO { get; set; }
        public double ADJAMT { get; set; }
        public int ADJDAYS { get; set; }
        public double GROSSMRP { get; set; }
        public string ENTTIME { get; set; }
        public string BRNARR { get; set; }
        public string BRNARR2 { get; set; }
        public string YN { get; set; }
        public string ADJUSTIN { get; set; }
        public double DEDMRP { get; set; }
        public string VATTYPE { get; set; }
        public double OneThosand { get; set; }
        public double FiveH { get; set; }
        public double OneH { get; set; }
        public double Fifty { get; set; }
        public double Twenty { get; set; }
        public double Ten { get; set; }
        public double Coins { get; set; }
        public double Paise { get; set; }
        public string GBCName { get; set; }
        public string AcYr { get; set; }

    }

    public class AdjustBatch
    {
        public string BatchNo { get; set; }
        public string Vrno { get; set; }
        public string Slno { get; set; }
        public decimal MRP { get; set; }
        public string Rate { get; set; }
        public string Expirydate { get; set; }
        public string qty { get; set; }
        public string free { get; set; }
        public string packing { get; set; }

    }

    public class Bill
    {
        public string GrossAmt { get; set; }
        public string NetAmt { get; set; }
        public string LessPerc { get; set; }
        public string Less { get; set; }
        public string cnt { get; set; }
        public string MstAmt { get; set; }
    }

    public class PatientMaster
    {
        [Key]
        public string PtCode { get; set; }
        public string PtName { get; set; }
        public string Address { get; set; }
        public string PhNo { get; set; }
        public string Email { get; set; }
        public string ServicePath { get; set; }
        public string PCODE { get; set; }
        public DateTime RegDate { get; set; }
        public string DeviceId { get; set; }
        public string StCode { get; set; }
        public string CtCode { get; set; }
        public string Area { get; set; }
        public string FDocCode { get; set; }
        public string FDocName { get; set; }
    }

    public class SaleReport
    {
        public string Vrno { get; set; }
        public DateTime Vrdate { get; set; }
        public string VrType { get; set; }
        public double NetAmt { get; set; }
        public string Vrdt { get; set; }
        public string crAmt { get; set; }
        public string cashAmt { get; set; }
        public string Pcode { get; set; }
        public string Pname { get; set; }
        public string NameP { get; set; }
        public string PatientID { get; set; }

    }

    public class SubstitudeItem
    {
        public string Icode { get; set; }
        public string Iname { get; set; }
        public string Genric { get; set; }
        public double? MRP { get; set; }
        public string Rate { get; set; }
        public string WRate { get; set; }
        public string Packing { get; set; }
        public string pursize { get; set; }
        public string shelf { get; set; }
        public string Stk { get; set; }
        public string minexpiry { get; set; }
        public string maxexpiry { get; set; }
        public string Margin { get; set; }
    }

    public class SaveRecieptInvoice
    {
        public string FDName { get; set; }
        public string VrNo { get; set; }
        public string CrVrNo { get; set; }
        public string AdjAmt { get; set; }
        public string DrVrNo { get; set; }
        public string DrSlNo { get; set; }
        public string CrSlNo { get; set; }
        public string AcVrno { get; set; }
        public string VrDate { get; set; }
        public string trcode { get; set; }
        public string doc { get; set; }
        public string VrType { get; set; }
        public string VrTime { get; set; }
        public string DocNo { get; set; }
        public string Ddate { get; set; }
        public string DrCr { get; set; }
        public string AVCode { get; set; }
        public string AVType { get; set; }
        public string Narr1 { get; set; }
        public string Narr2 { get; set; }
        public string VrAmt { get; set; }
        public string LogId { get; set; }
        public string NodeName { get; set; }
        public string DeptCode { get; set; }
        public string DCode { get; set; }
        public string Pcode { get; set; }
        public string AtType { get; set; }
        public string TrAmt { get; set; }
        public string Bankers1 { get; set; }
        public string Bankers2 { get; set; }
        public string Series { get; set; }
        public string VrUserId { get; set; }
        public string CodeDr { get; set; }
        public string CodeCr { get; set; }
        public string ConAmtDr { get; set; }
        public string ConAmtCr { get; set; }
        public string FACODE { get; set; }
        public string CDate { get; set; }
        public string BalAmt { get; set; }
        public string TOF { get; set; }
        public string RefNo { get; set; }
        public Boolean IsOutstanding { get; set; }
        public Boolean IsLdgFlag { get; set; }
    }

    //public class ClsRPVoucher
    //{
    //    public string VrNo { get; set; }
    //    public string AcVrNo { get; set; }
    //    public string AcSlNo { get; set; }
    //    public string AVType { get; set; }
    //    public string CrVrNo { get; set; }
    //    public string CrSlNo { get; set; }
    //    public string DrVrNo { get; set; }
    //     public string Bankers1 { get; set; }
    //    public string Bankers2 { get; set; }
    //    public string Bankers3 { get; set; }
    //    public string TOF { get; set; }
    //    public double AlAmt { get; set; }
    //    public double AdjAmt { get; set; }
    //    public double BalAmt { get; set; }
    //    public double VrAmt { get; set; }
    //    public double TrAmt { get; set; }
    //    public double ConAmtDr { get; set; }
    //    public double ConAmtCr { get; set; }
    //    public DateTime VrTime { get; set; }
    //    public bool IsNew { get; set; }
    //    public DateTime VrDate { get; set; }
    //    public string Pcode { get; set; }
    //    public string trcode { get; set; }
    //    public string DrCr { get; set; }
    //    public string AtType { get; set; }
    //    public string VrType { get; set; }
    //    public string ACCode { get; set; }
    //    public string doc { get; set; }
    //    public string DocNo { get; set; }
    //    public DateTime DocDate { get; set; }
    //    public string VrUserID { get; set; }
    //    public string DeptCode { get; set; }
    //    public string Narr1 { get; set; }
    //    public string Narr2 { get; set; }
    //    public string RefNo { get; set; }
    //    public string AVCode { get; set; }
    //    public string Series { get; set; }
    //    public string CodeCr { get; set; }
    //    public string CodeDr { get; set; }
    //    public string Narr { get; set; }
    //    public string Flag { get; set; }
    //    public string DCode { get; set; }
    //    public string facode { get; set; }
    //    public string NodeName { get; set; }
    //    public bool IsOutstanding = false;
    //    public bool IsLdgFlag = false;
    //    public List<clsVrItem> VrItem { get; set; }
    //    TIA3T.NEW.BLL.Classes.clsRPVoucher objRPVoucherN = new TIA3T.NEW.BLL.Classes.clsRPVoucher();
    //    TIA3T.BLL.clsRecieptPayment objRcPy = new TIA3T.BLL.clsRecieptPayment();
    //    TIA3T.NEW.BLL.Modules.stmGlobalMethods obj_stmGlobalMethods = new TIA3T.NEW.BLL.Modules.stmGlobalMethods();
    //    Global objgbl = new Global();

    //    public void SaveDataIntoTMPAV(string GBCDBName, string pcode, string instype, string insno, string avcode)
    //    {
    //        string SQl;
    //        try
    //        {
    //            SQl = "select * from " + GBCDBName + " AVLOG where pcode='" + pcode + "'";
    //            DataRow dr = objgbl.getDataRow(false, SQl);
    //            if (dr != null)
    //            {
    //                SQl = "update " + GBCDBName + " AVLOG set pcode='" + pcode + "',instype='" + instype + "',insno='" + insno + "',avcode='" + avcode + "' where pcode='" + pcode + "'";
    //                objgbl.ExecuteQuery(false, SQl);
    //            }
    //            else
    //            {
    //                SQl = "insert into " + GBCDBName + " AVLOG (pcode,instype,insno,avcode) values('" + pcode + "','" + instype + "','" + insno + "','" + avcode + "')";
    //                objgbl.ExecuteQuery(false, SQl);
    //            }
    //        }
    //        catch
    //        {
    //        }
    //    }
    //    public bool SaveRecord()
    //    {
    //        try
    //        {
    //            if (IsOutstanding == false)
    //            {
    //                SaveAv();
    //                SaveATR();
    //                SaveAc();
    //                SaveTr();
    //            }
    //            if (IsLdgFlag == false)
    //            {
    //                SaveAl();
    //                UpDateTr(1);
    //            }
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public void SaveAl()
    //    {
    //        if (AdjAmt != 0)
    //            objRPVoucherN.insert_AL(Pcode, DrVrNo, DrSlNo, CrVrNo, CrSlNo, AdjAmt.ToString(), VrDate.ToString(), BalAmt.ToString());
    //    }
    //    public void SaveATR()
    //    {
    //        objRPVoucherN.insert_ATR(AcVrNo, Pcode, AtType, TrAmt.ToString(), doc, DocNo, DocDate.ToString(), Narr1, Narr2, Bankers1, Bankers2);
    //    }
    //    public void SaveAc()
    //    {
    //        AVType = "A";
    //        objRPVoucherN.insert_AC(AcVrNo, Series, VrDate.ToString(), VrTime.ToString(), VrUserID, trcode, VrType, DrCr, AVCode, AVType, VrAmt.ToString(), Pcode, AtType, doc, DocNo, DocDate.ToString(), TrAmt.ToString(), Narr1, Narr2, RefNo, NodeName, DeptCode);
    //    }
    //    public void SaveTr()
    //    {
    //        objRPVoucherN.insert_TR(AcVrNo, VrNo, VrDate.ToString(), VrUserID, NodeName, VrTime.ToString(), trcode, doc, RefNo, DocNo, DocDate.ToString(), CodeDr, CodeCr, TrAmt.ToString(), ConAmtDr.ToString(), ConAmtCr.ToString(), Narr1, Narr2, facode, DeptCode, DCode);
    //    }
    //    public void SaveAv()
    //    {
    //        if (IsExistAv(AcVrNo))
    //        {
    //            objRPVoucherN.insert_Acvr(AcVrNo);
    //        }
    //        objRPVoucherN.update_AV(AcVrNo, VrDate.ToString(), trcode, doc, VrType, VrTime.ToString(), VrNo, DocNo, DocDate.ToString(), DrCr, AVCode, AVType, RefNo, Narr1, Narr2, VrDate.ToString(), VrUserID, NodeName, DeptCode, DCode);
    //    }
    //    public Boolean IsExistAv(string Code)
    //    {
    //        obj_stmGlobalMethods.val1 = Code;
    //        DataRow dr = obj_stmGlobalMethods.Select_AVRow("AcVrNo");
    //        if (dr.Table.Rows.Count > 0)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            return true;
    //        }
    //    }
    //    public void DeleteOldEntry()
    //    {
    //        DeleteReciept();
    //    }
    //    public void DeleteReciept()
    //    {
    //        if (IsOutstanding == true)
    //        {
    //            UpDateTr(2);
    //            DeleteAl();
    //        }
    //        else
    //        {
    //            DeleteAc();
    //            DeleteTR();
    //            DeleteATR();
    //            if (IsLdgFlag)
    //            {
    //                UpDateTr(2);
    //                DeleteAl();
    //            }
    //        }
    //    }
    //    public void UpDateTr(int CallType)
    //    {
    //        string tmpstr = "";
    //        int i;
    //        if (CallType == 1)
    //        {
    //            ///*Posting in Tr & Optr
    //            foreach (var objSVReceipt in VrItem)
    //            {
    //                if (objSVReceipt.DrSlNo != "")
    //                {
    //                    tmpstr = " And AcSlNo='" + objSVReceipt.DrSlNo + "'";
    //                }
    //                if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.DrVrNo, "MNRC") == false)  //For Checking Allocation Vouchers in OPTR
    //                {
    //                    objRPVoucherN.update_Dr("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.DrVrNo, tmpstr);
    //                }
    //                else
    //                {
    //                    objRPVoucherN.update_Dr("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.DrVrNo, tmpstr);
    //                }
    //                if (objSVReceipt.CRSlNo != "")
    //                {
    //                    tmpstr = " And AcSlNo='" + objSVReceipt.CRSlNo + "'";
    //                }
    //                if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.CrVrNo, "MNPY") == false)   //For Allocation Vouchers in OPTR
    //                {
    //                    objRPVoucherN.update_Cr("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.CrVrNo, tmpstr);
    //                }
    //                else
    //                {
    //                    objRPVoucherN.update_Cr("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.CrVrNo, tmpstr);
    //                }
    //            }
    //        }
    //        if (CallType == 2)
    //        {
    //            ////*************UpPosting in Tr & Optr
    //            foreach (var objSVReceipt in VrItem)
    //            {
    //                if (objSVReceipt.DrSlNo != "")
    //                {
    //                    tmpstr = " And AcSlNo='" + objSVReceipt.DrSlNo + "'";
    //                }
    //                if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.DrVrNo, "MNRC") == false)
    //                {
    //                    objRPVoucherN.update_Dr1("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.DrVrNo, tmpstr);
    //                }
    //                else
    //                {
    //                    objRPVoucherN.update_Dr1("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.DrVrNo, tmpstr);
    //                }
    //                if (objSVReceipt.CRSlNo != "")
    //                {
    //                    tmpstr = " And AcSlNo='" + objSVReceipt.CRSlNo + "'";
    //                }
    //                if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.CrVrNo, "MNPY") == false)
    //                {
    //                    objRPVoucherN.update_Cr1("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.CrVrNo, tmpstr);
    //                }
    //                else
    //                {
    //                    objRPVoucherN.update_Cr1("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.CrVrNo, tmpstr);
    //                }
    //            }
    //        }
    //    }
    //    public Boolean ChkOpTables(string PCODE, string DrVrNo, string FDName = "")
    //    {
    //        DataRow dr = objRPVoucherN.select_Op(DrVrNo);
    //        if (dr != null)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }
    //    public void DeleteAc()
    //    {
    //        objRPVoucherN.delete("AC", "ACVrNo", AcVrNo);
    //    }
    //    public void DeleteATR()
    //    {
    //        objRPVoucherN.delete("Tr", "AcVrNo", AcVrNo);
    //    }
    //    public void DeleteTR()
    //    {
    //        objRPVoucherN.delete("ATr", "AcVrNo", AcVrNo);
    //    }
    //    public void DeleteAl()
    //    {
    //        string str = trcode.ToUpper();
    //        if (str == "MNRC")
    //        {
    //            TOF = "D";
    //        }
    //        else
    //        {
    //            TOF = "C";
    //        }
    //        if (TOF == "C")
    //        {
    //            objRPVoucherN.delete("Al", "DrVrNo", AcVrNo);
    //        }
    //        else if (TOF == "D")
    //        {
    //            objRPVoucherN.delete("Al", "CrVrNo", AcVrNo);
    //        }
    //    }
    //    public string RtLedgBal(string PCode)
    //    {
    //        try
    //        {
    //            string LedgBal = ShowLDGBalance(PCode);
    //            if (LedgBal.Contains("CR"))
    //            {
    //                LedgBal = LedgBal.Replace("CR", "");
    //                LedgBal = (Convert.ToDouble(LedgBal) * (-1)).ToString();
    //            }
    //            else
    //            {
    //                LedgBal = LedgBal.Replace("DR", "");
    //            }
    //            return LedgBal;
    //        }
    //        catch
    //        {

    //            return "";
    //        }
    //    }
    //    public string ShowLDGBalance(string Selparties)
    //    {
    //        try
    //        {
    //            string SelAmt = null;
    //            string AmtType = "";
    //            obj_stmGlobalMethods.val1 = Selparties;
    //            double TOpBal = 0;
    //            DataRow drLedg = obj_stmGlobalMethods.Select_TRrow("LgBal");
    //            if (drLedg != null)
    //            {
    //                TOpBal = Convert.ToDouble(((drLedg["LgBal"] == null) ? "0" : Convert.ToDouble(drLedg["LgBal"]).ToString("#########0.00")));
    //                SelAmt = CrDr(TOpBal, AmtType);
    //                return SelAmt;
    //            }
    //            else
    //            {
    //                return "0";
    //            }
    //        }
    //        catch
    //        {
    //            return "";
    //        }
    //    }
    //    public string CrDr(double Amount, string Type)
    //    {
    //        string TotAmt = null;
    //        try
    //        {
    //            if (Amount < 0)
    //            {
    //                Type = " Cr";
    //                Amount = Amount * (-1);
    //            }
    //            else
    //            {
    //                Type = " Dr";
    //            }
    //            TotAmt = Amount.ToString("#########0.00");
    //            TotAmt = TotAmt + Type;
    //            return TotAmt;
    //        }
    //        catch
    //        {
    //            return "";
    //        }
    //    }

    //    public string RtOutStBal(string PCode, string Ptype = "")
    //    {
    //        try
    //        {
    //            DataRow dr = (new TIA3T.NEW.BLL.ClsAcpmst()).Select_OnPcode(PCode);
    //            if (dr != null)
    //            {
    //                Ptype = (dr["Ptype"] == null) ? "" : dr["Ptype"].ToString();
    //            }
    //            string OutStBal = ShowOTSDrBalance(PCode, Ptype);
    //            if (OutStBal.Contains("CR"))
    //            {
    //                OutStBal = OutStBal.Replace("CR", "");
    //                OutStBal = (Convert.ToDouble(OutStBal) * (-1)).ToString();
    //            }
    //            else
    //            {
    //                OutStBal = OutStBal.Replace("DR", "");
    //            }
    //            return OutStBal;
    //        }
    //        catch (Exception ex)
    //        {
    //            return "";
    //        }
    //    }

    //    public string ShowOTSDrBalance(string OTSSelparties, string Ptype)
    //    {
    //        try
    //        {
    //            if (Ptype == "C")
    //            {
    //                string OTSDrSelAmt = null;
    //                short i = 0;
    //                string OTSDrAmtType = "";

    //                DataRow DataRow = default(DataRow);

    //                DataTable dt_ = obj_stmGlobalMethods.Select_TRTable("DAMT", OTSSelparties);
    //                double OTSDrTOpBal = 0;
    //                i = 2;
    //                if (dt_.Rows.Count != 0)
    //                {
    //                    for (int k = 0; k <= dt_.Rows.Count - 1; k++)
    //                    {
    //                        DataRow = dt_.Rows[k];
    //                        OTSDrTOpBal = (DataRow["DAMT"] == null) ? 0 : Convert.ToDouble(DataRow["DAMT"]);
    //                        OTSDrSelAmt = CrDr(OTSDrTOpBal, OTSDrAmtType);

    //                    }
    //                    dt_.Clear();
    //                    return OTSDrSelAmt;
    //                }
    //                else
    //                {
    //                    return "";
    //                }
    //            }
    //            else if (Ptype == "S")
    //            {
    //                string OTSCrSelAmt = null;
    //                string OTSCrAmtType = "";
    //                DataRow DataRow = default(DataRow);
    //                DataTable dt_ = obj_stmGlobalMethods.Select_TRTable("CAMT", OTSSelparties);
    //                double OTSCrTOpBal = 0;

    //                if (dt_.Rows.Count != 0)
    //                {
    //                    for (int k = 0; k <= dt_.Rows.Count - 1; k++)
    //                    {
    //                        DataRow = dt_.Rows[k];
    //                        OTSCrTOpBal = (DataRow["CAMT"] == null) ? 0 : Convert.ToDouble(DataRow["CAMT"]);

    //                        OTSCrSelAmt = CrDr(OTSCrTOpBal, OTSCrAmtType);
    //                        return OTSCrSelAmt;
    //                    }
    //                    return OTSCrSelAmt;
    //                }
    //                else
    //                {
    //                    return "";
    //                }
    //            }
    //            else
    //            {
    //                return "";
    //            }
    //        }
    //        catch 
    //        {
    //            return "";
    //        }
    //    }

    //    public bool GetSMSManagement(string Fdname)
    //    {
    //        try
    //        {
    //            int Cnt = 0;
    //           DataRow drsuccess = TIA3T.BLL.clsGlobalMethodsBLL.Select_SMSSettingCnt(Fdname);
    //            if (drsuccess != null)
    //            {
    //                Cnt = (drsuccess["Cnt"]==null) ? 0 : Convert.ToInt16(drsuccess["Cnt"]);
    //                if (Cnt > 0)
    //                {
    //                    return true;
    //                }
    //                else
    //                {
    //                    return false;
    //                }
    //            }
    //            else
    //            {
    //                return false;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }
    //    }
    //    public void UpdatePayee()
    //    {
    //        try
    //        {
    //            string Buyerid = null;
    //             DataRow  drRcPy = objRcPy.Select_RL_Buyer(Pcode, RefNo);
    //            if (drRcPy != null)
    //            {
    //                Buyerid = ((drRcPy["buyerid"]==null) ? "" : drRcPy["buyerid"].ToString());
    //                objRcPy.Update_PAYEE_ConVrNo(AcVrNo, Buyerid, RefNo);
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }


    //    public void UpdatePayer()
    //    {
    //        try
    //        {
    //            string Supplierid = null;
    //            DataRow drRcPy = objRcPy.Select_RL_Supplier(Pcode, RefNo);
    //            if (drRcPy != null)
    //            {
    //                Supplierid = ((drRcPy["Supplierid"]==null) ? "" : drRcPy["Supplierid"].ToString());
    //                objRcPy.Update_RL_ConVrNo(AcVrNo, Supplierid, RefNo);
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }

    //}


    public class ClsRPVoucher
    {
        public string VrNo { get; set; }
        public string AcVrNo { get; set; }
        public string AcSlNo { get; set; }
        public string AVType { get; set; }
        public string CrVrNo { get; set; }
        public string CrSlNo { get; set; }
        public string DrVrNo { get; set; }
        public string DrSlNo { get; set; }
        public string Bankers1 { get; set; }
        public string Bankers2 { get; set; }
        public string Bankers3 { get; set; }
        public string TOF { get; set; }
        public double AlAmt { get; set; }
        public double AdjAmt { get; set; }
        public double BalAmt { get; set; }
        public double VrAmt { get; set; }
        public double TrAmt { get; set; }
        public double ConAmtDr { get; set; }
        public double ConAmtCr { get; set; }
        public DateTime VrTime { get; set; }
        public bool IsNew { get; set; }
        public DateTime VrDate { get; set; }
        public string Pcode { get; set; }
        public string trcode { get; set; }
        public string DrCr { get; set; }
        public string AtType { get; set; }
        public string VrType { get; set; }
        public string ACCode { get; set; }
        public string doc { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string VrUserID { get; set; }
        public string DeptCode { get; set; }
        public string Narr1 { get; set; }
        public string Narr2 { get; set; }
        public string RefNo { get; set; }
        public string AVCode { get; set; }
        public string Series { get; set; }
        public string CodeCr { get; set; }
        public string CodeDr { get; set; }
        public string Narr { get; set; }
        public string Flag { get; set; }
        public string DCode { get; set; }
        public string facode { get; set; }
        public string NodeName { get; set; }
        public bool IsOutstanding = false;
        public bool IsLdgFlag = false;
        public List<clsVrItem> VrItem { get; set; }
        TIA3T.NEW.BLL.Classes.clsRPVoucher objRPVoucherN = new TIA3T.NEW.BLL.Classes.clsRPVoucher();
        TIA3T.BLL.clsRecieptPayment objRcPy = new TIA3T.BLL.clsRecieptPayment();
        TIA3T.NEW.BLL.Modules.stmGlobalMethods obj_stmGlobalMethods = new TIA3T.NEW.BLL.Modules.stmGlobalMethods();
        Global objgbl = new Global();

        public void SaveDataIntoTMPAV(string GBCDBName, string pcode, string instype, string insno, string avcode)
        {
            string SQl;
            try
            {
                SQl = "select * from " + GBCDBName + " AVLOG where pcode='" + pcode + "'";
                DataRow dr = objgbl.getDataRow(false, SQl);
                if (dr != null)
                {
                    SQl = "update " + GBCDBName + " AVLOG set pcode='" + pcode + "',instype='" + instype + "',insno='" + insno + "',avcode='" + avcode + "' where pcode='" + pcode + "'";
                    objgbl.ExecuteQuery(false, SQl);
                }
                else
                {
                    SQl = "insert into " + GBCDBName + " AVLOG (pcode,instype,insno,avcode) values('" + pcode + "','" + instype + "','" + insno + "','" + avcode + "')";
                    objgbl.ExecuteQuery(false, SQl);
                }
            }
            catch
            {
            }
        }
        public bool SaveRecord()
        {
            try
            {
                if (IsOutstanding == false)
                {
                    SaveAv();
                    SaveATR();
                    SaveAc();
                    SaveTr();
                }
                if (IsLdgFlag == false)
                {
                    SaveAl();
                    UpDateTr(1);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SaveAl()
        {
            foreach (var objSVReceipt in VrItem)
            {
                if (objSVReceipt.AdjAmt != 0)
                    objRPVoucherN.insert_AL(objSVReceipt.PCODE, objSVReceipt.DrVrNo, objSVReceipt.DrSlNo, objSVReceipt.CrVrNo, objSVReceipt.CRSlNo, objSVReceipt.AdjAmt.ToString(), objSVReceipt.VrDate, objSVReceipt.BalAmt.ToString());
            }
        }
        public void SaveATR()
        {
            objRPVoucherN.insert_ATR(AcVrNo, Pcode, AtType, TrAmt.ToString(), doc, DocNo, DocDate.ToString("dd/MM/yyyy"), Narr1, Narr2, Bankers1, Bankers2);
        }
        public void SaveAc()
        {
            AVType = "A";
            objRPVoucherN.insert_AC(AcVrNo, Series, VrDate.ToString("MM/dd/yyyy"), VrTime.ToString("hh:mm:ss"), VrUserID, trcode, VrType, DrCr, AVCode, AVType, VrAmt.ToString(), Pcode, AtType, doc, DocNo, DocDate.ToString("MM/dd/yyyy"), TrAmt.ToString(), Narr1, Narr2, RefNo, NodeName, DeptCode);
        }
        public void SaveTr()
        {
            objRPVoucherN.insert_TR(AcVrNo, VrNo, VrDate.ToString("MM/dd/yyyy"), VrUserID, NodeName, VrTime.ToString("hh:mm:ss"), trcode, doc, RefNo, DocNo, DocDate.ToString("dd/MM/yyyy"), CodeDr, CodeCr, TrAmt.ToString(), ConAmtDr.ToString(), ConAmtCr.ToString(), Narr1, Narr2, facode, DeptCode, DCode);
        }
        public void SaveAv()
        {
            if (!IsExistAv(AcVrNo))
            {
                objRPVoucherN.insert_Acvr(AcVrNo);
            }
            objRPVoucherN.update_AV(AcVrNo, VrDate.ToString("dd/MM/yyyy"), trcode, doc, VrType, VrTime.ToString("hh:mm:ss"), VrNo, DocNo, DocDate.ToString("dd/MM/yyyy"), DrCr, AVCode, AVType, RefNo, Narr1, Narr2, VrAmt.ToString(), VrUserID, NodeName, DeptCode, DCode);
        }
        public Boolean IsExistAv(string Code)
        {
            obj_stmGlobalMethods.val1 = Code;
            DataRow dr = obj_stmGlobalMethods.Select_AVRow("AcVrNo");
            if (dr == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void DeleteOldEntry()
        {
            DeleteReciept();
        }
        public void DeleteReciept()
        {
            if (IsOutstanding == true)
            {
                UpDateTr(2);
                DeleteAl();
            }
            else
            {
                DeleteAc();
                DeleteTR();
                DeleteATR();
                if (IsLdgFlag)
                {
                    UpDateTr(2);
                    DeleteAl();
                }
            }
        }
        public void UpDateTr(int CallType)
        {
            string tmpstr = "";
            int i;
            if (CallType == 1)
            {
                ///*Posting in Tr & Optr
                foreach (var objSVReceipt in VrItem)
                {
                    if (objSVReceipt.DrSlNo != "")
                    {
                        tmpstr = " And AcSlNo='" + objSVReceipt.DrSlNo + "'";
                    }
                    if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.DrVrNo, "MNRC") == false)  //For Checking Allocation Vouchers in OPTR
                    {
                        objRPVoucherN.update_Dr("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.DrVrNo, tmpstr);
                    }
                    else
                    {
                        objRPVoucherN.update_Dr("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.DrVrNo, tmpstr);
                    }
                    if (objSVReceipt.CRSlNo != "")
                    {
                        tmpstr = " And AcSlNo='" + objSVReceipt.CRSlNo + "'";
                    }
                    if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.CrVrNo, "MNPY") == false)   //For Allocation Vouchers in OPTR
                    {
                        objRPVoucherN.update_Cr("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.CrVrNo, tmpstr);
                    }
                    else
                    {
                        objRPVoucherN.update_Cr("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.PCODE, objSVReceipt.CrVrNo, tmpstr);
                    }
                }
            }
            if (CallType == 2)
            {
                ////*************UpPosting in Tr & Optr
                foreach (var objSVReceipt in VrItem)
                {
                    if (objSVReceipt.DrSlNo != "")
                    {
                        tmpstr = " And AcSlNo='" + objSVReceipt.DrSlNo + "'";
                    }
                    if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.DrVrNo, "MNRC") == false)
                    {
                        objRPVoucherN.update_Dr1("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.DrVrNo, tmpstr);
                    }
                    else
                    {
                        objRPVoucherN.update_Dr1("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.DrVrNo, tmpstr);
                    }
                    if (objSVReceipt.CRSlNo != "")
                    {
                        tmpstr = " And AcSlNo='" + objSVReceipt.CRSlNo + "'";
                    }
                    if (ChkOpTables(objSVReceipt.PCODE, objSVReceipt.CrVrNo, "MNPY") == false)
                    {
                        objRPVoucherN.update_Cr1("Tr", objSVReceipt.AdjAmt.ToString(), objSVReceipt.CrVrNo, tmpstr);
                    }
                    else
                    {
                        objRPVoucherN.update_Cr1("OPTR", objSVReceipt.AdjAmt.ToString(), objSVReceipt.CrVrNo, tmpstr);
                    }
                }
            }
        }
        public Boolean ChkOpTables(string PCODE, string DrVrNo, string FDName = "")
        {
            DataRow dr = objRPVoucherN.select_Op(DrVrNo);
            if (dr != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void DeleteAc()
        {
            objRPVoucherN.delete("AC", "ACVrNo", AcVrNo);
        }
        public void DeleteATR()
        {
            objRPVoucherN.delete("Tr", "AcVrNo", AcVrNo);
        }
        public void DeleteTR()
        {
            objRPVoucherN.delete("ATr", "AcVrNo", AcVrNo);
        }
        public void DeleteAl()
        {
            string str = trcode.ToUpper();
            if (str == "MNRC")
            {
                TOF = "D";
            }
            else
            {
                TOF = "C";
            }
            if (TOF == "C")
            {
                objRPVoucherN.delete("Al", "DrVrNo", AcVrNo);
            }
            else if (TOF == "D")
            {
                objRPVoucherN.delete("Al", "CrVrNo", AcVrNo);
            }
        }
        public string RtLedgBal(string PCode)
        {
            try
            {
                string LedgBal = ShowLDGBalance(PCode);
                if (LedgBal.Contains("CR"))
                {
                    LedgBal = LedgBal.Replace("CR", "");
                    LedgBal = (Convert.ToDouble(LedgBal) * (-1)).ToString();
                }
                else
                {
                    LedgBal = LedgBal.Replace("DR", "");
                }
                return LedgBal;
            }
            catch
            {

                return "";
            }
        }
        public string ShowLDGBalance(string Selparties)
        {
            try
            {
                string SelAmt = null;
                string AmtType = "";
                obj_stmGlobalMethods.val1 = Selparties;
                double TOpBal = 0;
                DataRow drLedg = obj_stmGlobalMethods.Select_TRrow("LgBal");
                if (drLedg != null)
                {
                    TOpBal = Convert.ToDouble(((drLedg["LgBal"] == null) ? "0" : Convert.ToDouble(drLedg["LgBal"]).ToString("#########0.00")));
                    SelAmt = CrDr(TOpBal, AmtType);
                    return SelAmt;
                }
                else
                {
                    return "0";
                }
            }
            catch
            {
                return "";
            }
        }
        public string CrDr(double Amount, string Type)
        {
            string TotAmt = null;
            try
            {
                if (Amount < 0)
                {
                    Type = " Cr";
                    Amount = Amount * (-1);
                }
                else
                {
                    Type = " Dr";
                }
                TotAmt = Amount.ToString("#########0.00");
                TotAmt = TotAmt + Type;
                return TotAmt;
            }
            catch
            {
                return "";
            }
        }

        public string RtOutStBal(string PCode, string Ptype = "")
        {
            try
            {
                DataRow dr = (new TIA3T.NEW.BLL.ClsAcpmst()).Select_OnPcode(PCode);
                if (dr != null)
                {
                    Ptype = (dr["Ptype"] == null) ? "" : dr["Ptype"].ToString();
                }
                string OutStBal = ShowOTSDrBalance(PCode, Ptype);
                if (OutStBal.Contains("CR"))
                {
                    OutStBal = OutStBal.Replace("CR", "");
                    OutStBal = (Convert.ToDouble(OutStBal) * (-1)).ToString();
                }
                else
                {
                    OutStBal = OutStBal.Replace("DR", "");
                }
                return OutStBal;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string ShowOTSDrBalance(string OTSSelparties, string Ptype)
        {
            try
            {
                if (Ptype == "C")
                {
                    string OTSDrSelAmt = null;
                    short i = 0;
                    string OTSDrAmtType = "";

                    DataRow DataRow = default(DataRow);

                    DataTable dt_ = obj_stmGlobalMethods.Select_TRTable("DAMT", OTSSelparties);
                    double OTSDrTOpBal = 0;
                    i = 2;
                    if (dt_.Rows.Count != 0)
                    {
                        for (int k = 0; k <= dt_.Rows.Count - 1; k++)
                        {
                            DataRow = dt_.Rows[k];
                            OTSDrTOpBal = (DataRow["DAMT"] == null) ? 0 : Convert.ToDouble(DataRow["DAMT"]);
                            OTSDrSelAmt = CrDr(OTSDrTOpBal, OTSDrAmtType);

                        }
                        dt_.Clear();
                        return OTSDrSelAmt;
                    }
                    else
                    {
                        return "";
                    }
                }
                else if (Ptype == "S")
                {
                    string OTSCrSelAmt = null;
                    string OTSCrAmtType = "";
                    DataRow DataRow = default(DataRow);
                    DataTable dt_ = obj_stmGlobalMethods.Select_TRTable("CAMT", OTSSelparties);
                    double OTSCrTOpBal = 0;

                    if (dt_.Rows.Count != 0)
                    {
                        for (int k = 0; k <= dt_.Rows.Count - 1; k++)
                        {
                            DataRow = dt_.Rows[k];
                            OTSCrTOpBal = (DataRow["CAMT"] == null) ? 0 : Convert.ToDouble(DataRow["CAMT"]);

                            OTSCrSelAmt = CrDr(OTSCrTOpBal, OTSCrAmtType);
                            return OTSCrSelAmt;
                        }
                        return OTSCrSelAmt;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public bool GetSMSManagement(string Fdname)
        {
            try
            {
                int Cnt = 0;
                DataRow drsuccess = TIA3T.BLL.clsGlobalMethodsBLL.Select_SMSSettingCnt(Fdname);
                if (drsuccess != null)
                {
                    Cnt = (drsuccess["Cnt"] == null) ? 0 : Convert.ToInt16(drsuccess["Cnt"]);
                    if (Cnt > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void UpdatePayee()
        {
            try
            {
                string Buyerid = null;
                DataRow drRcPy = objRcPy.Select_RL_Buyer(Pcode, RefNo);
                if (drRcPy != null)
                {
                    Buyerid = ((drRcPy["buyerid"] == null) ? "" : drRcPy["buyerid"].ToString());
                    objRcPy.Update_PAYEE_ConVrNo(AcVrNo, Buyerid, RefNo);
                }
            }
            catch (Exception ex)
            {

            }
        }


        public void UpdatePayer()
        {
            try
            {
                string Supplierid = null;
                DataRow drRcPy = objRcPy.Select_RL_Supplier(Pcode, RefNo);
                if (drRcPy != null)
                {
                    Supplierid = ((drRcPy["Supplierid"] == null) ? "" : drRcPy["Supplierid"].ToString());
                    objRcPy.Update_RL_ConVrNo(AcVrNo, Supplierid, RefNo);
                }
            }
            catch (Exception ex)
            {

            }
        }

    }
    public class PartyDataRec
    {
        public string doc { get; set; }
        public string docno { get; set; }
        public string accode { get; set; }
        public string codedesc { get; set; }
        public string Banker { get; set; }
        public string Branch { get; set; }
    }

    public class clsVrItem
    {
        public string PCODE { get; set; }
        public string DrVrNo { get; set; }
        public string DrSlNo { get; set; }
        public string CrVrNo { get; set; }
        public string CRSlNo { get; set; }
        public double AdjAmt { get; set; }
        public string VrDate { get; set; }
        public double BalAmt { get; set; }
        public double RemAmt { get; set; }
        public string chqName { get; set; }
        public string dt { get; set; }
    }

    public class MISReport
    {
        public string TRCODE { get; set; }
        public double? TotAmt { get; set; }
        public double? CurTot { get; set; }
        public double? CurCash { get; set; }
    }

    public class partyReceiptData
    {
        public string AcVrNo { get; set; }
        public string AcSlno { get; set; }
        public DateTime vrdate { get; set; }
        public string vrdate1 { get; set; }
        public decimal? amount { get; set; }
        public decimal? BalAmt { get; set; }
        public int? AdjAmt { get; set; }
        public decimal? RemAmt { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string DocDate1 { get; set; }
        public string narr1 { get; set; }
        public string narr2 { get; set; }
        public string instype { get; set; }
    }

    [Table("statemaster")]
    public class statemaster
    {
        [Key]
        public string StCode { get; set; }
        public string StName { get; set; }
    }
    [Table("citymaster")]
    public class citymaster
    {
        [Key]
        public string CtCode { get; set; }
        public string CtName { get; set; }
    }
    [Table("areamst")]
    public class areamst
    {
        [Key]
        public string aCode { get; set; }
        public string aName { get; set; }
        public double? Charges { get; set; }
    }

    public class Flag
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
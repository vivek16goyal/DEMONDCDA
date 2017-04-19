using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GBCService.Models
{
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
    
    }

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

    [Table("AppSMSStatus")]
    public class AppSMSStatus
    {
        [Key]
        public int id { get; set; }
        public string MobileNo { get; set; }
        public string Status { get; set; }
        public string Msg { get; set; }
    }

    [Table("PatientMaster")]
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

    [Table("OB")]
    public class OB
    {
        [Key]
        public string VrId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string VrNo { get; set; }
        public DateTime? VrDate { get; set; }
        public string ImgName { get; set; }
        public double? OrdAmt { get; set; }
        public double? DelCharge { get; set; }
        public string AppType { get; set; }
    }

    [Table("OI")]
    public class OI
    {
        [Key, Column(Order = 0)]
        public string VrId { get; set; }
        [Key, Column(Order = 1)]
        public string SlNo { get; set; }
        public string Icode { get; set; }
        public string IName { get; set; }
        public string Packing { get; set; }
        public double? OrdQty { get; set; }
        public double? FreeQty { get; set; }
        public double? Rate { get; set; }
        public double? Value { get; set; }
        public double? MRP { get; set; }        
    }

    public class Item
    {      
        public string Icode { get; set; }
        public string IName { get; set; }
        public string Packing { get; set; }
        public double? Qty { get; set; }
        public double? Free { get; set; }
        public double? Rate { get; set; }
        public double? WRate { get; set; }
        public double? Value { get; set; }
        public double? MRP { get; set; }
    }

    public class Order
    {
        public string VrId { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string VrNo { get; set; }
        public DateTime VrDate { get; set; }
        public string ImgName { get; set; }
        public double? OrdAmt { get; set; }
        public List<Item> oi { get; set; }
        public string series { get; set; }
        public string AppType { get; set; }
        public string DelCharges { get; set; }
    }
    public class Order1
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
        public string status { get; set; }
    }

    public class Global
    {


        public string  SaveOrder(Order Ord, string FDNAME)
        {

            GBCCon gbc_con = new GBCCon();
            Global objGlobal = new Global();
            string name = gbc_con.Database.Connection.Database;
            //Get VrNo
            string vrno = GenrateVrNo(FDNAME, name + ".dbo.OB", (string.IsNullOrEmpty(Ord.series) ? "GC" : Ord.series), DateTime.Now.Date, "VRID");

            double amt = 0.0;
            OB obj_ob = gbc_con.db_ob.SingleOrDefault(p => p.VrId == vrno);
            obj_ob.FromUserId = Ord.FromUserId;
            obj_ob.ToUserId = Ord.ToUserId;
            obj_ob.VrDate = DateTime.Now.Date;
            obj_ob.VrId = vrno;
            obj_ob.VrNo = Ord.VrNo;
            obj_ob.ImgName = Ord.ImgName;
            obj_ob.AppType = Ord.AppType;
            int i = 1;
            string isWQty = "0";
            try
            {
                ServiceReg obj = gbc_con.db_SerReg.SingleOrDefault(p => p.Pcode == Ord.ToUserId);
                if (obj != null)
                {
                    isWQty = obj.pretailordQty;
                }
            }
            catch
            {
                isWQty = "0";
            }
            foreach (var item in Ord.oi)
            {
                string slno = "00" + i;
                i = i + 1;
                OI obj_oi = new OI();
                obj_oi.Icode = item.Icode;
                obj_oi.IName = item.IName;
                obj_oi.MRP = item.MRP;
                obj_oi.OrdQty = item.Qty;
                obj_oi.FreeQty = item.Free;
                obj_oi.Packing = item.Packing;                
                obj_oi.SlNo = slno.Substring(slno.Length - 3, 3);
                if (isWQty == "1")
                {
                    obj_oi.Rate = item.WRate;
                    obj_oi.Value = obj_oi.Rate * obj_oi.OrdQty;                    
                }
                else
                {
                    obj_oi.Rate = item.Rate;
                    obj_oi.Value = obj_oi.Rate * obj_oi.OrdQty;
                }
                obj_oi.VrId = vrno;
                amt = Convert.ToDouble(amt + obj_oi.Value);
                gbc_con.db_oi.Add(obj_oi);
            }
            amt = amt + Convert.ToDouble(Ord.DelCharges);
            //obj_ob.OrdAmt = amt;
            obj_ob.OrdAmt = Ord.OrdAmt;
            obj_ob.DelCharge = Convert.ToDouble(Ord.DelCharges);
            gbc_con.SaveChanges();
            return vrno;
        }
       

        public string GenrateVrNo(string cFdName, string cTbName, string VrSeries, DateTime VrDate, string FldVrNo = "")
        {
            SqlCommand cmdForNewVrnoN = new SqlCommand();
            SqlConnection con = new SqlConnection();
            string DMLForNewVrno = "";

            if (cFdName.Trim() == "" || VrSeries.Trim() == "")
            {
                return "";
            }
            GBCCon GbcCon = new GBCCon();
            string name = GbcCon.Database.Connection.Database;
            con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["GBCCon"].ToString();
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
            cmdForNewVrnoN.Parameters["@VrLength"].Value = System.Configuration.ConfigurationManager.AppSettings["VrLength"];
            cmdForNewVrnoN.Parameters["@VrDate"].Value = VrDate;
            cmdForNewVrnoN.Parameters["@FldVrNo"].Value = FldVrNo;
            cmdForNewVrnoN.Parameters["@DbForVc "].Value = name + ".dbo.VC";
            cmdForNewVrnoN.Parameters["@AcYr"].Value = ACYR();
            cmdForNewVrnoN.ExecuteNonQuery();
            DMLForNewVrno = cmdForNewVrnoN.Parameters["@VrNoNew2"].Value.ToString();
            DMLForNewVrno = DMLForNewVrno.Trim();

            cmdForNewVrnoN.Dispose();

            con.Close();

            return DMLForNewVrno;
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

    }
    public class ItemMaster
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
        public string free { get; set; }
        public string Qty { get; set; }
        public string GNAme { get; set; }
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
    [Table("AreaMaster")]
    public class areamst
    {
        [Key]
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
    }
    

}

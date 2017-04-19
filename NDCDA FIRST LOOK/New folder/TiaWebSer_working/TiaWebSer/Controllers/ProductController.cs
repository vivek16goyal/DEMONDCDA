using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TiaWebSer.Models;
using System.Threading.Tasks;
using System.IO;
using System.Web.Hosting;
using System.Drawing;
using System.Security.AccessControl;
using System.Security.Principal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using TIA3T.BLL;
using TIA3T.DAL;
using TIA3T.NEW.BLL.Modules;

namespace TiaWebSer.Controllers
{
    public class ProductController : ApiController
    {

        GBCDbConn gbc_con = new GBCDbConn();
        DbConn con = new DbConn();
        clsGlobalMethodsBLL objGlb = new clsGlobalMethodsBLL();
        clsRecieptPayment objRcPy = new clsRecieptPayment();
        stmGlobalMethods objGblMthd = new stmGlobalMethods();
        Global objGlobal = new Global();
        public enum SWFormMode
        {
            None = 0,
            AddRecord = 1,
            EditRecord = 2
        }
        public SWFormMode formmode;
        ClsPosting objPostIn = new ClsPosting();
        TIA3T.NEW.BLL.GlobalVariableBLL objGblVrbBll = new TIA3T.NEW.BLL.GlobalVariableBLL();
        public static string constr = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
        public static string constrGBC = ConfigurationManager.ConnectionStrings["GBCDbConn"].ConnectionString;
        SqlConnection sqlcon = new SqlConnection(constr);
        TIA3T.NEW.BLL.Classes.clsRPVoucher objRPVoucherN = new TIA3T.NEW.BLL.Classes.clsRPVoucher();
        TIA3T.NEW.BLL.Modules.stmGlobalMethods obj_stmGlobalMethods = new TIA3T.NEW.BLL.Modules.stmGlobalMethods();//objacpmst
        TIA3T.NEW.BLL.ClsAcpmst objacpmst = new TIA3T.NEW.BLL.ClsAcpmst();
        
        SaveRecieptInvoice objSVReceipt1 = new SaveRecieptInvoice();

        [HttpGet]
        public HttpResponseMessage GetItem(string Iname, string FdName)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    GBCDbConn GbcCon = new GBCDbConn();
                    string name = GbcCon.Database.Connection.Database + ".dbo.";
                    List<ItemMaster> list_item;
                    List<ItemMasterWsale> list_itemWsale;
                    if (FdName == "WSAL" || FdName == "QTNW")
                    {
                        list_itemWsale = con.Database.SqlQuery<ItemMasterWsale>("Declare @Error Varchar(100)  EXEC pr_GetItems_Z_Wsal @ColumnName = {0},  @SearchText = {1}, @Error      = @Error output ,   @GBCDBName      = {2}, @debug      ={3} , @Fdname={4} , @PTaxPackingPI={5} , @GdnCode ={6},@HideItms={7} ", "Iname", "" + Iname + "", name, 0, "", "False", "","False").Take(10).ToList();
                        foreach (var item in list_itemWsale)
                        {
                            item.stk = item.WStk;
                            item.GNAme = getGnName(item.ICODE);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, list_itemWsale);
                     }
                    else
                    {
                        list_item = con.Database.SqlQuery<ItemMaster>("Declare @Error Varchar(100)  EXEC pr_GetItems_Z @ColumnName = {0},  @SearchText = {1}, @Error      = @Error output ,   @GBCDBName      = {2}, @debug      ={3} , @Fdname={4} , @PTaxPackingPI={5} , @GdnCode ={6}, @HideItms={7}", "Iname", "" + Iname + "", name, 0, "", "False", "", "False").Take(10).ToList();
                        foreach (var item in list_item)
                        {
                            item.GNAme = getGnName(item.ICODE);
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, list_item);
                    }
                    
                }
                catch (Exception ex)
                {
                    List<ItemMaster> list_item = new List<ItemMaster>();
                    ItemMaster im_ = new ItemMaster();
                    im_.INAME = "$" + ex.Message;
                    list_item.Add(im_);
                    return Request.CreateResponse(HttpStatusCode.OK, list_item);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }

        [HttpGet]
        public IEnumerable<ItemMasterWsale> GetItemConsumer(string Iname, string PCODE)
        {
            try
            {
                Global objGbl = new Global();
                GBCDbConn GbcCon = new GBCDbConn();
                string name = GbcCon.Database.Connection.Database + ".dbo.";
                List<ItemMasterWsale> list_itemWsale;
                list_itemWsale = con.Database.SqlQuery<ItemMasterWsale>("Declare @Error Varchar(100)  EXEC pr_GetItems_Z_Wsal @ColumnName = {0},  @SearchText = {1}, @Error      = @Error output ,   @GBCDBName      = {2}, @debug      ={3} , @Fdname={4} , @PTaxPackingPI={5} , @GdnCode ={6} ", "Iname", "" + Iname + "", name, 0, "", "False", "").Take(10).ToList();
                foreach (var item in list_itemWsale)
                {
                    item.GNAme = getGnName(item.ICODE);
                    if (item.Mrp.Trim() == "0.00")
                    {
                        string sql = "select lastMRP from st where icode='" + item.ICODE + "'";
                        DataRow dr = objGbl.getDataRow(true, sql);
                        if (dr != null)
                        {
                            item.Mrp = (dr["lastMRP"] == null) ? "0.00" : (dr["lastMRP"].ToString().Trim() == "") ? "0.00" : dr["lastMRP"].ToString();
                        }
                        
                    }
                    string rate = GetRateCon(item.ICODE, Convert.ToDouble(item.Mrp), PCODE, item.pursize);
                    item.Rate = rate;
                    item.WRate = (Convert.ToDouble(rate) * Convert.ToDouble(item.pursize)).ToString();
                }
                return list_itemWsale;


                //List<ItemMaster> list_item = con.Database.SqlQuery<ItemMaster>("Declare @Error Varchar(100)  EXEC pr_GetItems_Z @ColumnName = {0},  @SearchText = {1}, @Error      = @Error output ,   @GBCDBName      = {2}, @debug      ={3} , @Fdname={4} , @PTaxPackingPI={5} , @GdnCode ={6} ", "Iname", "" + Iname + "%", name, 0, "", "False", "").Take(10).ToList();
                //foreach (var item in list_item)
                //{
                //    item.GNAme = getGnName(item.ICODE);
                //    string rate=GetRateCon(item.ICODE, Convert.ToDouble(item.Mrp), PCODE, item.pursize);
                //    item.Rate = (Convert.ToDouble(rate) * Convert.ToDouble(item.pursize)).ToString();
                //}
                //return list_item;
            }
            catch (Exception ex)
            {
                List<ItemMasterWsale> list_item = new List<ItemMasterWsale>();
                ItemMasterWsale im_ = new ItemMasterWsale();
                im_.INAME = "$" + ex.Message;
                list_item.Add(im_);
                return list_item;
            }
        }

        public string  getGnName(string Icode)
        {
            Global obj = new Global();
            string SelICode = "";
            DataRow dr =null;
            string GenericName = "";
            try
            {
                SelICode = Icode;
                SelICode = SelICode.Replace("'", "''");
                GBCDbConn GbcCon = new GBCDbConn();
                string name = GbcCon.Database.Connection.Database + ".dbo.";
                DataTable dt = obj.getDataTable(false, "SELECT GM.GNAME FROM( SELECT GCODE FROM " + name + "IM IM WHERE ICODE='" + SelICode + "') AS T LEFT JOIN " + name + "GM GM ON T.GCODE=GM.GCODE");
                if (dt.Rows.Count != 0)
                {
                    dr = dt.Rows[0];
                }
                //31/12/2015
                if (dr != null)
                {
                    GenericName = (dr.IsNull("GNAME")) ? "" : dr["GNAME"].ToString();
                }
                else
                {
                    GenericName = "";
                }
                return GenericName;
            }
            catch 
            {
                return "";
            }
        }

        public string GetRateCon(string icode, double MRP, string Pcode, string pursz)
        {
            try
            {
                //selecting rate type
                string Salerate = "";
                pursz = (String.IsNullOrEmpty(pursz)) ? "1" : pursz;
                if (!String.IsNullOrEmpty(Pcode))
                {
                    try
                    {
                        Salerate = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == Pcode).salerate;
                    }
                    catch
                    {
                    }
                }
                if (String.IsNullOrEmpty(Salerate))
                {
                    string Ser, FDNAME = "ORDR";
                    VC objVC = gbc_con.Db_vc.SingleOrDefault(p => p.FDNAME == FDNAME);
                    if (objVC == null)
                    {
                        VC obj = new VC();
                        obj.FDNAME = FDNAME;
                        obj.SER = "O2";
                        gbc_con.Db_vc.Add(obj);
                        gbc_con.SaveChanges();
                        Ser = "O2";
                    }
                    else
                    {
                        Ser = objVC.SER;
                    }
                    VC obj_vc = gbc_con.Db_vc.SingleOrDefault(p => p.FDNAME == FDNAME && p.SER == Ser);
                    if (obj_vc != null)
                    {
                        if (!String.IsNullOrEmpty(obj_vc.SerRate))
                        {
                            Salerate = obj_vc.SerRate;
                        }
                    }
                    Salerate = "SP";
                }
                string rate;
                RM obj_rm = gbc_con.Db_rm.SingleOrDefault(p => p.ICODE == icode && p.MRP == MRP);
                if (obj_rm != null)
                {
                    switch (Salerate)
                    {
                        case "RP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.MRP) / Convert.ToDouble(pursz));
                            break;
                        case "PR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.PURCPRICE) / Convert.ToDouble(pursz));
                            break;
                        case "SP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.SALEPRICE));
                            break;
                        case "LR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.LOCALPRICE));
                            break;
                        case "OR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.OUTSTPRICE));
                            break;
                        case "TR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.TRADEPRICE));
                            break;
                        case "CP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.COSTPRICE));
                            break;
                        case "AD":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.ADPRICE));
                            break;
                        case "CR":
                            rate = string.Format("{0:0.0000}", gbc_con.Db_Cr.Where(p => p.ICODE == icode && p.MRP == MRP).Take(1).SingleOrDefault(p=>p.ICODE!="").COSTRATE);
                            break;
                        case "PT":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.PURCPRICE) + Convert.ToDouble(obj_rm.VAT));
                            break;
                        case "CW":
                            CR o_cr = gbc_con.Db_Cr.Where(p => p.ICODE == icode && p.MRP == MRP).Take(1).SingleOrDefault(p => p.ICODE != "");
                            rate = string.Format("{0:0.0000}", o_cr.COSTRATE - ((o_cr.MSTAMT == null) ? 0 : Convert.ToDouble(o_cr.MSTAMT)));
                            break;
                        default:
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.SALEPRICE));
                            break;
                    }
                }
                else { rate = "0.0000"; }


                return rate;
            }
            catch
            {
                return "0.00";
            }
        }

        public List<Batch> GetBatch(string ICode, string vrno = "", string slno = "", string FdName = "")
        {
            GBCDbConn GbcCon = new GBCDbConn();
            string name = GbcCon.Database.Connection.Database + ".dbo.";
            if (vrno == null || slno == null)
            {
                vrno = ""; slno = "";
            }
            if (slno != "")
            {
                string str = "00" + slno;
                slno = str.Substring(str.Length - 3, 3);
            }

            List<Batch> list_Batch;
            if (FdName == "WSAL" || FdName=="QTNW")
            {
                list_Batch = con.Database.SqlQuery<Batch>("Declare @Error Varchar(100)  EXEC pr_GetStock_Wsal @ColumnName = {0},  @SearchText = {1}, @ICode      = {2} ,    @GdnCode      = {3},@TmpVrno      ={4} ,@CurRowSlno={5} ,@Error      = @Error output, @GBCDBName={6} , @IsNegativeStock ={7} ,@FetchZeroStock={8},@debug={9},@PTaxPackingPI={10},@pStBatch={11}", "Stock", "", "" + ICode + "", "", vrno, slno, name, "True", "False", 0, "False", "False").Take(10).ToList();
                foreach (var item in list_Batch)
                {
                    item.stock = item.Wstock;
                }
            }
            else
            {
                list_Batch = con.Database.SqlQuery<Batch>("Declare @Error Varchar(100)  EXEC pr_GetStock @ColumnName = {0},  @SearchText = {1}, @ICode      = {2} ,    @GdnCode      = {3},@TmpVrno      ={4} ,@CurRowSlno={5} ,@Error      = @Error output, @GBCDBName={6} , @IsNegativeStock ={7} ,@FetchZeroStock={8},@debug={9},@PTaxPackingPI={10},@pStBatch={11}", "Stock", "", "" + ICode + "", "", vrno, slno, name, "True", "False", 0, "False", "False").Take(10).ToList();
            }
            IM obj_im = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == ICode);
            string packing = "";
            if (obj_im != null)
            {
                packing = obj_im.packing;
            }

            double totstk = list_Batch.Sum(p => Convert.ToDouble(p.stock));
            if (list_Batch.Count != 0)
            {
                foreach (var item in list_Batch)
                {
                    item.packing = packing;
                    item.Expirydate = (item.Expiry == null ? "" : Convert.ToDateTime(item.Expiry).ToString("dd/MM/yyyy"));
                    item.TotStk = totstk.ToString();
                    item.Vrdate_ = (item.Vrdate == null ? "" : Convert.ToDateTime(item.Vrdate).ToString("dd/MM/yyyy"));
                }
                //return list_Batch.OrderByDescending(p => p.Expiry).ToList();
                return list_Batch;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public string GetBatchTotStk(string ICode, string vrno = "", string slno = "", string FdName = "")
        {
            List<Batch> batch_List;
            batch_List = GetBatch(ICode, vrno, slno, FdName);
            if (batch_List != null)
            {
                return batch_List.Take(1).SingleOrDefault(p => p.BatchNo != "").TotStk;
            }
            else
            {
                return "0";
            }
        }

        [HttpGet]
        public string GetItemRate(string ICode, string MRP, string Pcode, string FDName, string Series, string PurSz, string vrno, string slno)
        {
            string Salerate = "";
            PurSz = (String.IsNullOrEmpty(PurSz)) ? "1" : PurSz;
            if (!String.IsNullOrEmpty(Pcode))
            {
                Salerate = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == Pcode).salerate;
            }
            return getRate(ICode, Convert.ToDouble(MRP), Salerate, FDName, Series, PurSz, vrno, slno);
        }

        public string getRate(string Icode, double MRP, string Salerate, string FDName, string Series, string PurSz, string vrno, string slno)
        {
            string rate;
            //return "0.0000";
            try
            {
                if (String.IsNullOrEmpty(Salerate))
                {
                    VC obj_vc = gbc_con.Db_vc.SingleOrDefault(p => p.FDNAME == FDName && p.SER == Series);
                    if (obj_vc != null)
                    {
                        if (!String.IsNullOrEmpty(obj_vc.SerRate))
                        {
                            Salerate = obj_vc.SerRate;
                        }
                    }
                    Salerate = (FDName == "SALE" || FDName == "QTTN") ? "SP" : "LR";
                }

                RM obj_rm = gbc_con.Db_rm.SingleOrDefault(p => p.ICODE == Icode && p.MRP == MRP);
                if (obj_rm != null)
                {
                    switch (Salerate)
                    {
                        case "RP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.MRP) / Convert.ToDouble(PurSz));
                            break;
                        case "PR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.PURCPRICE) / Convert.ToDouble(PurSz));
                            break;
                        case "SP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.SALEPRICE));
                            break;
                        case "LR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.LOCALPRICE));
                            break;
                        case "OR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.OUTSTPRICE));
                            break;
                        case "TR":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.TRADEPRICE));
                            break;
                        case "CP":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.COSTPRICE));
                            break;
                        case "AD":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.ADPRICE));
                            break;
                        case "CR":
                            rate = string.Format("{0:0.0000}", gbc_con.Db_Cr.SingleOrDefault(p => p.VRNO == vrno && p.SLNO == slno).COSTRATE);
                            break;
                        case "PT":
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.PURCPRICE) + Convert.ToDouble(obj_rm.VAT));
                            break;
                        case "CW":
                            CR o_cr = gbc_con.Db_Cr.SingleOrDefault(p => p.VRNO == vrno && p.SLNO == slno);
                            rate = string.Format("{0:0.0000}", o_cr.COSTRATE - ((o_cr.MSTAMT == null) ? 0 : Convert.ToDouble(o_cr.MSTAMT)));
                            break;
                        default:
                            rate = string.Format("{0:0.0000}", Convert.ToDouble(obj_rm.LOCALPRICE));
                            break;
                    }
                }
                else { rate = "0.0000"; }


                if (rate == "0.0000" || rate == "")
                {
                    CR obj_cr = gbc_con.Db_Cr.SingleOrDefault(p => p.VRNO == vrno && p.SLNO == slno);

                    // List<CR> obj_cr = gbc_con.Db_Cr.Where(p => p.ICODE == Icode && (p.TRCODE == "PURC" || p.TRCODE == "DMIW")).OrderByDescending(p => p.VRDATE).ToList();
                    if (obj_cr != null)
                    {
                        rate = string.Format("{0:0.0000}", obj_cr.RATE);
                    }
                    else
                    {
                        rate = "0.0000";
                    }
                }
                if (rate == null || rate == "")
                {
                    rate = "0.0000";
                }
                if (FDName == "WSAL")
                {
                    return string.Format("{0:0.0000}", Convert.ToDouble(rate) * Convert.ToDouble(PurSz));
                }
                else
                {
                    return rate;
                }
            }
            catch
            {
                return "0.0000";
            }
        }
        [HttpGet]
        public HttpResponseMessage GetPt(string Iname, string Pcode)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    if (Pcode == null || Pcode.ToUpper() == "ZZZZZZ")
                    {
                        Pcode = "";
                    }
                    string dbname = gbc_con.Database.Connection.Database;
                    DbConn Con = new DbConn();
                    List<AutoList> list_pt = con.Database.SqlQuery<AutoList>("EXEC GetAutoList_APP @name = {0},@Pcode={1} , @flag = {2}, @GBCName={3}", "" + Iname + "", Pcode, "P", dbname).Take(10).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, list_pt);
                }
                catch (Exception ex)
                {
                    List<AutoList> list_pt = new List<AutoList>();
                    AutoList im_ = new AutoList();
                    im_.Desr = "$" + ex.Message;
                    list_pt.Add(im_);
                    return Request.CreateResponse(HttpStatusCode.OK, list_pt);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }

        public string GetPtInfo(string Pcode)
        {
            IEnumerable<AutoList> a = (IEnumerable<AutoList>) GetPt("", Pcode);
            string str = "";
            foreach (var item in a)
            {
                if (!item.Desr.StartsWith("$"))
                {
                    str = item.Desr + "|" + item.Address + "|" + item.Phone + "|" + item.Code;
                    break;
                }
            }
            return str;
        }

        public string GetPartyInfo(string PtCode)
        {
            string str = "";
            Global obj_gbl = new Global();
            DataRow dr = obj_gbl.getDataRow(false, "select  company from patientmaster where ptcode='" + PtCode + "'");
            if (dr != null)
            {
                string pcode = (dr["company"] == null) ? "" : dr["company"].ToString();
                if (pcode != "")
                {
                    DataRow dr1 = obj_gbl.getDataRow(true, "select pcode,pname from acpmst where pcode='" + pcode + "'");
                    if (dr1 != null)
                    {
                        OrderController ord = new OrderController();
                        str = dr1["pcode"].ToString() + "|" + dr1["pname"].ToString() + "|" + ord.Getldgbal(pcode); ;
                    }
                }
            }
            return str;
        }

        [HttpGet]
        public HttpResponseMessage GetDr(string Iname)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    DbConn Con = new DbConn();
                    List<AutoList> list_pt = con.Database.SqlQuery<AutoList>("EXEC GetAutoList_APP @name = {0},  @flag = {1}", "" + Iname + "", "D").Take(10).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, list_pt);
                }
                catch (Exception ex)
                {
                    List<AutoList> list_pt = new List<AutoList>();
                    AutoList im_ = new AutoList();
                    im_.Name = "$" + ex.Message;
                    list_pt.Add(im_);
                    return Request.CreateResponse(HttpStatusCode.OK, list_pt);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }

        [HttpGet]
        public string GetTmpSIVrNo(string FDName, string Ser, string LogUserId)
        {
            return GetTmpVrNo(FDName, Ser, LogUserId);
        }

        public string GetLessPerc(string FDName, string Ser, string Pcode)
        {
            double lessPerc, lessPercParty, lessPercVC;
            ACPMST obj_ac = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == Pcode);
            lessPercParty = (obj_ac == null) ? 0 : (obj_ac.discount == null) ? 0 : Convert.ToDouble(obj_ac.discount);
            VC obj_vc = gbc_con.Db_vc.SingleOrDefault(p => p.SER == Ser && p.FDNAME == FDName);
            lessPercVC = (obj_vc == null) ? 0 : (obj_vc.SerPerc == null) ? 0 : Convert.ToDouble(obj_vc.SerPerc);
            lessPerc = (lessPercParty < lessPercVC) ? (lessPercParty == 0) ? lessPercVC : lessPercParty : (lessPercVC == 0) ? lessPercParty : lessPercVC;
            return lessPerc.ToString("##############0.00");
        }

        public Bill GetSaleInfo(string tempvrno, string FDName, string Ser, string Pcode)
        {
            Bill obj_Bill = new Bill();
            Global obj = new Global();
            DataTable dt = obj.getDataTable(true, "select count(*) cnt, sum(value) gross ,sum(mstamt) mstamt from tmpsi where vrno='" + tempvrno + "' group by vrno ");
            if (dt.Rows.Count != 0)
            {
                double grs, net, less, mst;
                grs = Convert.ToDouble(dt.Rows[0]["gross"]);
                less = Convert.ToDouble(GetLessPerc(FDName, Ser, Pcode));
                mst = Convert.ToDouble(dt.Rows[0]["mstamt"]);
                net = grs - ((grs * less) / 100) + mst;
                obj_Bill.GrossAmt = grs.ToString("##############0.00");
                obj_Bill.LessPerc = less.ToString();
                obj_Bill.Less = ((grs * less) / 100).ToString("##############0.00");
                obj_Bill.NetAmt = net.ToString("##############0.00");
                obj_Bill.cnt = dt.Rows[0]["cnt"].ToString();
                obj_Bill.MstAmt = dt.Rows[0]["MstAmt"].ToString();
            }
            return obj_Bill;
        }
        [HttpGet]
        public string setGrossNet(string tempvrno, string FDName, string Ser, string Pcode)
        {
            Bill obj_Bill = GetSaleInfo(tempvrno, FDName, Ser, Pcode);
            if (obj_Bill != null)
            {
                return obj_Bill.GrossAmt + "|" + objGblMthd.RoundOffAmount(Global.PRoundOff, Global.CEILINGROUNDOFF, Convert.ToDouble(obj_Bill.NetAmt));
            }
            else
            {
                return "";
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTmpSIData(string FDName, string Ser, string LogUserId, string Index, string Icode, string batchNo, string MRP, string Rate, string Qty, string Free, string expiry, string ConVrNo, string ConSlNo, string PurSz, string Packing, string TmpVrNO, string Pcode)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    Global obj_gbl = new Global();
                    if (TmpVrNO == "" || TmpVrNO == null)
                    {
                        TmpVrNO = GetTmpVrNo(FDName, Ser, LogUserId);
                    }

                    Ser = Ser.ToUpper();
                    TmpSI objTmpSI = new TmpSI();
                    objTmpSI.VRNO = TmpVrNO;
                    string str = "00" + Index;
                    objTmpSI.SLNO = str.Substring(str.Length - 3, 3);
                    objTmpSI.ICODE = Icode;
                    objTmpSI.BATCHNO = batchNo;
                    objTmpSI.MRP = Convert.ToDouble(MRP);

                    if (FDName == "WSAL" || FDName =="QTNW")
                    {
                        try
                        {
                            objTmpSI.WRATE = Convert.ToDouble(Rate);
                        }
                        catch
                        {
                            objTmpSI.WRATE = 0;
                        }
                        objTmpSI.QTYISSUED = Convert.ToDouble(Qty) * Convert.ToDouble(PurSz);
                        objTmpSI.FREEISSUED = Convert.ToDouble(Free) * Convert.ToDouble(PurSz);
                        objTmpSI.WQTYISSUED = Convert.ToDouble(Qty);
                        objTmpSI.WFREEISSUED = Convert.ToDouble(Free);
                        IM objIm = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == Icode);
                        if (objIm == null)
                        {
                            objTmpSI.MST = 0;
                        }
                        else
                        {
                            objTmpSI.MST = Convert.ToDouble((objIm.MSTWSALE == null ? 0 : objIm.MSTWSALE));
                            objTmpSI.MSTFLAG = objIm.MSTWSFLAG;
                        }
                        objTmpSI.RATE = Convert.ToDouble(Rate) / Convert.ToDouble(PurSz);
                    }
                    else
                    {
                        try
                        {
                            objTmpSI.RATE = Convert.ToDouble(Rate);
                        }
                        catch
                        {
                            objTmpSI.RATE = 0;
                        }
                        objTmpSI.QTYISSUED = Convert.ToDouble(Qty);
                        objTmpSI.FREEISSUED = Convert.ToDouble(Free);
                        IM objIm = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == Icode);
                        if (objIm == null)
                        {
                            objTmpSI.MST = 0;
                        }
                        else
                        {
                            objTmpSI.MST = Convert.ToDouble(objIm.MSTSALE == null ? 0 : objIm.MSTSALE);
                            objTmpSI.MSTFLAG = objIm.MSTSFLAG;
                        }
                    }
                    try
                    {
                        objTmpSI.VALUE = Convert.ToDouble(Qty) * Convert.ToDouble(Rate);
                    }
                    catch
                    {
                        objTmpSI.VALUE = 0;
                    }
                    if (expiry == "" || expiry == null)
                    {

                    }
                    else
                    {
                        try
                        {
                            objTmpSI.EXPIRY = DateTime.ParseExact(expiry, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            try
                            {
                                objTmpSI.EXPIRY = DateTime.ParseExact(obj_gbl.ChangeDate(expiry), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                            }
                            catch
                            {

                            }
                        }
                    }
                    objTmpSI.CONVRNO = ConVrNo;
                    objTmpSI.CONSLNO = ConSlNo;
                    objTmpSI.Packing = Packing;
                    objTmpSI.PURSIZE = Convert.ToDouble(PurSz);
                    string LessPerc = GetLessPerc(FDName, Ser, Pcode);
                    objTmpSI.MSTVAL = objTmpSI.VALUE - ((objTmpSI.VALUE * Convert.ToDouble(LessPerc)) / 100);
                    objTmpSI.MSTAMT = Convert.ToDouble(((objTmpSI.MSTVAL * objTmpSI.MST) / 100).ToString("##############0.00"));

                    CR obj_cr = gbc_con.Db_Cr.SingleOrDefault(p => p.VRNO == ConVrNo && p.SLNO == ConSlNo && p.ICODE == Icode);
                    objTmpSI.PRATE = (obj_cr == null) ? 0 : Convert.ToDouble(obj_cr.COSTRATE);

                    objTmpSI.SaveDataTMPSI(objTmpSI);
                    return Request.CreateResponse(HttpStatusCode.OK, objTmpSI.VRNO);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "$" + ex.Message);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }


        public string GetTmpVrNo(string FDName, string Ser, string LogUserId)
        {
            TmpSI objTmpSI = new TmpSI();
            return objTmpSI.GetTMPTBVrno(FDName, Ser, "0", "", "App", LogUserId);
        }

        [HttpGet]
        public HttpResponseMessage SaveInvoice(string TempVrNo, string PCODE, string VrSeries, string TRCODE, string UserId, string PatientID, string NameP, string Addr, string DCode, string DrName, string DrAddr, string isPrint, string Device="")
        {
            if ((new Global()).CheckStarterService())
            {
                Order obj_ord = new Order();
                if (GlobalVariablesBLL.GBCDBName != "" && GlobalVariablesBLL.GBCDBName != null)
                {                    
                    string Vrno = "";
                    string VRTYPE;
                    try
                    {
                        VRTYPE = gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == PCODE).type;
                    }
                    catch
                    {
                        VRTYPE = "";
                    }
                    if (VRTYPE == "" || VRTYPE == null)
                    {
                        VRTYPE = "C";
                    }
                    try
                    {
                        RecalculateValue(TempVrNo, PCODE, TRCODE, VrSeries);
                        Bill obj_bill = GetSaleInfo(TempVrNo, TRCODE, VrSeries, PCODE);
                        Global obj_gbl = new Global();
                        SaveInvoice obj_invcSave = new SaveInvoice();
                        obj_invcSave.TempVrNo = TempVrNo;
                        obj_invcSave.VrSeries = VrSeries;
                        obj_invcSave.VRNO = "";
                        obj_invcSave.VRDATE = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        obj_invcSave.PCODE = PCODE;
                        obj_invcSave.VRTYPE = VRTYPE;
                        obj_invcSave.TRCODE = TRCODE;
                        obj_invcSave.GROSS = Convert.ToDouble(obj_bill.GrossAmt);
                        obj_invcSave.LESSPERC = Convert.ToDouble(obj_bill.LessPerc);
                        obj_invcSave.LESS = Convert.ToDouble(obj_bill.LessPerc);
                        obj_invcSave.DISCOUNT = Convert.ToDouble(obj_bill.LessPerc);
                        obj_invcSave.MstAmt = Convert.ToDouble(obj_bill.MstAmt);
                        obj_invcSave.MST = Convert.ToDouble(obj_bill.MstAmt);
                        obj_invcSave.DrAmt = Convert.ToDouble(obj_bill.NetAmt);
                        double RAmount = objGblMthd.RoundOffAmount(Global.PRoundOff, Global.CEILINGROUNDOFF, Convert.ToDouble(obj_bill.NetAmt));
                        obj_invcSave.NETAMT = RAmount;
                        obj_invcSave.ROUNDOFF = RAmount - obj_invcSave.DrAmt;
                        obj_invcSave.UserId = UserId;
                        obj_invcSave.USRID = UserId;
                        obj_invcSave.DCode = UserId;
                        if (TRCODE == "SALE" || TRCODE == "QTTN")
                        {
                            if ((PatientID == "" || PatientID == null) && (NameP == "" || NameP == null))
                            {
                                DataRow dr = SelectRandomPt();
                                if (dr != null)
                                {
                                    PatientID = dr["PtCode"].ToString();
                                    NameP = dr["PtName"].ToString();
                                }
                            }
                            obj_invcSave.PatientID = (PatientID == "" || PatientID == null) ? "000000" : PatientID;
                            obj_invcSave.NameP = NameP;
                            obj_invcSave.Addr = Addr;

                            if (DrName == "" || DrName == null)
                            {
                                DataRow dr = SelectRandomDr();
                                if (dr != null)
                                {
                                    DCode = dr["Hrcode"].ToString();
                                    DrName = dr["Name"].ToString();
                                }
                            }

                            obj_invcSave.DCode = (DrName == "" || DrName == null) ? "000000" : DCode;
                            if (obj_invcSave.DCode == "000000")
                            {
                                HrMaster obj_hr = con.Db_hr.SingleOrDefault(p => p.HrCode == obj_invcSave.DCode);
                                if (obj_hr != null)
                                {
                                    DrName = obj_hr.Name;
                                    DrAddr = obj_hr.address;
                                }
                            }
                            obj_invcSave.DrName = DrName;
                            obj_invcSave.DrAddr = DrAddr;
                        }
                        else
                        {
                            obj_invcSave.DrName = DrName;
                        }

                        obj_invcSave.SBNARR2 = "TiaERP@App Bill.";
                        obj_invcSave.NodeId = Device;
                        obj_invcSave.VrTime = System.DateTime.Now.ToString("HH:MM:ss");
                        Vrno = obj_gbl.SaveInvoiceBill(obj_invcSave, isPrint);

                        obj_ord.vrno = Vrno;
                        obj_ord.pcode = (TRCODE == "SALE") ? ((NameP == null) ? "" : NameP) : gbc_con.Db_acpmst.SingleOrDefault(p => p.PCODE == PCODE).PNAME;
                        obj_ord.TotalAmt = Convert.ToDouble(obj_invcSave.NETAMT).ToString("##############0.00");
                        DataTable dt = obj_gbl.getDataTable(true, "select * from si where vrno='" + Vrno + "'");
                        List<ItemMaster> list = new List<ItemMaster>();
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            ItemMaster item1 = new ItemMaster();
                            string icode = dt.Rows[i]["Icode"].ToString();
                            item1.INAME = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == icode).INAME;  //dt.Rows[i]["Icode"].ToString();
                            if (TRCODE == "SALE" || TRCODE == "QTTN")
                            {
                                item1.Qty = dt.Rows[i]["QtyIssued"].ToString();
                                item1.free = dt.Rows[i]["FreeIssued"].ToString();
                                item1.Rate = dt.Rows[i]["Value"].ToString();
                            }
                            else
                            {
                                item1.Qty = dt.Rows[i]["wQtyIssued"].ToString();
                                item1.free = dt.Rows[i]["wFreeIssued"].ToString();
                                item1.Rate = dt.Rows[i]["Value"].ToString();
                            }
                            list.Add(item1);
                        }
                        obj_ord.items = list;
                        string BillNo = "", TextPost = "";
                        objPostIn = null;
                        SqlCommand cmd = new SqlCommand("", sqlcon);
                        sqlcon.Open();
                        objGlb.TrPosting_TCR(TRCODE, Vrno, ref objPostIn, ref BillNo, TextPost, "", 0, ref cmd);
                        sqlcon.Close();
                    }
                    catch (Exception ex)
                    {
                        obj_ord.vrno = "^" + Vrno + ex.Message + " DbName " + GlobalVariablesBLL.GBCDBName;
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, obj_ord);
                }
                else
                {
                    obj_ord.vrno = "^ \nTiaERPApp Service Is Restart But Failed To Reinitialize Connection. So, Re-Login Required. Don't Worry Your Data Will Be Recovered.  ";                    
                }
                return Request.CreateResponse(HttpStatusCode.OK, obj_ord);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Your TiaERPApp Service Is Stop!!! To use TiaERP@App Application, start TiaERPApp Service from AppServiceStarter.");
            }
        }

        public void RecalculateValue(string TmpVrNo,string Pcode,string FDNAME,string Ser)
        {
            try
            {
                Global obj = new Global();
                DataTable dt = obj.getDataTable(true, "select * from tmpsi where vrno='" + TmpVrNo + "'");
                if (dt.Rows.Count != 0 && dt != null)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        string icode = dt.Rows[i]["ICODE"].ToString();
                        string qty = dt.Rows[i]["QTYISSUED"].ToString().Trim();
                        string rate = dt.Rows[i]["RATE"].ToString().Trim();
                        string slno = dt.Rows[i]["SLNO"].ToString().Trim();
                        double MST = 0, VALUE = 0, MSTVAL = 0, MSTAMT = 0;
                        string MSTFlag = "";
                        if (FDNAME == "WSAL" || FDNAME == "QTNW")
                        {
                            IM objIm = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == icode);
                            if (objIm == null)
                            {
                                MST = 0;
                            }
                            else
                            {
                                MST = Convert.ToDouble((objIm.MSTWSALE == null ? 0 : objIm.MSTWSALE));
                                MSTFlag = objIm.MSTWSFLAG;
                            }
                        }
                        else
                        {
                            IM objIm = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == icode);
                            if (objIm == null)
                            {
                                MST = 0;
                            }
                            else
                            {
                                MST = Convert.ToDouble(objIm.MSTSALE == null ? 0 : objIm.MSTSALE);
                                MSTFlag = objIm.MSTSFLAG;
                            }
                        }
                        try
                        {
                            VALUE = Convert.ToDouble(qty) * Convert.ToDouble(rate);
                        }
                        catch
                        {
                            VALUE = 0;
                        }
                        string LessPerc = GetLessPerc(FDNAME, Ser, Pcode);
                        MSTVAL = VALUE - ((VALUE * Convert.ToDouble(LessPerc)) / 100);
                        MSTAMT = Convert.ToDouble(((MSTVAL * MST) / 100).ToString("##############0.00"));
                        string SQl = "update tmpsi set value=" + VALUE + ",MST=" + MST + ",MSTFLAG='" + MSTFlag + "',MSTAMT=" + MSTAMT + ",MSTVAl=" + MSTVAL + " where vrno='" + TmpVrNo + "' and slno='" + slno + "' and icode='" + icode + "'";
                        obj.ExecuteQuery(true, SQl);
                    }
                }
            }
            catch
            {
            }
        }


        [HttpGet]
        public List<AdjustBatch> IsbatchSkip(string icode, double Quty, double free, string Pcode, string FDName, string Series, string PurSz, string Convrno, string Conslno, string rate, string VrNO, string Slno)
        {
            double Qty = Quty + free;
            double SiQty = Quty, StkCr, chgQty = Quty, NewQty = Quty;
            List<Batch> obj_BatchList1 = new List<Batch>();
            List<Batch> obj_BatchList = GetBatch(icode, VrNO, Slno, FDName);
            List<AdjustBatch> list_AdjustBatch = new List<AdjustBatch>();
            Batch obj_BatchList_One = new Batch();
            if (obj_BatchList == null)
            {
                IM obj_im = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == icode);
                string packing = "";
                if (obj_im != null)
                {
                    packing = obj_im.packing;
                }
                string data = GetLastMRPAndBatch(icode);
                AdjustBatch obj = new AdjustBatch();
                obj.BatchNo = data.Split('|')[1];
                obj.Expirydate = "";
                obj.MRP = Convert.ToDecimal(data.Split('|')[0]);
                obj.packing = packing;
                obj.Rate = "0.0";
                obj.Slno = "";
                obj.qty = Quty.ToString();
                obj.free = free.ToString();
                obj.Vrno = "";
                list_AdjustBatch.Add(obj);
                goto a;
            }
            else
            {
                obj_BatchList_One = obj_BatchList.SingleOrDefault(p => p.VrNo == Convrno && p.slno == Conslno);
                if (obj_BatchList_One != null)
                {
                    if (Convert.ToDouble(obj_BatchList_One.stock) >= Qty || FDName == "QTTN" || FDName == "QTNW")
                    {
                        AdjustBatch obj = new AdjustBatch();
                        obj.BatchNo = obj_BatchList_One.BatchNo;
                        obj.Expirydate = obj_BatchList_One.Expirydate;
                        obj.MRP = obj_BatchList_One.Mrp;
                        obj.packing = obj_BatchList_One.packing;
                        obj.Rate = rate;
                        obj.Slno = obj_BatchList_One.slno;
                        obj.qty = Quty.ToString();
                        obj.free = free.ToString();
                        obj.Vrno = obj_BatchList_One.VrNo;
                        list_AdjustBatch.Add(obj);
                        goto a;
                    }
                }
            }
            if (obj_BatchList_One != null)
            {
                obj_BatchList1.Add(obj_BatchList_One);
            }
            obj_BatchList1.AddRange(obj_BatchList.Where(p => p.VrNo != Convrno && p.slno != Conslno).ToList());
            foreach (var item in obj_BatchList1)
            {
                if (FDName == "WSAL")
                {
                    if (item.stock.Contains("."))
                    {
                        continue;
                    }
                }
                if (SiQty <= 0 && free == 0)
                {
                    break;
                }
                else
                {
                    AdjustBatch obj = new AdjustBatch();
                    StkCr = Convert.ToDouble(item.stock);
                    if (SiQty > 0)
                    {
                        if (SiQty <= StkCr && SiQty <= chgQty)
                        {
                            NewQty = SiQty;
                            SiQty = 0;
                        }
                        else if (chgQty <= SiQty && chgQty <= StkCr)
                        {
                            NewQty = chgQty;
                            SiQty = SiQty - NewQty;
                        }
                        else
                        {
                            NewQty = StkCr;
                            SiQty = SiQty - StkCr;
                        }
                        chgQty = chgQty - NewQty;
                    }
                    else
                    {
                        NewQty = 0;
                    }
                    StkCr = StkCr - NewQty;
                    if (SiQty == 0 && StkCr >= free)
                    {
                        obj.free = free.ToString();
                        free = 0;
                    }
                    else
                    {
                        obj.free = 0.ToString();
                    }


                    obj.BatchNo = item.BatchNo;
                    obj.Expirydate = item.Expirydate;
                    obj.MRP = item.Mrp;
                    obj.packing = item.packing;
                    obj.Rate = GetItemRate(icode, item.Mrp.ToString(), Pcode, FDName, Series, PurSz, item.VrNo, item.slno);
                    obj.Slno = item.slno;
                    obj.qty = NewQty.ToString();
                    obj.Vrno = item.VrNo;
                    list_AdjustBatch.Add(obj);

                }
            }
        a: return list_AdjustBatch;
        }


        [HttpGet]
        public void ClearTmpSI(string vrno, string slno, string flag)
        {
            slno = "00" + slno;
            slno = slno.Substring(slno.Length - 3, 3);
            Global objgbl = new Global();
            objgbl.DeleteTmpSI(vrno, slno, flag);
        }

        [HttpGet]
        public string ConA(string a)
        {
            try
            {
                return DateTime.ParseExact("31/05/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        public string Con1(string a, string b)
        {
            try
            {
                return DateTime.ParseExact("05/31/2015", "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public HttpResponseMessage AlternateIMList(string icode,string Pcode="ZZZZZZ")
        {
            try
            {
                Global objG = new Global();
                string firstChar = icode.Substring(0, 1);
                string sql1 = "select * from im  where Icode='" + icode + "'";
                DataRow dr = objG.getDataRow(true, sql1);                
                DataTable dt = null;
                List<SubstitudeItem> obj_item = new List<SubstitudeItem>();
                if (dr != null)
                {
                    string gCode = (dr["GCODE"] == null) ? "" : dr["GCODE"].ToString();
                    string strength = (dr["Strength"] == null) ? "" : dr["Strength"].ToString();
                    GBCDbConn GbcCon = new GBCDbConn();
                    string name = GbcCon.Database.Connection.Database + ".dbo.";
                    string sql = "Declare @Error Varchar(100)  EXEC pr_GetItems_Z_wsal @ColumnName = 'Iname',  @SearchText = '', @Error      = @Error output ,   @GBCDBName      = '" + name + "', @debug      = 0 , @Fdname='' , @PTaxPackingPI='False' , @GdnCode = '',@pItemList=3, @HideItms=False,@IsTaxFromBatch='False',@IsPackFromBatch='False' ,@pItemSpclCharSearch= 'False',@GCode ='" + gCode + "',@Strength ='" + strength + "',@ICode ='" + icode + "',@ICodeFirsChar ='" + firstChar + "'";
                    dt = objG.getDataTable(false, sql);
                    
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        SubstitudeItem objS = new SubstitudeItem();
                       // objS.MRP = (dt.Rows[i]["lastmrp"] == null) ? 0 : (dt.Rows[i]["lastmrp"].ToString() == "") ? 0 : Convert.ToDouble(dt.Rows[i]["lastmrp"]);
                        objS.MRP = (dt.Rows[i]["MRP"] == null) ? 0 : (dt.Rows[i]["MRP"].ToString() == "") ? 0 : Convert.ToDouble(dt.Rows[i]["MRP"]);
                        if (objS.MRP != 0)
                        {
                            objS.Genric = getGnName(icode);
                            objS.Iname = (dt.Rows[i]["Iname"] == null) ? "" : dt.Rows[i]["Iname"].ToString();
                            objS.Icode = (dt.Rows[i]["Icode"] == null) ? "" : dt.Rows[i]["Icode"].ToString();
                            objS.Packing = (dt.Rows[i]["packing"] == null) ? "" : dt.Rows[i]["packing"].ToString();
                            objS.pursize = (dt.Rows[i]["pursize"] == null) ? "" : dt.Rows[i]["pursize"].ToString();
                            objS.shelf = (dt.Rows[i]["shelf"] == null) ? "" : dt.Rows[i]["shelf"].ToString();
                            objS.Stk = (dt.Rows[i]["stk"] == null) ? "" : dt.Rows[i]["stk"].ToString();
                            //objS.minexpiry = (dt.Rows[i]["minexpiry"] == null) ? "" :Convert.ToDateTime(dt.Rows[i]["minexpiry"]).ToString("dd/MM/yyyy");
                            //objS.maxexpiry = (dt.Rows[i]["maxexpiry"] == null) ? "" :Convert.ToDateTime( dt.Rows[i]["maxexpiry"]).ToString("dd/MM/yyyy");
                            // objS.Margin = (dt.Rows[i]["marginPerc"] == null) ? "" : dt.Rows[i]["marginPerc"].ToString();
                            objS.Rate = GetRateCon(objS.Icode, Convert.ToDouble(objS.MRP), Pcode, objS.pursize);
                            objS.WRate = (Convert.ToDouble(objS.Rate) * Convert.ToDouble(objS.pursize)).ToString();
                            obj_item.Add(objS);
                        }
                    }
                }
                obj_item = obj_item.Take(30).OrderBy(p => p.MRP).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, obj_item);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is some problem while getting information from server. Please try after some time." + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPartyRcptData(string pcode,string fdname,string Ser)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    string SQL = objRcPy.Select_GridData("", "000", fdname, Ser, null, null, "", SWFormMode.AddRecord, false, false, pcode, "");
                    List<partyReceiptData> list_party = gbc_con.Database.SqlQuery<partyReceiptData>(SQL).Take(20).ToList();
                    foreach (var item in list_party)
                    {
                        item.vrdate1 = item.vrdate.ToString("dd/MM/yyyy");
                        item.DocDate1 = item.DocDate.ToString("dd/MM/yyyy");
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, list_party);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is some problem while getting information from server. Please try after some time." + ex.Message);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }

        [HttpGet]
        public PartyDataRec LoadDataFromTMPAV(string Pcode)
        {
            string Sql = "Select * from AVLOG where pcode='" + Pcode + "'";
            DataRow dr = objGlobal.getDataRow(true, Sql);
            PartyDataRec obj = new PartyDataRec();
            if (dr != null)
            {
                obj.doc = (dr["InsType"] == null ) ? "" : dr["InsType"].ToString();
                obj.docno = (Convert.ToInt16((dr["Insno"] == null || dr["Insno"] == "") ? "1" : dr["Insno"].ToString()) + 1).ToString();
                Sql = "select pcode,pname from acpmst where pcode='" + ((dr["avcode"] == null) ? "" : dr["avcode"].ToString()) + "'";
                dr = objGlobal.getDataRow(true, Sql);
                if (dr != null)
                {
                    obj.accode = (dr["pcode"] == null) ? "" : dr["pcode"].ToString();
                    obj.codedesc = (dr["pname"] == null) ? "" : dr["pname"].ToString(); 
                }
            }
            Sql = "select Bankname,Branchname from acpmst where pcode='" + Pcode + "'";
            dr = objGlobal.getDataRow(true, Sql);
            if (dr != null)
            {
                obj.Banker = (dr["BankName"] == null) ? "" : dr["BankName"].ToString();
                obj.Branch = (dr["BranchName"] == null) ? "" : dr["BranchName"].ToString(); 
            }
            return obj;
        }


        [HttpPost]
        public HttpResponseMessage SaveRPVr(ClsRPVoucher obj)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    GBCDbConn GbcCon = new GBCDbConn();
                    string GBCDBName = GbcCon.Database.Connection.Database + ".dbo.";
                    Global gbl = new Global();
                    ClsRPVoucher ObjVrRecpt = new ClsRPVoucher();
                    ObjVrRecpt = obj;
                    string RefVrNo;

                    //genrate Vrno
                    obj.VrNo = gbl.GenrateVrNo(obj.trcode, GBCDBName + "AV", obj.Series, DateTime.Now, "AcVrNo");
                    ObjVrRecpt.IsNew = true;
                    if (obj.VrNo == "")
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is some problem while saving information on server. Please try after some time. VrNo is Blank");
                    }
                    RefVrNo = obj.VrNo;
                    if (obj.trcode == "MNRC")
                    {
                        ObjVrRecpt.DrCr = "D";
                        ObjVrRecpt.AtType = "C";
                        ObjVrRecpt.ConAmtDr = 0;
                        ObjVrRecpt.CodeCr = obj.Pcode;
                        ObjVrRecpt.CodeDr = obj.ACCode;
                    }
                    else
                    {
                        ObjVrRecpt.DrCr = "C";
                        ObjVrRecpt.AtType = "S";
                        ObjVrRecpt.ConAmtCr = 0;
                        ObjVrRecpt.CodeCr = obj.ACCode;
                        ObjVrRecpt.CodeDr = obj.Pcode;
                    }
                    ObjVrRecpt.VrNo = obj.Series;
                    ObjVrRecpt.DocDate = DateTime.Now;
                    ObjVrRecpt.AcVrNo = RefVrNo;
                    ObjVrRecpt.VrDate = DateTime.Now;
                    ObjVrRecpt.VrTime = DateTime.Now;
                    ObjVrRecpt.TrAmt = obj.VrAmt;
                    ObjVrRecpt.AVCode = obj.ACCode;
                    ObjVrRecpt.DeleteOldEntry();
                    List<clsVrItem> listVrItem = new List<clsVrItem>();
                    foreach (var item in obj.VrItem)
                    {
                        clsVrItem objItem = new clsVrItem();
                        if (item.BalAmt != 0)
                        {
                            if (obj.trcode == "MNRC")
                            {
                                objItem.DrVrNo = item.DrVrNo;
                                objItem.CRSlNo = "001";
                                objItem.DrSlNo = item.DrSlNo;
                                objItem.CrVrNo = RefVrNo;
                                ObjVrRecpt.TOF = "D";
                            }
                            else
                            {
                                objItem.CrVrNo = item.DrVrNo;
                                objItem.DrSlNo = "001";
                                objItem.CRSlNo = item.DrSlNo;
                                objItem.DrVrNo = RefVrNo;
                                ObjVrRecpt.TOF = "C";
                            }
                            objItem.AdjAmt = item.AdjAmt;
                            objItem.BalAmt = item.RemAmt;
                            objItem.PCODE = item.PCODE;
                            objItem.VrDate = ObjVrRecpt.VrDate.ToString("MM/dd/yyyy");
                            objItem.dt = ObjVrRecpt.VrDate.ToString("dd/MM/yyyy");
                            listVrItem.Add(objItem);
                        }
                    }
                    ObjVrRecpt.SaveDataIntoTMPAV(GBCDBName, obj.Pcode, obj.doc, obj.DocNo, obj.AVCode);
                    ObjVrRecpt.VrItem = listVrItem;
                    if (ObjVrRecpt.SaveRecord())
                    {
                      
                    }
                    else
                    {
                       
                    }

                    if (obj.Pcode != "" || obj.Pcode != null)
                    {
                        if (obj.Bankers1 != "")
                        {
                            if (obj.Pcode == "ZZZZZZ")
                            {
                                objRcPy.Update_PM_Banker(obj.RefNo, obj.Bankers1, obj.Bankers2);
                            }
                            else
                            {
                                objacpmst.PCODE = obj.Pcode;
                                objacpmst.BANKERS1 = obj.Bankers1;
                                objacpmst.BANKERS2 = obj.Bankers2;
                                objacpmst.Update_AcpMst("Bankers", "");
                            }
                        }
                    }
                    int strtchqno, endchqno;
                    DataTable dt = objRcPy.Select_ChequeMaster(obj.ACCode);
                    if (dt.Rows.Count != 0)
                    {
                        for (int i = 0; i <= dt.Rows.Count - 1; i++)
                        {
                            DataRow drRcPy = dt.Rows[i];
                            strtchqno = (drRcPy["StartInsno"] == null) ? 0 : Convert.ToInt16(drRcPy["StartInsno"]);
                            endchqno = (drRcPy["EndInsno"] == null) ? 0 : Convert.ToInt16(drRcPy["EndInsno"]);
                            if (Convert.ToInt16(obj.DocNo) >= strtchqno && Convert.ToInt16(obj.DocNo) <= endchqno)
                            {
                                objRcPy.Update_ChequeMaster_CurrentInsno(obj.DocNo, obj.ACCode, strtchqno.ToString(), endchqno.ToString());
                            }
                        }
                    }

                    if (ObjVrRecpt.GetSMSManagement(obj.trcode))
                    {
                        string LedgBal = ObjVrRecpt.RtLedgBal(obj.Pcode);
                        string OutstBal = ObjVrRecpt.RtLedgBal(obj.Pcode);
                        (new TIA3T.NEW.BLL.ClsSms()).SheduleSMSNew(obj.trcode, "NEW", obj.AcVrNo, obj.VrAmt.ToString(), DateTime.Now, DateTime.Now, OutstBal, LedgBal, DateTime.Now, DateTime.Now, obj.VrUserID);
                    }
                    (new TIA3T.BLL.clsRecieptPayment()).Update_TRSUB_VrId(RefVrNo, "0");
                    if (obj.trcode == "MNRC")
                    {
                        ObjVrRecpt.UpdatePayee();
                    }
                    else
                    {
                        ObjVrRecpt.UpdatePayer();
                    }
                    ObjVrRecpt.VrNo = ObjVrRecpt.VrDate.ToString("dd/MM/yyyy");
                    return Request.CreateResponse(HttpStatusCode.OK, ObjVrRecpt);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is some problem while saving information on server. Please try after some time." + ex.Message);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "");
            }
        }


        [HttpGet]
        public string GetLastMRPAndBatch(string Icode)
        {
            Global objGbl=new Global();
            string lastMRP = "0.1", BatchNo = ".";
            try
            {
                string sql = "select lastMRP from st where icode='" + Icode + "'";
                DataRow dr = objGbl.getDataRow(true, sql);
                if (dr != null)
                {
                    lastMRP = (dr["lastMRP"] == null) ? "0.1" : dr["lastMRP"].ToString();
                }
                sql = "select top 1 batchno from cr where icode='" + Icode + "' order by vrdate desc";
                dr = objGbl.getDataRow(true, sql);
                if (dr != null)
                {
                    BatchNo = (dr["batchno"] == null) ? "." : dr["batchno"].ToString();
                }
            }
            catch
            {

            }
            return lastMRP + "|" + BatchNo;
        }

        [HttpGet]
        public string GetBoxSz(string Icode)
        {
            try
            {
               DataRow dr= (new TIA3T.NEW.BLL.ClsIMMst()).Select_onIcode(Icode);
               if (dr != null)
               {
                   return ((dr["Boxsize"] == null) ? "0.00" : dr["Boxsize"].ToString());
               }
               else
               {
                   return "0.00";
               }
            }
            catch
            {
                return "0.00";
            }
        }

        [HttpGet]
        public DataRow SelectRandomPt()
        {
            Random rnd = new Random();
            int r = rnd.Next(1, 50);
            string str = "select top 1 PtName,PtCode from (select top  " + r + " PtNAme,PtCode from PatientMaster (nolock)  where ptcode not in('999999999999','111111111111') Order by PtCode Desc ) as Q1 Where len(isnull (PtName,'')) > 2 Order by PtCode ";
            DataRow dr=(new Global()).getDataRow(false, str);
            return dr;
        }
        [HttpGet]
        public DataRow SelectRandomDr()
        {
            Random rnd = new Random();
            int r = rnd.Next(1, 50);
            string str = "select top 1 Name,Hrcode from (select top  " + r + " Name,Hrcode  from hrMaster (nolock) Order by hrcode Desc ) as Q1 Order by hrcode ";
            DataRow dr = (new Global()).getDataRow(false, str);
            return dr;
        }
    }
}





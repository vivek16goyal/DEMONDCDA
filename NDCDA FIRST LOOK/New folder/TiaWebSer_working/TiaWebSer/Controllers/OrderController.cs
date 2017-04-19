using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Script.Serialization;
using TiaWebSer.Models;



namespace TiaWebSer.Controllers
{
    public class OrderController : ApiController
    {
        TIA3T.NEW.BLL.clsAllReports objRpt = new TIA3T.NEW.BLL.clsAllReports();
        GBCDbConn gbc_con = new GBCDbConn();
        DbConn con = new DbConn();
        string GBCServicePath = "http://tiaapp.goyalonline.in/";
        [HttpGet]
        public HttpResponseMessage GetArea(string name)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    List<areamst> list_area = con.Db_area.Where(p => p.aName.StartsWith(name)).Take(10).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, list_area);
                }
                catch (Exception ex)
                {
                    List<areamst> list_item = new List<areamst>();
                    areamst im_ = new areamst();
                    im_.aName = "$" + ex.Message;
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
        public HttpResponseMessage GetParty(string name, string area)
        {
            bool isno = false;
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    try
                    {
                        Int64 i = Convert.ToInt64(name.Trim());
                        isno = true;
                    }
                    catch
                    {
                        isno = false;
                    } 
                    List<ACPMST> list_acpmst;
                    if (area != null)
                    {
                        if (isno)
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == "C" && p.ConsumerMob.StartsWith(name) && p.areacode == area).Take(10).ToList();
                        }
                        else
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == "C" && p.PNAME.StartsWith(name) && p.areacode == area).Take(10).ToList();
                        }
                    }
                    else
                    {
                        if (isno)
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == "C" && p.ConsumerMob.StartsWith(name)).Take(10).ToList();
                        }
                        else
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == "C" && p.PNAME.StartsWith(name)).Take(10).ToList();
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, list_acpmst);
                }
                catch (Exception ex)
                {
                    List<ACPMST> list_item = new List<ACPMST>();
                    ACPMST im_ = new ACPMST();
                    im_.PNAME = "$" + ex.Message;
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
        public HttpResponseMessage Getallparty(string Stcode, string name)
        {
            GBCDbConn con = new GBCDbConn();
            try
            {
                List<ACPMST> list_party = con.Db_acpmst.Where(p => p.PTYPE == "C").ToList();
                return Request.CreateResponse(HttpStatusCode.OK, list_party);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetAcCode(string name, string Type)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    List<ACPMST> list_acpmst;
                    if (String.IsNullOrEmpty(name))
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == Type ).Take(20).ToList();
                    }
                    else
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == Type && p.PNAME.StartsWith(name)).Take(20).ToList();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, list_acpmst);
                }
                catch (Exception ex)
                {
                    List<ACPMST> list_item = new List<ACPMST>();
                    ACPMST im_ = new ACPMST();
                    im_.PNAME = "$" + ex.Message;
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
        public HttpResponseMessage GetPartyALL(string name)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    bool isno = false;
                    try
                    {
                        Int64 i = Convert.ToInt64(name.Trim());
                        isno = true;
                    }
                    catch
                    {
                        isno = false;
                    }
                    List<ACPMST> list_acpmst;
                    if (isno)
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.ConsumerMob.StartsWith(name)).Take(10).ToList();
                    }
                    else
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.PNAME.StartsWith(name)).Take(10).ToList();
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, list_acpmst);
                }
                catch (Exception ex)
                {
                    List<ACPMST> list_item = new List<ACPMST>();
                    ACPMST im_ = new ACPMST();
                    im_.PNAME = "$" + ex.Message;
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
        public HttpResponseMessage GetPartyRcPy(string name, string fdanme)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    bool isno = false;
                    try
                    {
                        Int64 i = Convert.ToInt64(name.Trim());
                        isno = true;
                    }
                    catch
                    {
                        isno = false;
                    }
                    List<ACPMST> list_acpmst;
                    if (fdanme == "MNRC")
                    {
                        if (isno)
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.ConsumerMob.StartsWith(name) && p.PTYPE == "C").Take(10).ToList();
                        }
                        else
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PNAME.StartsWith(name) && p.PTYPE == "C").Take(10).ToList();
                        }
                    }
                    else
                    {
                        if (isno)
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.ConsumerMob.StartsWith(name) && p.PTYPE == "S").Take(10).ToList();
                        }
                        else
                        {
                            list_acpmst = gbc_con.Db_acpmst.Where(p => p.PNAME.StartsWith(name) && p.PTYPE == "S").Take(10).ToList();
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, list_acpmst);
                }
                catch (Exception ex)
                {
                    List<ACPMST> list_item = new List<ACPMST>();
                    ACPMST im_ = new ACPMST();
                    im_.PNAME = "$" + ex.Message;
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
        public string Getldgbal(string pcode)
        {
            try
            {
                Global obj_global = new Global();
                List<string> ldgBal = gbc_con.Database.SqlQuery<string>("select  dbo.ufnGetLdgBal({0})", pcode).ToList();
                GBCDbConn GbcCon = new GBCDbConn();
                string name = GbcCon.Database.Connection.Database + ".dbo.";
                //string opBal = GetOpBal(pcode, obj_global.ACYR_START_DATE(DateTime.Now.Date), name).ToString().Trim().Replace("Dr","").Replace("Cr","");
                string ldgBAl = ldgBal[0].Trim().Replace("Dr", "").Replace("Cr", "");
                ldgBAl = (ldgBAl == "Nil") ? "0" : ldgBAl;
                double val = Math.Abs(Convert.ToDouble(ldgBAl));
                string result = (val > 0) ? val + "Dr" : val + "Cr";
                return result;
            }
            catch
            {
                return "0.00";
            }
        }

       

        public double? GetOpBal(string pcode, DateTime? frmdate, string name)
        {
            PartyOpBal Obj_partyOpbal = gbc_con.Database.SqlQuery<PartyOpBal>("EXEC sp_OPBalance @Pcode={0},@frmdate={1},@GBCName={2}", pcode, frmdate, name).SingleOrDefault(p => p.PartyName != "");
            return Obj_partyOpbal.OpBalance;
        }

        [HttpGet]
        public List<PartyLdgr> GetPartyLedger(string pcode, DateTime frmdate, DateTime todate, bool IsOPBalInclude, bool IsSummary)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    Global obj_global = new Global();
                    if (frmdate == null)
                    {
                        frmdate = obj_global.ACYR_START_DATE(DateTime.Now.Date);
                    }
                    if (todate == null)
                    {
                        todate = obj_global.ACYR_END_DATE(DateTime.Now.Date);
                    }
                    double? opBal;
                    double? TDrAmt = 0;
                    double? TCrAmt = 0;
                    double? DrAmt = 0;
                    double? CrAmt = 0;
                    double? Bal = 0;
                    GBCDbConn GbcCon = new GBCDbConn();
                    string name = GbcCon.Database.Connection.Database + ".dbo.";
                    //PartyLdgr obj_Party = new PartyLdgr();
                    ////get op balance
                    name = name.Replace(obj_global.ACYR_START_DATE(DateTime.Now.Date).Year.ToString().Trim(), obj_global.ACYR_START_DATE(Convert.ToDateTime(frmdate)).Year.ToString().Trim());
                    if (IsOPBalInclude)
                    {
                        opBal = GetOpBal(pcode, frmdate, name);
                        if (opBal < 0)
                        {
                            TCrAmt = opBal * -1;
                        }
                        else {
                            TDrAmt = opBal;
                        }
                        
                    }
                    else
                    {
                        opBal = 0;
                        TCrAmt = 0;
                        TDrAmt = 0;
                    }

                    //get party ledger
                    string SQL = objRpt.ShowLedgerSummary_Query(false, true, 0, 0, "", false, "", "General Ledger", "", "", "'" + pcode + "'", name, frmdate.ToString("dd/MM/yyyy"), todate.ToString("dd/MM/yyyy"), "All", "");
                    List<PartyLdgr> list_partyLdgr = new List<PartyLdgr>();
                    List<PartyLedger> list_partyLdgr1;
                    list_partyLdgr1 = gbc_con.Database.SqlQuery<PartyLedger>(SQL).ToList();
                    double? Opbal = 0;
                    double? balance = 0;
                    string AmtType;
                    for (int i = 0; i <= list_partyLdgr1.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            Opbal = list_partyLdgr1[i].OPBalance;
                        }
                        if (i != 0)
                        {
                            PartyLdgr obj = new PartyLdgr();
                            obj.Description = "";
                            string dt = (list_partyLdgr1[i].Mdate == null) ? "" : list_partyLdgr1[i].Mdate;
                            string Line1 = "";

                            if (dt != "")
                            {
                                string[] SplStr = dt.Split('-');
                                dt = dt.Substring(0, 3) + "-" + SplStr[1].ToString();
                            }

                            string InsStr = (list_partyLdgr1[i].Insno == "") ? "" : list_partyLdgr1[i].InsType + " No." + list_partyLdgr1[i].Insno + "  Dt." + list_partyLdgr1[i].InsDate;
                            Line1 = list_partyLdgr1[i].Name;
                            Line1 = Line1 + " " + list_partyLdgr1[i].Narr1 + " " + list_partyLdgr1[i].Narr2 + " " + InsStr;
                            obj.Description = Line1;
                            obj.dramt = (list_partyLdgr1[i].Debit == null) ? 0 : list_partyLdgr1[i].Debit;
                            DrAmt = obj.dramt;
                            TDrAmt = TDrAmt + DrAmt;
                            obj.cramt = (list_partyLdgr1[i].Credit == null) ? 0 : list_partyLdgr1[i].Credit;
                            CrAmt = obj.cramt;
                            TCrAmt = TCrAmt + CrAmt;
                            if (TCrAmt < TDrAmt)
                            {
                                Bal = TDrAmt - TCrAmt;
                                AmtType = " Dr";
                            }
                            else
                            {
                                Bal = TCrAmt - TDrAmt;
                                AmtType = " Cr";
                            }
                            obj.Dt = dt;
                            obj.Balance = Bal;
                            balance = obj.Balance;
                            list_partyLdgr.Add(obj);
                        }
                    }

                    PartyLdgr obj_last = new PartyLdgr();
                    obj_last.Description = "Total";
                    obj_last.cramt = TCrAmt;
                    obj_last.dramt = TDrAmt;
                    obj_last.Balance = balance;
                    obj_last.OPBalance = Opbal;
                    list_partyLdgr.Add(obj_last);


                    return list_partyLdgr;

                }
                catch (Exception ex)
                {
                    List<PartyLdgr> list_partyLdgr = new List<PartyLdgr>();
                    PartyLdgr obj = new PartyLdgr();
                    obj.Error = "1";
                    obj.Description = ex.Message;
                    list_partyLdgr.Add(obj);
                    return list_partyLdgr;
                }
            }
            else
            {
                return null;
            }
        }


        [HttpPost]
        public Order SaveOrder(Order ord)
        {
            try
            {
                Global obj_g = new Global();
                return obj_g.SaveOrder(ord);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveOrderCon(Order ord)
        {

            try
                {
                    Global obj_g = new Global();
                    if (ord.FDName == "" || ord.FDName == null || ord.FDName == "ORDR")
                    {
                        ord.FDName = "ORDR";
                        if (ord.series == "" || ord.series == null)
                        {
                            VC objVC = gbc_con.Db_vc.Where(p => p.FDNAME == "ORDR").Take(1).SingleOrDefault();
                            if (objVC == null)
                            {
                                ord.series = "O2";
                            }
                            else
                            {
                                ord.series = objVC.SER;
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, obj_g.SaveOrder(ord));
                }
                catch (Exception ex)
                {
                    Order ord1 = new Order();
                    ord1.pcode = ex.Message + " Inner Exception Message " + ex.InnerException.InnerException.ToString().Split('\n')[0].ToString();
                    return Request.CreateResponse(HttpStatusCode.OK, ord1);
                }
           
        }
        [HttpPost]
        public Order1 SaveOrderCon1(Order1 ord)
        {
            try
            {
                Global obj_g = new Global();
                if (ord.FDName == "" || ord.FDName==null || ord.FDName == "ORDR")
                {
                    ord.FDName = "ORDR";
                    if (ord.series == "" || ord.series == null)
                    {
                        VC objVC = gbc_con.Db_vc.Where(p => p.FDNAME == "ORDR").Take(1).SingleOrDefault();
                        if (objVC == null)
                        {
                            ord.series = "O2";
                        }
                        else
                        {
                            ord.series = objVC.SER;
                        }
                    }
                }
                return obj_g.SaveOrder1(ord);
            }
            catch (Exception ex)
            {
                Order1 ord1 = new Order1();
                ord1.pcode = ex.Message + " Inner Exception Message " + ex.InnerException.InnerException.ToString().Split('\n')[0].ToString();
                return ord1;
            }
        }



        //public void FireUrl(string Url)
        //{
        //    try
        //    {
        //        ASCIIEncoding encoder = new ASCIIEncoding();
        //        byte[] data = encoder.GetBytes(serializedObject); 

        //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
        //        request.Method = "POST";
        //        request.ContentType = "application/json";
        //        request.ContentLength=Data.
                

        //        WebResponse response = request.GetResponse();
        //        Stream stream = response.GetResponseStream();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        // checked party
        [HttpGet]
        public string checkparty(string PtCode)
        {
            try
            {

                GBCDbConn con = new GBCDbConn();

                ACPMST obj1 = con.Db_acpmst.SingleOrDefault(p => p.PCODE == PtCode);
                ACPMST_temp obj_C = con.ACPMST_temp.SingleOrDefault(p => p.PtCode == PtCode);
                if (obj1 == null)
                {


                    if (obj_C == null)
                    {
                        return "0";
                    }
                    else
                    {
                        GBCCon con1 = new GBCCon();
                        ServiceReg obj = con1.db_SerReg.SingleOrDefault(p => p.Pcode == obj_C.PCODE);
                        if (obj == null)
                        {
                            return obj_C.ServicePath + "|" + "" + "|" + "" + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|";
                        }
                        else
                        {
                            return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|";
                        }
                    }
                }

                else
                {
                    GBCCon con1 = new GBCCon();
                    ServiceReg obj = con1.db_SerReg.SingleOrDefault(p => p.Pcode == obj_C.PCODE);
                    if (obj == null)
                    {
                        return obj_C.ServicePath + "|" + "" + "|" + "" + "|" + obj_C.PtName + "|" + obj1.PCODE + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|";
                    }
                    else
                    {
                        return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj1.PCODE + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|";
                    }
                }

            }
            catch (Exception ex)
            {
                return "$" + ex.Message;
            }
        }
        [HttpGet]
        public string GetStartEndDate()
        {
            Global obj_global = new Global();
            string date = obj_global.ACYR_START_DATE(DateTime.Now.Date).ToString("yyyy-MM-dd") + " % " + obj_global.ACYR_END_DATE(DateTime.Now.Date).AddDays(1).ToString("yyyy-MM-dd");
            return date;
        }
        [HttpGet]
        public IEnumerable<VCList> GetSer()
        {
            List<VCList> obj = new List<VCList>();
            try
            {               
                VCList o = new VCList();
                VCList o1 = new VCList();
                VCList o2 = new VCList();
                VCList o3 = new VCList();
                VCList o4 = new VCList();
                VCList o5 = new VCList();
                VCList o6 = new VCList();
                VCList o7 = new VCList();

                List<VC> list_vc = gbc_con.Db_vc.Where(p => p.FDNAME=="SALE").ToList();                
                o.FDNAME = "SALE";
                o.vcli = list_vc;
                obj.Add(o);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "WSAL").ToList();
                o1.FDNAME = "WSAL";
                o1.vcli = list_vc;
                obj.Add(o1);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "ORDS").ToList();
                o2.FDNAME = "ORDS";
                o2.vcli = list_vc;
                obj.Add(o2);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "ORDR").ToList();
                o3.FDNAME = "ORDR";
                o3.vcli = list_vc;
                obj.Add(o3);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "MNRC").ToList();
                o4.FDNAME = "MNRC";
                o4.vcli = list_vc;
                obj.Add(o4);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "MNPY").ToList();
                o5.FDNAME = "MNPY";
                o5.vcli = list_vc;
                obj.Add(o5);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "QTTN").ToList();
                o6.FDNAME = "QTTN";
                o6.vcli = list_vc;
                obj.Add(o6);

                list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "QTNW").ToList();
                o7.FDNAME = "QTNW";
                o7.vcli = list_vc;
                obj.Add(o7);

                return obj;
            }
            catch 
            {
                return obj;
            }
        }
        [HttpGet]
        public List<OB> OrderVrNOList(string PtCode)
        {
            GBCDbConn gbc_con = new GBCDbConn();
            List<OB> list_ord = gbc_con.Db_ob.Where(p => p.PCODE == PtCode).OrderByDescending(p => p.VRDATE).OrderByDescending(p => p.VRNO).ToList();
            return list_ord;
        }
        [HttpGet]
        public List<Order> Order_history(string PtCode)
        {
            List<OB> list_ord = gbc_con.Db_ob.Where(p => p.PatientID == PtCode).ToList();
            List<Order> ord = new List<Order>();
            foreach (var item in list_ord)
            {
                Order obj_ord = new Order();
                obj_ord.DrName = item.DrName;
                obj_ord.NameP = item.NameP;
                obj_ord.TotalAmt = item.ORAMT.ToString();
                obj_ord.vrdate = Convert.ToDateTime(item.VRDATE).ToString("dd/MM/yyyy");
                obj_ord.vrno = item.VRNO;
                obj_ord.imgName = item.PresImg;
                List<ItemMaster> li_Oi = new List<ItemMaster>();
                List<OI> OI = gbc_con.Db_OI.Where(p => p.VRNO == item.VRNO).ToList();
                foreach (var oi_item in OI)
                {
                    try
                    {
                        ItemMaster obj_oi = new ItemMaster();
                        try
                        {
                            obj_oi.INAME = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == oi_item.ICODE).INAME;
                        }
                        catch
                        {
                            obj_oi.INAME = oi_item.ICODE;
                        }
                        obj_oi.Qty = oi_item.ORDERQTY.ToString();
                        obj_oi.free = oi_item.FREEQTY.ToString();
                        obj_oi.Rate = oi_item.ORDRATE.ToString();
                        obj_oi.ICODE = oi_item.ORDVAL.ToString();
                        li_Oi.Add(obj_oi);
                    }
                    catch
                    {
                    }
                }
                obj_ord.items = li_Oi;
                ord.Add(obj_ord);               
            }
            ord = ord.OrderByDescending(p=>p.vrdate).OrderByDescending(p=>p.vrno).ToList();
            return ord;
        }
        [HttpGet]
        public List<Order> Order_history1(string PtCode)
        {
            List<OB> list_ord = gbc_con.Db_ob.Where(p => p.PCODE == PtCode).ToList();
            List<Order> ord = new List<Order>();
            foreach (var item in list_ord)
            {
                Order obj_ord = new Order();
                obj_ord.DrName = item.DrName;
                obj_ord.NameP = item.NameP;
                obj_ord.TotalAmt = item.ORAMT.ToString();
                obj_ord.vrdate = Convert.ToDateTime(item.VRDATE).ToString("dd/MM/yyyy");
                obj_ord.vrno = item.VRNO;
                obj_ord.imgName = item.PresImg;
                List<ItemMaster> li_Oi = new List<ItemMaster>();
                List<OI> OI = gbc_con.Db_OI.Where(p => p.VRNO == item.VRNO).ToList();
                foreach (var oi_item in OI)
                {
                    try
                    {
                        ItemMaster obj_oi = new ItemMaster();
                        try
                        {
                            obj_oi.INAME = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == oi_item.ICODE).INAME;
                        }
                        catch
                        {
                            obj_oi.INAME = oi_item.ICODE;
                        }
                        obj_oi.Qty = oi_item.ORDERQTY.ToString();
                        obj_oi.free = oi_item.FREEQTY.ToString();
                        obj_oi.Rate = oi_item.ORDRATE.ToString();
                        obj_oi.ICODE = oi_item.ORDVAL.ToString();
                        li_Oi.Add(obj_oi);
                    }
                    catch
                    {
                    }
                }
                obj_ord.items = li_Oi;
                obj_ord.status = GetOrdStatus(obj_ord.vrno);
                ord.Add(obj_ord);
            }
            ord = ord.OrderByDescending(p => p.vrdate).OrderByDescending(p => p.vrno).ToList();
            return ord;
        }

        [HttpGet]
        public HttpResponseMessage GetStateList(string name)
        {
            DbConn con = new DbConn();
            try
            {
                List<statemaster> list_state = con.Db_state.Where(p=>p.StName.StartsWith(name)).ToList();
                if (list_state.Count == 0)
                {
                    list_state = GetSateListFromGBC(name);
                }
                return Request.CreateResponse(HttpStatusCode.OK, list_state);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }

        }

      



        [HttpGet]
        public HttpResponseMessage GetCityList(string Stcode,string name)
        {
            DbConn con = new DbConn();
            try
            {
                List<citymaster> list_city = con.Db_city.Where(p=>p.CtCode.StartsWith(Stcode) && p.CtName.StartsWith(name)).ToList();
                if (list_city.Count == 0)
                {
                    list_city = GetCityListFromGBC(Stcode, name);
                }
                return Request.CreateResponse(HttpStatusCode.OK, list_city);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetAreaList(string name)
        {
            DbConn con = new DbConn();
            try
            {
                List<areamst> list_area = con.Db_area.Where(p=>p.aName.Contains(name)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, list_area);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        public List<statemaster> GetSateListFromGBC(string name)
        {
            string Url = GBCServicePath + "Values/GetStateList?name=" + name;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json, text/javascript, */*";
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            List<statemaster> myobj;
            using (StreamReader reader = new StreamReader(stream))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = reader.ReadToEnd();
                myobj = (List<statemaster>)js.Deserialize(objText, typeof(List<statemaster>));
            }
            return myobj;
        }

        public List<citymaster> GetCityListFromGBC(string Stcode, string name)
        {
            string Url = GBCServicePath + "Values/GetStateList?Stcode=" + Stcode + "&name=" + name;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json, text/javascript, */*";
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            List<citymaster> myobj;
            using (StreamReader reader = new StreamReader(stream))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = reader.ReadToEnd();
                myobj = (List<citymaster>)js.Deserialize(objText, typeof(List<citymaster>));
            }
            return myobj;
        }

        public List<areamst> GetAreaListFromGBC(string name)
        {
            string Url = GBCServicePath + "Values/GetAreaList?name=" + name;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json, text/javascript, */*";
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            List<areamst> myobj;
            using (StreamReader reader = new StreamReader(stream))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var objText = reader.ReadToEnd();
                myobj = (List<areamst>)js.Deserialize(objText, typeof(List<areamst>));
            }
            return myobj;
        }

        [HttpGet]
        public string  GetOrdStatus(string Vrno)
        {            
            try
            {
                string status = "";
                OB obj = gbc_con.Db_ob.SingleOrDefault(p => p.VRNO == Vrno);
                if (obj != null)
                {
                    status = obj.Status;
                }
                return status;
            }
            catch 
            {
                return "";
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOrderTrackingDetail(string Vrno)
        {
            try
            {
                OB obj_ob = gbc_con.Db_ob.SingleOrDefault(p => p.VRNO == Vrno);
                if (obj_ob != null)
                {
                    if (obj_ob.Status == "BEP" || obj_ob.Status == "DIS" || obj_ob.Status == "DLV" || obj_ob.Status == "RCD")
                    {
                        DataRow dr = (new Global()).getDataRow(true, "select * from Sb where vrno='" + obj_ob.ORDERNO + "'");
                        if (dr != null)
                        {
                            obj_ob.BillAmt = (dr["NetAmt"] == null) ? 0.00 : Convert.ToDouble(dr["NetAmt"].ToString());
                            obj_ob.BillDt = (dr["vrdate"] == null) ? "" :Convert.ToDateTime(dr["vrdate"]).ToString("dd/MM/yyyy");
                            obj_ob.DCode = "Wallete Debit Amt:" + obj_ob.EWD;
                            obj_ob.NameP = "Wallete Credit Amt: " + ((dr["WaltDiscAmt"] == null) ? 0.00 : Convert.ToDouble(dr["WaltDiscAmt"].ToString())) ;
                            //obj_ob.BillRemark = (dr["BillRemark"] == null) ? "" : dr["BillRemark"].ToString();
                            //obj_ob.ORDERNO = (dr["vrno"] == null) ? "" : dr["vrno"].ToString();
                        }                        
                    }
                    obj_ob.BillRemark = (obj_ob.BillRemark == null) ? "" : obj_ob.BillRemark;
                    obj_ob.BillRemark = (obj_ob.BillRemark == null) ? "" : obj_ob.BillRemark;
                    obj_ob.TRCode = Convert.ToDateTime(obj_ob.VRDATE).ToString("dd/MM/yyyy");
                }
               
                return Request.CreateResponse(HttpStatusCode.OK, obj_ob);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Error Occurred While Getting Data. "+ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetWalleteBal(string Ptcode)
        {
            try
            {
                Global obj = new Global();
                return Request.CreateResponse(HttpStatusCode.OK, obj.getWalletBalance(Ptcode));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Error Occurred While Getting Wallet Balance. " + ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetLastWalletetTrans(string Ptcode)
        {
            try
            {
                Global obj = new Global();
                return Request.CreateResponse(HttpStatusCode.OK, obj.GetLastTransc(Ptcode));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Error Occurred While Getting Last Wallet Transcztion. " + ex.Message);
            }
        }

        [HttpGet]
        public void UpdateRcdStatus(string OrdNo)
        {
            try
            {
                Global obj = new Global();
                string Sql = "update ob set status = 'RCD' where status='DIS' and vrno='" + OrdNo + "'";
                obj.ExecuteQuery(true, Sql);
            }
            catch
            {
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOrdDataOnVrno(string VrNo)
        {
            try
            {
                GBCDbConn GbcCon = new GBCDbConn();
                string GBCDBname = GbcCon.Database.Connection.Database + ".dbo.";
                Global objGBL = new Global();
                OB ordObj = gbc_con.Db_ob.SingleOrDefault(p => p.VRNO == VrNo);
                Order obj_ord = new Order();
                if (ordObj != null)
                {
                    obj_ord.NameP = ordObj.NameP;
                    obj_ord.TotalAmt = ordObj.ORAMT.ToString();
                    obj_ord.vrdate = Convert.ToDateTime(ordObj.VRDATE).ToString("dd/MM/yyyy");
                    obj_ord.vrno = ordObj.VRNO;
                    obj_ord.imgName = ordObj.PresImg;
                    List<ItemMaster> li_Oi = new List<ItemMaster>();
                    List<OI> OI = gbc_con.Db_OI.Where(p => p.VRNO == VrNo).ToList();
                    foreach (var oi_item in OI)
                    {
                        try
                        {
                            ItemMaster obj_oi = new ItemMaster();
                            
                            try
                            {
                                IM IMObj = gbc_con.Db_itm.SingleOrDefault(p => p.ICODE == oi_item.ICODE);
                                if (IMObj != null)
                                {
                                    obj_oi.INAME = IMObj.INAME;
                                    obj_oi.packing = IMObj.packing;
                                    obj_oi.WRate = (oi_item.ORDRATE * IMObj.PURSIZE).ToString();
                                    DataRow dr = objGBL.getDataRow(false, "SELECT GNAME FROM " + GBCDBname + "GM  WHERE isnull( GCODE,'')='" + (IMObj.GCODE == null ? "" : IMObj.GCODE) + "' ");
                                    if (dr != null)
                                    {
                                        obj_oi.GNAme = dr["GNAME"].ToString();
                                    }
                                }
                            }
                            catch
                            {
                                obj_oi.INAME = oi_item.ICODE;
                            }
                            
                            obj_oi.Qty = oi_item.ORDERQTY.ToString();
                            obj_oi.free = oi_item.FREEQTY.ToString();
                            obj_oi.WQty = oi_item.ORDERWQTY.ToString();
                            obj_oi.Wfree = oi_item.FREEWQTY.ToString();
                            obj_oi.Rate = oi_item.ORDRATE.ToString();
                            obj_oi.ICODE = oi_item.ICODE;
                            obj_oi.Mrp = oi_item.ORDMRP.ToString();

                            li_Oi.Add(obj_oi);
                        }
                        catch
                        {
                        }
                    }
                    obj_ord.items = li_Oi;
                }
                return Request.CreateResponse(HttpStatusCode.OK, obj_ord);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Error Occurred While Getting Data. " + ex.Message);
            }
        }


    }
}

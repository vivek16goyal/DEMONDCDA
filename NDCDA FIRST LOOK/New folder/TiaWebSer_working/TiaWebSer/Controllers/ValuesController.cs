using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Script.Serialization;
using TiaWebSer.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web;

namespace TiaWebSer.Controllers
{
    public class ValuesController : ApiController
    {
        DAl objdal = new DAl();
        string GBCServicePath = "http://tiaapp.goyalonline.in/";
        [HttpGet]
        public string ConnectToServer(string SrvrName,string Dbname,string auth,string pass,string user,string InvoiceType)
        {
            return "";
        }
        [HttpGet]
        public List<string> Demo(string a)
        {
            List<string> ls = new List<string>();
            ls.Add("Yashu");
            ls.Add("Wardha");
            return ls;
        }       

        [HttpPost]
        public List<string> Demo_1(string a)
        {
            List<string> ls = new List<string>();
            ls.Add("Yashu...");
            ls.Add("Wardha...");
            return ls;
        }
        [HttpGet]
        public List<string> Demo_1()
        {
            List<string> ls = new List<string>();
            ls.Add("Yashu...");
            ls.Add("Wardha...");
            return ls;
        }

        [HttpGet]
        public string Ping(string PtCode)
        {
            try
            {
                Global obj = new Global();
                DataRow dr = obj.getDataRow(false, "select * from patientmaster where ptcode='" + PtCode + "'");
                if (dr == null)
                {
                    string Url = GBCServicePath + "Values/GetPtData?PtCode=" + PtCode;
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
                    request.ContentType = "application/json; charset=utf-8";
                    request.Accept = "application/json, text/javascript, */*";
                    request.Method = "GET";

                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    string json = "";
                    PatientMaster myobj;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var objText = reader.ReadToEnd();
                        myobj = (PatientMaster)js.Deserialize(objText, typeof(PatientMaster));
                        if (myobj != null)
                        {
                            string[] arr = myobj.PtName.Split(' ');
                            string fname = "", lname = "", mname = "";
                            if (arr.Length == 1)
                            {
                                fname = myobj.PtName;
                            }
                            if (arr.Length == 2)
                            {
                                fname = arr[0];
                                lname = arr[1];
                            }
                            if (arr.Length >= 3)
                            {
                                fname = arr[0];
                                mname = arr[1];
                                lname = arr[2];
                            }
                            bool result = obj.ExecuteQuery(false, "insert into patientmaster (ptcode,ptname,address,company,IniDate,ptmidname,ptlstname,firstname,PhNo,DeviceId,stcode,ctcode,fdocName,fdoccode)  values('" + myobj.PtCode + "','" + myobj.PtName + "','" + myobj.Address + "','ZZZZZZ',getdate(),'" + mname + "','" + lname + "','" + fname + "','" + myobj.PhNo + "','" + myobj.DeviceId + "','" + myobj.StCode + "','" + myobj.CtCode + "' ,'" + myobj.FDocName+ "','"+myobj.FDocCode+"')");
                        }
                    }
                    json = json.ToLower();
                }
               
                return "1";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string UpdatePatientData(string PtCode, string Ptname, string MoNo, string Email, string Add, string Stcode = "", string CtCode = "", string area = "", string DrName = "", string DrCode = "")
        {
            try
            {
                Global obj = new Global();
                DataRow dr = obj.getDataRow(false, "select * from patientmaster where ptcode='" + PtCode + "'");
                string[] arr = Ptname.Split(' ');
                string fname = "", lname = "", mname = "";
                if (arr.Length == 1)
                {
                    fname = Ptname;
                }
                if (arr.Length == 2)
                {
                    fname = arr[0];
                    lname = arr[1];
                }
                if (arr.Length >= 3)
                {
                    fname = arr[0];
                    mname = arr[1];
                    lname = arr[2];
                }
                if (dr == null)
                {
                    bool result = obj.ExecuteQuery(false, "insert into patientmaster (ptcode,ptname,address,company,IniDate,ptmidname,ptlstname,firstname,PhNo,stcode,ctcode,area,FDOCCODE,FDOCNAME,EMAIL)  values('" + PtCode + "','" + Ptname + "','" + Add + "','ZZZZZZ',getdate(),'" + mname + "','" + lname + "','" + fname + "','" + MoNo + "','" + Stcode + "','" + CtCode + "','"+area+"','"+DrCode+"','"+DrName+"','"+Email+"')");
                }
                else
                {
                    bool result = obj.ExecuteQuery(false, "update patientmaster set ptname='" + Ptname + "',address='" + Add + "',ptmidname='" + mname + "',ptlstname='" + lname + "',firstname='" + fname + "',PhNo='" + MoNo + "',stcode='" + Stcode + "',ctcode='" + CtCode + "',area='" + area + "',FDOCCODE='" + DrCode + "',FDOCNAME='" + DrName + "',EMAIL='"+Email+"' where ptcode='" + PtCode + "'");
                }
                string Url = GBCServicePath + "Values/UpdatePtData?PtCode=" + PtCode + "&Ptname=" + Ptname + "&MoNo=" + MoNo + "&Email=" + Email + "&Add=" + Add + "&Stcode=" + Stcode + "&CtCode=" + CtCode + "&area=" + area + "&DrName=" + DrName + "&DrCode=" + DrCode;
                FireUrl(Url);
                return "1";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
           
        }
        //change by vivek//

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
                            return obj_C.ServicePath + "|" + "" + "|" + "" + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + "|" + obj_C.PREPTCODE + "|";
                        }
                        else
                        {
                            return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + "|" + obj_C.PREPTCODE + "|";
                        }
                    }
                }

                else
                {
                    GBCCon con1 = new GBCCon();
                    ServiceReg obj = con1.db_SerReg.SingleOrDefault(p => p.Pcode == obj_C.PCODE);
                    if (obj == null)
                    {
                        return obj_C.ServicePath + "|" + "" + "|" + "" + "|" + obj_C.PtName + "|" + obj1.PCODE + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + "|" + "|" + obj_C.PREPTCODE + "|";
                    }
                    else
                    {
                        return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj1.PCODE + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + "|" + "|" + obj_C.PREPTCODE + "|";
                        //return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj1.ConsumerMob + "|";
                    }
                }

            }
            catch (Exception ex)
            {
                return "$" + ex.Message;
            }
        }
        //chnage by updateparty

        [HttpGet]
        public string UpdatePartyData(string PtCode, string Ptname, string MoNo, string Email, string Add, string Stcode = "", string CtCode = "", string area = "", string DrName = "", string DrCode = "")
        {

            try
            {
                using (GBCDbConn con = new GBCDbConn())
                {
                    //string pcode = GenrateCode(name);
                    //ACPMST_temp obj = new ACPMST_temp();
                    ACPMST_temp obj = con.ACPMST_temp.SingleOrDefault(p => p.PtCode == PtCode);
                    obj.Address = Add;
                    obj.Email = Email;
                    obj.PhNo = MoNo;
                    obj.PtName = Ptname;
                    //obj.PtCode = pcode;
                    //obj.RegDate = DateTime.Now.Date;
                    //obj.DeviceId = deviceId;
                    obj.StCode = Stcode;
                    obj.CtCode = CtCode;
                    obj.Area = area;
                    // obj.FDocCode = DrCode;
                    //obj.FDocName = DrName;
                    // con.DB_Acc_Temp.Add(obj);
                    con.SaveChanges();
                    return "";

                }
            }
            catch (Exception ex)
            {
                return "$" + ex.Message + ex.InnerException.Message;
            }
        }
        
        //change mobno
        [HttpGet]
        public string getDataFromMobno(string MobNo)
        {
            //GBCCon gbc_con = new GBCCon();
            GBCDbConn gbc_con = new GBCDbConn();
            List<ACPMST_temp> obj = gbc_con.ACPMST_temp.Where(p => p.PhNo == MobNo).ToList();
            if (obj.Count != 0)
            {
                ACPMST_temp o = obj.Take(1).SingleOrDefault();
                return o.PtName + "<|>" + o.Address + "<|>" + o.PtCode;
            }
            else
            {
                return "";
            }
        }
        [HttpGet]
        public string Getallparty()
        {
            //   bool isno = false;

            try
            {
                List<ACPMST> list_acpmst;
                {
                    ACPMST list = new ACPMST();
                    GBCDbConn gbc_con = new GBCDbConn();
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.PTYPE == "C").ToList();
                        string PNAME = list.PNAME;
                        string PCODE = list.PCODE;
                        string Add1 = list.Add1;
                        string ConsumerMob = list.ConsumerMob;
                        return PNAME + "|" + PCODE + "|" + Add1 + "|" + ConsumerMob;
                    }
                }


            }
            catch (Exception ex)
            {
                List<ACPMST> list_item = new List<ACPMST>();
                ACPMST im_ = new ACPMST();
                im_.PNAME = "$" + ex.Message;
                list_item.Add(im_);
                // returnist_item);
            }

            return " ConsumerMob";
        }
        //change Getdatafrom mobno1
        [HttpGet]
        public string getDataFromMobno1(string MobNo)
        {
            GBCDbConn gbc_con = new GBCDbConn();
            List<ACPMST_temp> obj = gbc_con.ACPMST_temp.Where(p => p.PhNo == MobNo).ToList();
            if (obj.Count != 0)
            {
                ACPMST_temp o = obj.Take(1).SingleOrDefault();
                return o.PtName + "<|>" + o.Address + "<|>" + o.PtCode;
            }
            else
            {
                return "";
            }
        }
        //fetch discount
        [HttpGet]
        public string get_discount(string ptcode)
        {
          //  bool isno = false;
            try
            {
                List<ACPMST> list_acpmst;
                {
                    ACPMST list = new ACPMST();
                    GBCDbConn gbc_con = new GBCDbConn();
                    {
                        list_acpmst = gbc_con.Db_acpmst.Where(p => p.PCODE == ptcode).ToList();
                        ACPMST o = list_acpmst.Take(1).SingleOrDefault();                       
                        string discount = (o.discount).ToString();
                        string ConsumerMob = list.ConsumerMob;
                        return discount;
                    }
                }


            }
            catch (Exception ex)
            {
                List<ACPMST> list_item = new List<ACPMST>();
                ACPMST im_ = new ACPMST();
                im_.PNAME = "$" + ex.Message;
                list_item.Add(im_);
                // returnist_item);
            }

            return " ConsumerMob";
        }
        //change regcode1
        [HttpGet]
        public string getDataFromRegCode1(string code)
        {
            GBCDbConn gbc_con = new GBCDbConn();
            List<ACPMST_temp> obj = gbc_con.ACPMST_temp.Where(p => p.PtCode == code).ToList();
            if (obj.Count != 0)
            {
                ACPMST_temp o = obj.Take(1).SingleOrDefault();
                return o.PtName + "<|>" + o.Address + "<|>" + o.PtCode + "<|>" + o.PhNo;
            }
            else
            {
                return "";
            }
        }

        //chen mobumber//
        [HttpGet]
        public string CheckNo(string Mo)
        {
            try
            {
                using (GBCDbConn con = new GBCDbConn())
                {
                    List<ACPMST_temp> obj = con.ACPMST_temp.Where(p => p.PhNo == Mo).ToList();
                    if (obj.Count == 0)
                    {
                        return "OK";
                    }
                    return "@";
                }
            }
            catch (Exception ex)
            {
                return "$" + ex.Message + ex.InnerException.Message; ;
            }
        }
        //Register party
        [HttpGet]
        public string RegisterCustmoer(string name, string Add, string email, string phone, string Pass, string AppType, string PCODE, string Stcode = "", string CtCode = "", string area = "", string deviceId = "", string DrName = "", string DrCode = "")
        {
            try
            {
                using (GBCDbConn con = new GBCDbConn())
                {
                    string pcode = GenrateCode(name);
                    ACPMST_temp obj = new ACPMST_temp();
                    obj.Address = Add;
                    obj.Email = email;
                    obj.PhNo = phone;
                    obj.PtName = name;
                    obj.PtCode = pcode;
                    obj.RegDate = DateTime.Now.Date;
                    obj.DeviceId = deviceId;
                    obj.StCode = Stcode;
                    obj.CtCode = CtCode;
                    obj.Area = area;
                    obj.PCODE = PCODE;

                    // obj.FDocCode = DrCode;
                    //obj.FDocName = DrName;
                    con.ACPMST_temp.Add(obj);
                    con.SaveChanges();
                    string msg = "Welcome To TiaER@App!\n Your UserId/Registration Code :" + pcode;
                    if (AppType == "$")
                    {
                        msg = "Welcome!\n Your UserId/Registration Code :" + pcode;
                    }
                   // GetSMSUrl(phone, msg, AppType, PCODE);
                    return pcode;
                }
            }
            catch (Exception ex)
            {
                return "$" + ex.Message + ex.InnerException.Message;
            }
        }
        //change acmpstdiscount
        [HttpGet]
        public string GenrateCode(string Pname)
        {
            string pcode = "";
            //if (Pname.Length >= 3)
            //{
            //    Pname = Pname.Replace(" ", "").Replace("'", "");
            //}
            //if (Pname.Length == 1)
            //{
            //    Pname = Pname + "00";
            //}
            //else if (Pname.Length == 2)
            //{
            //    Pname = Pname + "0";
            //}
            //Pname = Pname.Substring(0, 3);
            //DataTable dt = GetPCode("left(PCODE,3)", Pname);
            //if (dt.Rows.Count != 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    pcode = "00" + (Convert.ToInt16(dr["NUM"]) + 1).ToString();
            //    pcode = Pname + pcode.Substring(pcode.Length - 3, 3);

            //}
            DateTime Today = DateTime.Now.Date;
            string year = Today.ToString("yyyy");
            Pname = Pname.Substring(0, 3);
            string Pname1 = year + Pname;
            //  DataTable dt = new DataTable();
            DataTable dt = GetPCode(Pname1);
            if (dt.Rows[0][0].ToString() != "")
            {
                string eno = dt.Rows[0][0].ToString();
                int eno_no = Convert.ToInt32(eno);
                int codeno = eno_no + 1;
                string Fno = codeno.ToString();
                for (int j = 0; j < 4 - codeno.ToString().Length; j++)
                {
                    Fno = "0" + Fno;
                }
                pcode =2017+ Pname + Fno;
            }
            else
            {
                pcode = 2017 + Pname + "0001";
            }
            return pcode;



        }
        public DataTable GetPCode(string Pname)
        {
            try
            {
                // string SQL = "Select Isnull(Max(right(PCODE,3)),0) as Num from TiaERPAppReg where " + left + "='" + strname + "' And IsNumeric(Right(pcode, 3)) > 0";
                string SQL = "select MAX(Substring(PtCode,8,10)) from ACPMST_temp where  PtCode like '%" + Pname + "%'";
                return objdal.getDataNew_code(SQL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        
     
        //changeup//

        [HttpGet]
        public void UpdateArea(string acode, string Ptcode)
        {
            Global obj = new Global();
            bool result = obj.ExecuteQuery(false, "update patientmaster set area='" + acode + "' where ptcode='" + Ptcode + "'");
            string Url = GBCServicePath + "Values/UpdatePtArea?PtCode=" + Ptcode + "&area=" + acode;
            FireUrl(Url);
        }

        public string FireUrl(string Url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "get";

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                string json = "";

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        json += reader.ReadLine();
                    }
                }
                return json;
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public string ExecuteUrl(string url)
        {
            int cnt = 5;
        a: try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                string json = "";

                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        json += reader.ReadLine();
                    }
                }
                return json;
            }
            catch
            {
                if (cnt <= 5)
                {
                    cnt++;
                    goto a;
                }
                return "";
            }
        }
        
        [HttpGet]
        public string GetCodeName(string st, string ct, string ar)
        {
            DbConn con = new DbConn();
            string scode, ccode, acode;
            try
            {
                scode = con.Db_state.SingleOrDefault(p => p.StCode == st).StName;
            }
            catch
            {
                scode = st;
            }
            try
            {
                ccode = con.Db_city.SingleOrDefault(p => p.CtCode == ct).CtName;
            }
            catch
            {
                ccode = ct;
            }
            try
            {
                acode = con.Db_area.SingleOrDefault(p => p.aCode == ar).aName;
            }
            catch
            {
                acode = ar;
            }
            double? charges = 0;

            try
            {
                charges = con.Db_area.SingleOrDefault(p => p.aCode == ar).Charges;
            }
            catch
            {
                charges = 0;
            }
            return scode + "|" + ccode + "|" + acode + "|" + charges;
        }



        [HttpGet]
        public HttpResponseMessage GetSaleRpt(DateTime fromdate, DateTime Todate, string pcode, string ptcode, string vrtype)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    DbConn con = new DbConn();
                    GBCDbConn gbc_con = new GBCDbConn();
                    List<VC> list_vc = gbc_con.Db_vc.ToList();
                    list_vc = gbc_con.Db_vc.Where(p => p.FDNAME == "SALE").ToList();
                    string series = "";
                    string name = gbc_con.Database.Connection.Database + ".dbo.";
                    foreach (var item in list_vc)
                    {
                        series = series + ",'" + item.SER + "'";
                    }
                    series = series.Substring(1, series.Length - 1);
                    series = "(" + series + ")";
                    List<SaleReport> list_sale = new List<SaleReport>();


                    list_sale = con.Database.SqlQuery<SaleReport>("EXEC Sp_PartyWiseSaleBill @FromDate={0},@ToDate={1},@IsAllSer={2},@Series={3},@PCode={4},@PtCode={5},@RegdId={6},@VrType={7},@Less={8},@GdnCode={9},@OrderBy={10},@Summary={11},@ChkDr={12},@DrCode={13},@TSelectedNode={14},@FromVr={15},@ToVr={16},@GBCDBName={17}", fromdate, Todate, true, series, pcode, ptcode, "", vrtype, false, "", "Vrdate", false, false, "", "ACC070", "", "", name).Take(30).ToList();

                    list_sale = list_sale.OrderByDescending(p => p.Vrdate).OrderByDescending(p => p.Vrno).Take(30).ToList();
                    double crTotal = 0, cashTotal = 0;
                    foreach (var item in list_sale)
                    {
                        item.Vrdt = item.Vrdate.ToString("dd/MM/yyyy");
                        if (item.VrType == "C")
                        {
                            item.cashAmt = item.NetAmt.ToString("0.00");
                            item.crAmt = "0.00";
                            cashTotal = cashTotal + item.NetAmt;
                        }
                        else
                        {
                            item.crAmt = item.NetAmt.ToString("0.00");
                            item.cashAmt = "0.00";
                            crTotal = crTotal + item.NetAmt;
                        }
                    }

                    SaleReport obj = new SaleReport();
                    obj.Vrno = "Total";
                    obj.cashAmt = cashTotal.ToString("0.00");
                    obj.crAmt = crTotal.ToString("0.00");
                    list_sale.Add(obj);

                    return Request.CreateResponse(HttpStatusCode.OK, list_sale);

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
        public HttpResponseMessage GetMISReport(DateTime TDate)
        {
            if ((new Global()).CheckStarterService())
            {
                try
                {
                    DbConn con = new DbConn();
                    GBCDbConn gbc_con = new GBCDbConn();
                    string name = gbc_con.Database.Connection.Database + ".dbo.";
                    string CurDate;
                    string ToDate;
                    if (TDate == null)
                    {
                        ToDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        CurDate = DateTime.Now.Date.ToString("yyyy-MM");
                    }
                    else
                    {
                        ToDate = TDate.ToString("yyyy-MM-dd");
                        CurDate = TDate.ToString("yyyy-MM");
                    }
                    List<MISReport> li_MIS = con.Database.SqlQuery<MISReport>("EXEC Sp_MISReport @Curdate={0},@TDate={1},@GBCDbName={2}", CurDate, ToDate, name).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, li_MIS);
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
        public string UpDateDeviceId(string PtCode, string DeviceId)
        {
            try
            {
                Global obj = new Global();
                bool result = obj.ExecuteQuery(false, "update patientmaster set DeviceId ='" + DeviceId + "' where ptcode='" + PtCode + "'");
                return "";
            }
            catch
            {
                return "$";
            }
        }

        [HttpGet]
        public HttpResponseMessage SaveAcpmst(string Ptype, string Pname, string area, string MobNo, string add)
        {
            try
            {
                GBCDbConn gbc_con = new GBCDbConn();
                ACPMST obj = new ACPMST();
                obj.PCODE = GEtPCOde(Pname);
                if (Ptype == "C")
                {
                    obj.SCODE = "AB";
                }
                else
                {
                    obj.SCODE = "LB";
                }
                obj.PNAME = Pname;
                obj.PTYPE = Ptype;
                obj.areacode = area;
                obj.Add1 = add;
                obj.ConsumerMob = MobNo;
                gbc_con.Db_acpmst.Add(obj);
                gbc_con.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, obj.PCODE);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "$Error occurred while saving data " + ex.Message);
            }
        }
        public string GEtPCOde(string name)
        {
            string strname = name, GetPCode;
            if (name.Length >= 3)
            {
                strname = name.Replace(" ", "");
                strname = strname.Replace("'", "");
                strname = strname.Substring(0, 3);
                if (strname.Length == 2)
                {
                    strname = strname + "0";
                }
            }
            else if (name.Length == 2)
            {
                strname = strname + "0";
            }
            else
            {
                strname = strname + "00";
            }
            DataSet dsAcpmst = (new TIA3T.NEW.BLL.ClsAcpmst()).GetPCode("left(PCODE,3)", strname);
            if (dsAcpmst.Tables["Tab_Emp"].Rows.Count != 0)
            {
                DataRow dr_Emp = dsAcpmst.Tables["Tab_Emp"].Rows[0];
                GetPCode = "00" + (Convert.ToInt16(dr_Emp["NUM"]) + 1).ToString();
                GetPCode = GetPCode.Substring(GetPCode.Length - 3, 3);
            }
            else
            {
                GetPCode = "";
            }

            return strname + GetPCode;
        }

        [HttpGet]
        public void SendLedgerEmail(string pcode, DateTime frmdate, DateTime todate, bool IsOPBalInclude, bool IsSummary, string ToEmail, string Subject, string MsgText, string Footer, string OPBal)
        {
            OrderController ord = new OrderController();
            Global objG = new Global();
            List<PartyLdgr> list_partyLdgr = new List<PartyLdgr>();
            list_partyLdgr = ord.GetPartyLedger(pcode, frmdate, todate, IsOPBalInclude, IsSummary);
            string sql = "select Pname from acpmst where pcode='" + pcode + "'";
            DataRow dr = objG.getDataRow(true, sql);
            string pname = "", Coname = "", coAdd = "";
            if (dr != null)
            {
                pname = dr["Pname"].ToString();
            }
            sql = "select * from multiplefirm";
            dr = objG.getDataRow(false, sql);
            if (dr != null)
            {
                Coname = dr["coname"].ToString();
                coAdd = dr["coaddr"].ToString();
            }
            string MSG;
            string TBL = "", TR_MidRow = "", TR_LstRow = "", LBL_OpBal = "<label style='font-size:20px;color:#1390cf'>OpBal : " + OPBal + " </label><br />";
            for (int i = 0; i <= list_partyLdgr.Count-1; i++)
            {
                var item = list_partyLdgr[i];
                string balancestr;
                if ((Convert.ToDecimal(item.Balance)) > 0)
                {
                    balancestr = Math.Round(Convert.ToDecimal(item.Balance)) + "Dr";
                }
                else
                {
                    balancestr = Math.Round(Convert.ToDecimal(item.Balance)) + "Cr";
                }
                if (IsSummary)
                {
                    TBL = "";
                    decimal CrAmt,DrAmt;
                    CrAmt = Math.Round(Convert.ToDecimal(list_partyLdgr[list_partyLdgr.Count - 1].cramt), 2);
                    DrAmt = Math.Round(Convert.ToDecimal(list_partyLdgr[list_partyLdgr.Count - 1].dramt), 2);
                    TBL = "<br />" +
                        "<label style='font-size:20px;color:#1390cf'>DrAmt : " + DrAmt + " </label><br />" +
                        "<label style='font-size:20px;color:#1390cf'>CrAmt : " + CrAmt + " </label><br />" +
                        "<label style='font-size:20px;color:#1390cf'>Balance : " + balancestr + " </label><br />";

                }
                else
                {
                    if (i == list_partyLdgr.Count - 1)
                    {
                        TR_LstRow = "<tr style='border:groove'><td colspan='2' style='color:purple;font-weight:bold;border:groove'><label style='font-size:18px;'>" + item.Description + "</label> </td><td style='color:purple;font-weight:bold; text-align:right;width:70px;border:groove'><label style='font-size:18px;'>" + Math.Round(Convert.ToDecimal(item.dramt), 2) + "</label></td><td  style='color:purple;font-weight: bold; text-align:right;width:70px;border:groove'><label style='font-size:18px;'>" + Math.Round(Convert.ToDecimal(item.cramt), 2) + "</label></td><td style='color:purple;font-weight:bold; text-align:right;width:70px;border:groove'><label style='font-size:18px;'>" + balancestr.Replace("-", "") + "</label></td></tr></tbody>";
                    }
                    else
                    {
                        TR_MidRow = TR_MidRow + "<tr style='border:groove'><td style='width:50px;border:groove'><label style='font-size:18px;font-weight:normal'>" + item.Dt + "</label> </td> <td  style='text-align:justify;width:600px;border:groove'><label style='font-size:18px;font-weight:normal'>" + item.Description + "</label></td><td style=' text-align:right;border:groove'><label style='font-size:18px;font-weight:normal'>" + Math.Round(Convert.ToDecimal(item.dramt), 2) + "</label></td><td style=' text-align:right;border:groove'><label style='font-size:18px;font-weight:normal'>" + Math.Round(Convert.ToDecimal(item.cramt), 2) + "</label></td><td style=' text-align:right;border:groove'><label style='font-size:18px;font-weight:normal'>" + balancestr.Replace("-", "") + "</label></td></tr>";
                    }
                    TBL = "<table  style='border-collapse:collapse;border:groove;font-size:18px'>" +
                        "<tbody>" +
                        "<tr style='border:groove;background-color:#1390cf;color:white'><td style='width:50px;border:groove'><label style='font-size:18px;'>Dt</label> </td><td style='border:groove'> <label style='font-size:18px;'>Description</label></td><td style='border:groove'> <label style='font-size:18px;'>DrAmt</label></td><td style='border:groove'> <label style='font-size:18px;'>CrAmt</label></td><td style='border:groove'> <label style='font-size:18px;'>Balance</label></td></tr>" +
                        TR_MidRow +
                        TR_LstRow +
                    "</table>";
                }
            }

            MSG = "<div style='border:groove;width:930px;padding:30px'>" +
                    "<center>" +
                    "<label style='font-size:22px;color:#1390cf'>" + Coname + "</label><br />" +
                    "<label style='font-size:16px;font-weight:normal;width:450px;'>" + coAdd + "</label><br />" +
                    "<label style='font-size:20px;color:#1390cf'>" + pname + " [" + pcode + "]</label><br />" +
                    "<label style='font-size:18px;font-weight:normal'>Ledger From Date: " + frmdate + " To Date : " + todate + "</label><br />" +
                    LBL_OpBal +
                    TBL +
                    "</center>" +
                    "</div>";
            MSG = MsgText + "<br /><br />" + MSG + "<br /><br />" + Footer;
            SendEmail(ToEmail, Subject, MSG, false, "");
        }


        [HttpGet]
        public void SendEmail(string ToEmail,string Subject, string Body, bool IsAttach, string FilePath)
        {
            try
            {
                Global objG = new Global();
                string sql = "select * from emailsetup where isnull(port,'')<>'' and isnull(emailid,'')<>'' and isnull(password,'') <> ''";
                DataRow dr = objG.getDataRow(false, sql);
                if (dr != null)
                {
                    string FromEmail =  dr["EMAILID"].ToString();
                    string pass =  dr["PASSWORD"].ToString();
                    if (FromEmail.Trim() != "" && pass.Trim() != "")
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                        mail.From = new MailAddress(FromEmail);
                        mail.IsBodyHtml = true;
                        mail.To.Add(ToEmail);
                        mail.Subject = Subject;
                        mail.Body = Body;

                        if (IsAttach)
                        {
                            System.Net.Mail.Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(FilePath);
                            mail.Attachments.Add(attachment);
                        }

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(FromEmail, pass);
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [HttpGet]
        public string GetDefaultMailSetting(string Pcode)
        {
            string footer = "", Email = "";
            Global ObjGbl = new Global();
            string sql = "select footerSign from emailsetup";
            DataRow dr = ObjGbl.getDataRow(false, sql);
            if (dr != null)
            {
                footer = (dr["footerSign"] == null) ? "Thank You" : dr["footerSign"].ToString();
                sql = "select email from acpmst where pcode='" + Pcode + "' ";
                dr = ObjGbl.getDataRow(true, sql);
                if (dr != null)
                {
                    Email = (dr["email"] == null) ? "" : dr["email"].ToString();
                }
            }
            return Email + "|" + footer;
        }
      
    }
}
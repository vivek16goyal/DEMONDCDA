using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using GBCService.Models;

namespace GBCService.Controllers
{
    public class ValuesController : ApiController
    {
        DAL objdal = new DAL();
        [HttpGet]
        public string CheckService()
        {
            return "Ok Fine!!";
        }

        [HttpGet]
        public ServiceReg GetData(string Pcode)
        {
            try
            {
                ServiceReg obj;
                using (GBCCon con = new GBCCon())
                {
                     obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public string UpdatePartyInfo(string Pcode, string ServicePath, string InetAddress, string Port, string MCAddress, DateTime? ServiceLastActiveDate,string Database ,string ServerName,string Auth ,string userId,string Pass)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    obj.ServicePath = ServicePath;
                    obj.InetAddress = InetAddress;
                    obj.Port = Port;
                    obj.MCAddress = MCAddress;
                    obj.ServiceLastActiveDate = DateTime.Now;
                    obj.SqlServerAuth = (Auth == "1") ? true : false;
                    obj.SqlPassword = Pass;
                    obj.SqlUserId = userId;
                    obj.DataBaseName = Database;
                    obj.SqlServerName = ServerName;
                    con.SaveChanges();
                    return "1";
                }
            }
            catch
            {
                return "0";
            }
        }
       
        [HttpGet]
        public string UpdatePartyInfoFromApp(string Pcode)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    obj.PingCount = String.IsNullOrEmpty(obj.PingCount.ToString()) ? 1 : obj.PingCount + 1;
                    obj.AppLastActiveDate = DateTime.Now;
                    con.SaveChanges();
                    return "1";
                }
            }
            catch
            {
                return "0";
            }
        }
       
        [HttpGet]
        public string UpdateDataBaseConnecton(string Pcode,bool status)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    obj.IsDBConnected = status; 
                    con.SaveChanges();
                    return "1";
                }
            }
            catch
            {
                return "0";
            }
        }

        [HttpGet]
        public string GetDataBaseConnecton(string Pcode)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    if (Convert.ToBoolean(obj.IsDBConnected))
                    { return "1"; }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch
            {
                return "0";
            }
        }

        [HttpGet]
        public string StopService(string pcode)
        {
            GBCCon con = new GBCCon();
            ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == pcode);
            try
            {
                obj.ServiceStatus = false;
                con.SaveChanges();
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        [HttpGet]
        public string StartService(string pcode)
        {
            GBCCon con = new GBCCon();
            ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == pcode);
            try
            {
                obj.ServiceStatus = true;
                con.SaveChanges();
                return "1";
            }
            catch
            {
                return "0";
            }
        }

        [HttpGet]
        public ServiceReg CheckPcode(string pcode)
        {
            using (GBCCon con = new GBCCon())
            {
                DateTime date = DateTime.Now.Date;
                ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == pcode); 
                return obj;
            }
        }

        [HttpGet]
        public string PingGBCService()
        {
            return "1";
        }

        [HttpGet]
        public HttpResponseMessage getServiceStatus(string pcode, string AppType = "")
        {
            string result;
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    pcode = pcode.Trim();
                    DateTime date = DateTime.Now.Date;
                    ServiceReg obj;
                    obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == pcode);
                    if (obj == null)
                    {
                        result = "*";
                    }
                    else
                    {
                        if (obj.IsActive == true && obj.ValidUpToDate >= date && obj.ServiceStatus == true)
                        {
                            if (AppType == "")
                            {
                                result = obj.ServicePath + "<|>" + obj.UserId + "<|>" + obj.Password + "<|>" + obj.FDName + "<|>" + obj.PType + "<|>" + "";
                            }
                            else
                            {
                                result = obj.Pcode + "<|>" + obj.Name + "<|>" + obj.ServicePath;
                            }
                        }
                        else
                        {
                            if (obj.IsActive == true)
                            {
                                if (obj.ValidUpToDate >= date)
                                {
                                    if (obj.ServiceStatus == true)
                                    {
                                        if (AppType == "")
                                        {
                                            result = obj.ServicePath + "<|>" + obj.UserId + "<|>" + obj.Password + "<|>" + obj.FDName + "<|>" + obj.PType + "<|>" + "";
                                        }
                                        else
                                        {
                                            result = obj.Pcode + "<|>" + obj.Name + "<|>" + obj.ServicePath;
                                        }
                                    }
                                    else
                                    {
                                        if (AppType == "")
                                        {
                                            result = obj.ServicePath + "<|>" + obj.UserId + "<|>" + obj.Password + "<|>" + obj.FDName + "<|>" + obj.PType + "<|>" + "#";
                                        }
                                        else
                                        {
                                            result = "#";
                                        }
                                         // Service stop
                                    }
                                }
                                else
                                {
                                    result = "%"; //licence expire
                                }
                            }
                            else
                            {
                                result = "$";//Deactive
                            }
                        }

                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is some problem while getting information from server. Please try after some time." + ex.Message);
            }
        }

        [HttpGet]
        public string RegisterCustmoer(string name, string Add, string email, string phone, string Pass, string AppType, string PCODE, string Stcode = "", string CtCode = "", string area = "", string deviceId = "",string DrName="", string DrCode="")
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    string pcode = GetPtCode();
                    PatientMaster obj = new PatientMaster();
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
                    obj.FDocCode = DrCode;
                    obj.FDocName = DrName;
                    con.db_patient.Add(obj);
                    con.SaveChanges();
                    string msg = "Welcome To TiaER@App!\n Your UserId/Registration Code :" + pcode;
                    if (AppType == "$")
                    {
                        msg = "Welcome!\n Your UserId/Registration Code :" + pcode;
                    }
                    GetSMSUrl(phone, msg, AppType, PCODE);
                    return pcode;
                }
            }
            catch(Exception ex)
            {
                return "$" + ex.Message + ex.InnerException.Message ;
            }
        }

        [HttpGet]
        public string GenrateCode(string Pname)
        {
            string pcode="";
            if (Pname.Length >= 3)
            {
                Pname = Pname.Replace(" ", "").Replace("'", "");
            }
            if (Pname.Length == 1)
            {
                Pname = Pname + "00";
            }else if(Pname.Length == 2)
            {
                Pname = Pname + "0";
            }
            Pname = Pname.Substring(0, 3);
            DataTable dt = GetPCode("left(PCODE,3)", Pname);
            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                pcode = "00" + (Convert.ToInt16(dr["NUM"]) + 1).ToString();
                pcode = Pname + pcode.Substring(pcode.Length - 3, 3);
            }
            return pcode;
        }


        private string GetPtCode()
        {
            string functionReturnValue = null;
            DataTable dt_;
            DateTime Today = DateTime.Now.Date;
            try
            {
                string Code = "";
                DataRow dr = default(DataRow);
                Code = Code + Today.ToString("yyyy") + GetMonthCode() + Today.ToString("dd");
                dt_ = GetPTCode(Code);
                if (dt_.Rows.Count != 0)
                {
                    dr = dt_.Rows[0];
                    string str = "000000" + Convert.ToString(dr["Num"]);
                    functionReturnValue = Code + str.Substring(str.Length - 5, 5);
                }
                else
                {
                    return null;
                }
                return functionReturnValue;

            }
            catch 
            {
                return null;
            }
        }

        public string GetMonthCode()
        {
            string month= DateTime.Now.Date.ToString("MM");
            switch (month)
            {
                case "01":
                    return "J";
                case "02":
                    return "K";
                case "03":
                    return "L";
                case "04":
                    return "A";
                case "05":
                    return "B";
                case "06":
                    return "C";
                case "07":
                    return "D";
                case "08":
                    return "E";
                case "09":
                    return "F";
                case "10":
                    return "G";
                case "11":
                    return "H";
                case "12":
                    return "I";
                default:
                    return "M";
            }
        }

        public DataTable GetPCode(string left, string strname)
        {
            try
            {
                string SQL = "Select Isnull(Max(right(PCODE,3)),0) as Num from TiaERPAppReg where " + left + "='" + strname + "' And IsNumeric(Right(pcode, 3)) > 0";
                return objdal.getDataTable(SQL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable GetPTCode(string code)
        {
            try
            {
                string SQL = "select Isnull(Max(right(PtCode,3)),0)+1 as Num from PatientMaster  where left(PtCode,7)='" + code + "'";
                return objdal.getDataTable(SQL);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public string CheckNo(string Mo)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                   List<PatientMaster> obj = con.db_patient.Where(p => p.PhNo == Mo).ToList();
                    if (obj.Count == 0)
                    {
                        return "OK";
                    }
                    return "@";
                }
            }
            catch(Exception ex)
            {
                return "$" + ex.Message + ex.InnerException.Message; ;
            }
        }

        [HttpGet]
        public string GetSMSUrl(string mono, string msg, string AppType, string Pcode)
        {
            GBCCon con = new GBCCon();
            string userId, senderName, passW;
            string servicePath = "";
            userId = System.Configuration.ConfigurationManager.AppSettings["SMSUserName"].ToString();
            senderName = System.Configuration.ConfigurationManager.AppSettings["SMSSendername"].ToString();
            passW = System.Configuration.ConfigurationManager.AppSettings["SMSPass"].ToString();
            string PartyUrl = "";
            string url = "http://sms.goyalonline.in/SMS_API/sendsms.php?username=<<U>>&password=<<P>>&mobile=<<SNDTO>>&message=<<MSG>>&sendername=<<N>>&routetype=1";
            if (AppType == "$")
            {
                try
                {
                    servicePath = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode).ServicePath;
                    PartyUrl = ExecuteUrl(servicePath + "/Connection/GetSMSUrl?MSG=" + msg + "&PhNo=" + mono);
                }
                catch
                {
                    PartyUrl = "";
                }
            }
            if (PartyUrl != "")
            {
                if (PartyUrl.ToLower().Contains("successfully"))
                {
                    AppSMSStatus objSms = new AppSMSStatus();
                    objSms.MobileNo = mono;
                    objSms.Msg = msg;
                    objSms.Status = "Sent";
                    con.db_AppSms.Add(objSms);
                    con.SaveChanges();
                    return "1";
                }
                else
                {
                    //PartyUrl = PartyUrl.Substring(1, PartyUrl.Length - 1);
                    //PartyUrl = PartyUrl.Remove(PartyUrl.Length - 1, 1);
                    //url = PartyUrl;
                }
            }
            url = url.Replace("<<U>>", userId).Replace("<<P>>", passW).Replace("<<N>>", senderName).Replace("<<SNDTO>>", mono).Replace("<<MSG>>", msg);
            return FireUrl(url, mono, msg, AppType, Pcode);
            //return url;

        }

        public string FireUrl(string url, string mono, string msg, string AppType, string Pcode)
        {
            try
            {
                string data = "0";
                string json = "";
                json = ExecuteUrl(url);
                if (json.ToLower().Contains("successfully"))
                {
                    data = "1";
                }
                else
                {
                    data = json;
                }
                GBCCon con = new GBCCon();

                if (json == "")
                {
                    try
                    {
                        string servicePath = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode).ServicePath;
                        json = ExecuteUrl(servicePath + "/Values/ExecuteUrlSMS?url=" + url);
                        if (json.ToLower().Contains("successfully"))
                        {
                            data = "1";
                        }
                        else
                        {
                            data = json;
                        }
                    }
                    catch { }
                }

                AppSMSStatus objSms = new AppSMSStatus();
                objSms.MobileNo = mono;
                objSms.Msg = msg;
                objSms.Status = json;
                con.db_AppSms.Add(objSms);
                con.SaveChanges();
                return data;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        public string PingGBCService(string PCode)
        {
            GBCCon con = new GBCCon();
            ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == PCode);
            string ServicePath="",Pingurl="";
            if (obj != null)
            {
                ServicePath = obj.ServicePath;
                Pingurl = ExecuteUrl(ServicePath + "/Connection/Ping");
                if (Pingurl != "")
                {
                    obj.ServiceStatus = true;
                    Pingurl = "1";
                }
                else
                {
                    obj.ServiceStatus = false;
                    Pingurl = "0";
                }
            }
            else
            {
                obj.ServiceStatus = false;
                Pingurl = "0";
            }
            con.SaveChanges();
            return Pingurl;
        }

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
        public List<ServiceReg> GetActiveSupplier(string Pcode="")
        {
            try
            {
                GBCCon con = new GBCCon();
                List<ServiceReg> obj = null;
                if (Pcode == "")
                {
                    obj = con.db_SerReg.Where(p => p.PType == "S" && p.IsDBConnected == true && p.IsActive == true && p.ServiceStatus == true).ToList();
                }
                else
                {
                    obj = con.db_SerReg.Where(p => p.Pcode == Pcode).ToList();
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }
        [HttpGet]
        public string UpDateDeviceId(string PtCode,string DeviceId)
        {
            try
            {
                GBCCon con = new GBCCon();
                PatientMaster obj = con.db_patient.SingleOrDefault(p => p.PtCode == PtCode);
                if (obj != null)
                {
                    obj.DeviceId = DeviceId;
                }
                con.SaveChanges();
                return "";
            }
            catch
            {
                return "$";
            }
        }
        [HttpGet]
        public string UpdateCustomer(string PCode,string ClientCode)
        {
            try
            {
                GBCCon con = new GBCCon();
                ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == PCode);
                PatientMaster obj_C = con.db_patient.SingleOrDefault(p => p.PtCode == ClientCode);
                obj_C.ServicePath = obj.ServicePath ;
                obj_C.PCODE = PCode;
                con.SaveChanges();
                string str = obj.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "| " + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area;
                return str;
            }
            catch(Exception ex)
            {
                return "$ " + ex.Message + " " + ex.InnerException.Message;
            }
        }

        [HttpGet]
        public string CheckPatient(string PtCode)
        {
            try
            {
                GBCCon con = new GBCCon();
                PatientMaster obj_C = con.db_patient.SingleOrDefault(p => p.PtCode == PtCode);
                if (obj_C == null)
                {
                    return "0";
                }
                else
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == obj_C.PCODE);
                    if (obj == null)
                    {
                        return obj_C.ServicePath + "|" + "" + "|" + "" + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + obj_C.FDocName + "|" + obj_C.FDocCode;
                    }
                    else
                    {
                        return obj_C.ServicePath + "|" + obj.Name + "|" + obj.Pcode + "|" + obj_C.PtName + "|" + obj_C.PtCode + "|" + obj_C.PhNo + "|" + obj_C.Email + "|" + obj_C.Address + "|" + obj_C.DeviceId + "|" + obj_C.StCode + "|" + obj_C.CtCode + "|" + obj_C.Area + "|" + obj_C.FDocName + "|" + obj_C.FDocCode;
                    }
                }
            }
            catch(Exception ex)
            {
                return "$"+ ex.Message;
            }
        }

        [HttpGet]
        public PatientMaster GetPtData(string PtCode)
        {
            try
            {
                GBCCon con = new GBCCon();
                PatientMaster obj = con.db_patient.SingleOrDefault(p => p.PtCode == PtCode);
                return obj;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public string UpdatePtData(string PtCode, string Ptname, string MoNo, string Email, string Add, string Stcode = "", string CtCode = "", string area = "", string DrName = "", string DrCode = "")
        {
            try
            {
                GBCCon con = new GBCCon();
                PatientMaster obj = con.db_patient.SingleOrDefault(p => p.PtCode == PtCode);
                obj.PtName = Ptname;
                obj.PhNo = MoNo;
                obj.Email = Email;
                obj.Address = Add;
                obj.StCode = Stcode;
                obj.CtCode = CtCode;
                obj.Area = area;
                obj.FDocCode = DrCode;
                obj.FDocName = DrName;
                con.SaveChanges();
                return "";
            }
            catch
            {
                return "$";
            }
        }
        [HttpGet]
        public string UpdatePtArea(string PtCode,  string area )
        {
            try
            {
                GBCCon con = new GBCCon();
                PatientMaster obj = con.db_patient.SingleOrDefault(p => p.PtCode == PtCode);            
                obj.Area = area;
                con.SaveChanges();
                return "";
            }
            catch
            {
                return "$";
            }
        }
        [HttpPost]
        public string Save([FromBody] Order m)
        {
            try
            {
                Global obj = new Global();
                m.series = "GC";
                string vrno = obj.SaveOrder(m, "GORC");
                return vrno;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpGet]
        public List<Order1> Order1_history(string PtCode, string AppType,string PCode ="")
        {
            string SerPath;
            GBCCon gbc_con = new GBCCon();
            List<OB> list_ord;
            if (PCode == "")
            {
                list_ord = gbc_con.db_ob.Where(p => p.FromUserId == PtCode && p.AppType == AppType).OrderByDescending(p => p.VrDate).ToList();
            }
            else
            {
                list_ord = gbc_con.db_ob.Where(p => p.FromUserId == PtCode && p.AppType == AppType && p.ToUserId == PCode).OrderByDescending(p => p.VrDate).ToList();
            }
            List<Order1> ord = new List<Order1>();
            foreach (var item in list_ord)
            {
                Order1 obj_ord = new Order1();
                obj_ord.NameP = gbc_con.db_patient.SingleOrDefault(p => p.PtCode == PtCode).PtName;
                SerPath = gbc_con.db_SerReg.SingleOrDefault(p => p.Pcode == item.ToUserId).ServicePath;
                obj_ord.pcode = gbc_con.db_SerReg.SingleOrDefault(p => p.Pcode == item.ToUserId).Name;
                obj_ord.TotalAmt = item.OrdAmt.ToString();
                obj_ord.vrdate = Convert.ToDateTime(item.VrDate).ToString("dd/MM/yyyy");
                obj_ord.vrno = item.VrNo;
                obj_ord.imgName = item.ImgName;
                List<ItemMaster> li_Oi = new List<ItemMaster>();
                List<OI> OI = gbc_con.db_oi.Where(p => p.VrId == item.VrId).ToList();
                foreach (var oi_item in OI)
                {
                    try
                    {
                        ItemMaster obj_oi = new ItemMaster();
                        obj_oi.INAME = oi_item.IName;
                        obj_oi.Qty = oi_item.OrdQty.ToString();
                        obj_oi.free = oi_item.FreeQty.ToString();
                        obj_oi.Rate = oi_item.Rate.ToString();
                        obj_oi.ICODE = oi_item.Value.ToString();
                        li_Oi.Add(obj_oi);
                    }
                    catch
                    {
                    }
                }
                obj_ord.items = li_Oi;
                obj_ord.status = GetStatus(obj_ord.vrno, SerPath);
                ord.Add(obj_ord);
            }
            ord = ord.OrderByDescending(p => p.vrdate).OrderByDescending(p => p.vrno).ToList();
            return ord;
        }

        public string GetStatus(string vrno,string SerPath)
        {
            string url = SerPath + "/Order/GetOrdStatus?Vrno=" + vrno;
            string Status = ExecuteUrl(url);
            if (Status.Contains("OPL"))
            {
                Status = "Order Placed";
            }
            else if (Status.Contains("BEP"))
            {
                Status = "Bill Prepared";
            }
            else if (Status.Contains("DIS"))
            {
                Status = "Dispatched";
            }
            else if (Status.Contains("DLV"))
            {
                Status = "Delivered";
            }
            else if (Status.Contains("CNL"))
            {
                Status = "Canceled";
            }
            else if (Status.Contains("RCD"))
            {
                Status = "Order Received";
            }
            else if (Status.Contains("ONR"))
            {
                Status = "Not Received";
            }
            else if (Status.Contains("OND"))
            {
                Status = "Not Delivered";
            }
            else
            {
                Status = "Undefined";
            }
            return Status;
        }

        [HttpGet]
        public string UpdateFromStartService(string Pcode, string ServicePath, string InetAddress, string Port, string MCAddress, string Database, string ServerName, string Auth, string userId, string Pass)
        {
            string Val = "0";
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    obj.ServicePath = ServicePath;
                    obj.InetAddress = InetAddress;
                    obj.Port = Port;
                    obj.MCAddress = MCAddress;
                    obj.ServiceLastActiveDate = DateTime.Now;
                    obj.SqlServerAuth = (Auth == "1") ? true : false;
                    obj.SqlPassword = Pass;
                    obj.SqlUserId = userId;
                    obj.DataBaseName = Database;
                    obj.SqlServerName = ServerName;
                    obj.ServiceStatus = true;
                    obj.IsDBConnected = true;
                    con.SaveChanges();
                    Val = "1";
                }
            }
            catch
            {
                Val = "0";
            }
            string PingVal = PingGBCService(Pcode);
            return Val+"/"+PingVal;
        }
        [HttpGet]
        public string updateUseraccount(string hrcode, string Password)
        {
            try
            {
                //Global obj = new Global();
                //string SQL = "update HrMaster set Password ='" + Password + "' where hrcode ='" + hrcode + "'";

                {
                    Global objGlbl = new Global();
                    DbConn con = new DbConn();
                    //  HrMaster1 obj =con.DB_master.SingleOrDefault(p => p.HrCode == hrcode);
                    // ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Password);

                    HrMaster obj = con.Db_hr.SingleOrDefault(p => p.HrCode == hrcode);
                    obj.Password = Password;
                    con.SaveChanges();
                    return "";
                }
            }
            catch
            {
                return "$";
            }
        }
        [HttpGet]
        public string UpdateFromStartServicePath(string Pcode, string ServicePath, string ServiceStatus="")
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == Pcode);
                    if (obj != null)
                    {
                        obj.ServicePath = ServicePath;
                        obj.ServiceStatus = Convert.ToBoolean(ServiceStatus);
                        con.SaveChanges();
                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch
            {
                return "0";
            }
        }
        [HttpGet]
        public string GetSuppAppUserData(string pcode)
        {
            try
            {
                using (GBCCon con = new GBCCon())
                {
                    ServiceReg obj = con.db_SerReg.SingleOrDefault(p => p.Pcode == pcode);
                    if (obj != null)
                    {
                        return obj.Name + "<|>" + obj.MobileNo + "<|>" + obj.EmailID + "<|>" + obj.Address + "<|>" + obj.ServiceStatus + "<|>" + obj.InetAddress;
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        [HttpGet]
        public HttpResponseMessage GetStateList(string name)
        {
            GBCCon con = new GBCCon();
            try
            {
                List<statemaster> list_state = con.Db_state.Where(p => p.StName.StartsWith(name)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, list_state);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }

        }

        [HttpGet]
        public HttpResponseMessage GetCityList(string Stcode, string name)
        {
            GBCCon con = new GBCCon();
            try
            {
                List<citymaster> list_city = con.Db_city.Where(p => p.CtCode.StartsWith(Stcode) && p.CtName.StartsWith(name)).ToList();
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
            GBCCon con = new GBCCon();
            try
            {
                List<areamst> list_area = con.Db_area.Where(p => p.AreaName.StartsWith(name)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, list_area);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        [HttpGet]
        public List<OB> OrderVrNOList(string PtCode, string AppType)
        {
            GBCCon gbc_con = new GBCCon();
            List<OB> list_ord = gbc_con.db_ob.Where(p => p.FromUserId == PtCode && p.AppType == AppType).OrderByDescending(p => p.VrDate).OrderByDescending(p => p.VrNo).ToList();
            return list_ord;
        }
       
        [HttpGet]
        public string getPretailOrdQty(string PCode)
        {
            GBCCon gbc_con = new GBCCon();
            ServiceReg obj = gbc_con.db_SerReg.SingleOrDefault(p => p.Pcode == PCode);
            if (obj != null)
            {
                return obj.pretailordQty;
            }
            else
            {
                return "0";
            }
        }

        [HttpGet]
        public string getDataFromMobno(string MobNo)
        {
            GBCCon gbc_con = new GBCCon();
            List<PatientMaster> obj = gbc_con.db_patient.Where(p => p.PhNo == MobNo).ToList();
            if (obj.Count!=0)
            {
                PatientMaster o = obj.Take(1).SingleOrDefault();
                return o.PtName + "<|>" + o.Address + "<|>" + o.PtCode;
            }
            else
            {
                return "";
            }
        }

        [HttpGet]
        public string getDataFromRegCode(string code)
        {
            GBCCon gbc_con = new GBCCon();
            List<PatientMaster> obj = gbc_con.db_patient.Where(p => p.PtCode == code).ToList();
            if (obj.Count != 0)
            {
                PatientMaster o = obj.Take(1).SingleOrDefault();
                return o.PtName + "<|>" + o.Address + "<|>" + o.PtCode + "<|>" + o.PhNo;
            }
            else
            {
                return "";
            }
        }


    }
}
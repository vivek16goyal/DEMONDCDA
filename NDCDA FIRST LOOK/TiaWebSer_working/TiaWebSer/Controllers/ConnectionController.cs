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
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TiaWebSer.Models;
namespace TiaWebSer.Controllers
{
    public class ConnectionController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage Ping()
        {
            try
            {
                if ((new Global()).CheckStarterService())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "1");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Your TiaERPApp service is stop!!! To use TiaERP@App Application, start TiaERPApp Service from AppServiceStarter.");
                }
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "0");
            }
        }

        [HttpGet]
        public string SelectDate(string Vrno)
        {
            Global objGlbl = new Global();
            string Dates = "";
            DataRow dr= objGlbl.SelectDemo(Vrno);
            if (dr.Table.Rows.Count > 0)
            {
                Dates = dr.Table.Rows[0]["VrDate"].ToString();
            }
            return Dates;
        }
        [HttpGet]
        public HttpResponseMessage Login(string UserNmae, string Pass, string Device="")
        {
            string[] strlog = new string[3];
            if ((new Global()).CheckStarterService())
            {
                Global objGlbl = new Global();
                DbConn con = new DbConn();                
                try
                {
                    HrMaster obj_HrMaster = con.Db_hr.SingleOrDefault(p => p.HrCode == UserNmae && p.Password == Pass);
                    if (obj_HrMaster == null)
                    {
                        strlog[0] = "0";
                    }
                    else
                    {
                        if (obj_HrMaster.UserType == 2)
                        {
                            bool flag = false;
                            DataRow dr = objGlbl.GetUserLoginsSql(1, "");
                            if (dr == null)
                            {
                                flag = true;
                            }
                            else
                            {
                                int loginuser = (dr["Cnt"] == null) ? 0 : Convert.ToInt16(dr["Cnt"]);
                                int cnt = 1;
                                try
                                {
                                    cnt = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["cnt"]);
                                }
                                catch
                                {
                                }
                                if (loginuser >= cnt)
                                {
                                    flag = false;
                                    strlog[0] = "No Of Users Exceed. You Can Not Login...!";
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                if (objGlbl.GetUserLoginsSql(2, UserNmae) == null)
                                {
                                    strlog[0] = "1";
                                    strlog[1] = obj_HrMaster.Name;
                                    strlog[2] = GetMenu(UserNmae);
                                    objGlbl.InsertLoginLog(UserNmae, Device);
                                    objGlbl.DBConnInfo(UserNmae);
                                }
                                else
                                {
                                    strlog[0] = "Sorry... AlReady LogIn For User " + UserNmae + " !";
                                }
                            }
                        }
                        else
                        {
                            strlog[0] = "Sorry..." + UserNmae + " Is Not Valid Mobile Login User !";
                        }
                    }
                }
                catch (Exception ex)
                {
                    strlog[0] = ex.Message;
                }
                return Request.CreateResponse(HttpStatusCode.OK, strlog);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Your TiaERPApp service is stop!!! To use TiaERP@App Application, start TiaERPApp Service from AppServiceStarter.");
            }
        }

        public string GetMenu(string HrCode)
        {
            string Menu = "";
            try
            {
                Global objGlbl = new Global();
                DataTable dt = objGlbl.getDataTable(false, "Select * from mnuhrights where hrcode in (select code1 from mmst  where mtype='GRHR' and code2='" + HrCode + "')");
                if (dt != null)
                {
                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        Menu = dt.Rows[i]["MNUID"].ToString() + "," + Menu;
                    }
                }
                Menu = Menu + "<<.>>" + System.Configuration.ConfigurationManager.AppSettings["SType"];
            }
            catch
            {
            }
            return Menu;
        }

        [HttpGet]
        public void UpdateUserLoginTime(string HRCODE)
        {
            Global objGlbl = new Global();
            objGlbl.ExecuteQuery(false, "Update Userloginlock set LastActiveTime=GetDate() Where LogId='" + HRCODE + "' And Convert(Varchar(10),IsNull(logdate,''),103)=Convert(Varchar(10),Getdate(),103) And logouttime is null");
        }


        [HttpGet]
        public void logout(string UserNmae)
        {
            Global objGlbl = new Global();
            objGlbl.InsertLogout(UserNmae);
        }
        

        [HttpGet]
        public string GetCon(string Main, string GBC, string UserCnt, string SType="2")
        {
            try
            {
                Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
                ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;
                AppSettingsSection section1 = config.GetSection("appSettings") as AppSettingsSection;
                if (section != null)
                {
                    section.ConnectionStrings["DbConn"].ConnectionString = Main;
                    section.ConnectionStrings["GBCDbConn"].ConnectionString = GBC;
                    section1.Settings.Remove("cnt");
                    config.Save();
                    section1.Settings.Add("cnt", UserCnt);
                    section1.Settings.Remove("SType");
                    config.Save();
                    section1.Settings.Add("SType", SType);
                    config.Save();
                }
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string GetSMSUrl( string MSG, string PhNo)
        {
            Global objGlbl = new Global();
            DataRow dr = objGlbl.getDataRow(false, "select url from smssetting");
            if (dr == null)
            {
                return "";
            }
            else
            {
                try
                {
                    string url = (dr["url"] == null) ? "" : dr["url"].ToString();
                    url = url.Replace("<<SNDTO>>", PhNo).Replace("<<MSG>>", MSG);
                    ValuesController objValues = new ValuesController();
                    return objValues.ExecuteUrl(url);
                }
                catch
                {
                    return "";
                }
            }
        }

        [HttpGet]
        public HttpResponseMessage GetFlag(string LogID)
        {
            try
            {
                Flag objFlag = new Flag();
                Global objGlbl = new Global();               
                DataRow dr = objGlbl.getDataRow(false, "select YN from MMST where code2='" + LogID + "'");
                if (dr != null)
                {
                    objFlag.Key = "pWsaleFreeQty";
                    objFlag.Value = (dr["YN"] == null) ? "0" : ((dr["YN"].ToString() == "1") ? "1" : "0");
                    objGlbl.listFlag.Add(objFlag);
                }
                objGlbl.ReadFlagValue();
                return Request.CreateResponse(HttpStatusCode.OK, objGlbl.listFlag);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error While getting Flag Value" + ex.Message);
            }
        }

        [HttpGet]
        public string getServiceVersion()
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["ServiceVersion"];
            }
            catch
            {
                return "1.0.0";
            }
        }
        [HttpGet]
        public void SendNotify(string PtCode,string Vrno)
        {
            try
            {
                (new Global()).SendNotification(PtCode, Vrno);
            }
            catch
            {
            }
        }
    }
}

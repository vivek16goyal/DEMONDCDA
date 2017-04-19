using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.Administration;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;
using System.Net.NetworkInformation;
using System.Data.OleDb;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Timers;

namespace StartTiaAppService
{
    public partial class Form1 : Form
    {
        string CipPCODE = "", localPhyAdd, NoOfLoginUser = "1", Stype = "2";
        DataTable dt_ = new DataTable();
        bool AppReg = false;
        DateTime CipAppReDate;
        bool IsDbConnect = false;
        string gbcDataBaseName;
        string CiphertextFilePath;
        bool starttimr = false;
        string ServicePath, InetAddress, Port;
        static string strWebsitename = "TiaAppWebService";
        BackgroundWorker helperBW;
        public static string GBCUrl = ConfigurationManager.AppSettings["GBCServiceUrl"];
        bool TestingInetCon = Convert.ToBoolean(ConfigurationManager.AppSettings["TestingInetCon"]); //// set this variable true for testing before publish make it false
        bool ReqiredDetail = false;
        static string MainCon, GBCCon;
        string AppActiveDateTime, OldAppActiveDate;
        static bool IsServiceStart = false;
        static DateTime startDt;

        public void ReadCipherSetData()
        {
            try
            {
                addMsg("Cipher Reading Process Start.");
                bool isValidAddress = false;
                dt_ = GetCipherData();
                string ClntCode = "", ClntName = "";
                if (dt_ != null)
                {
                    foreach (DataRow row1 in dt_.Rows)
                    {
                        if (row1[0].ToString().ToUpper().Trim() == "CLIENTCODE")
                        {
                            ClntCode = row1[1].ToString().ToUpper().Trim();
                            break;
                        }
                    }
                    foreach (DataRow row1 in dt_.Rows)
                    {
                        if (row1[0].ToString().ToUpper().Trim() == "CONAME")
                        {
                            ClntName = row1[1].ToString().ToUpper().Trim();
                            break;
                        }
                    }
                    label11.Text=ClntCode+" - "+ClntName;
                    addMsg("Getting Machine Physical Address.");
                    localPhyAdd = GetPhyAddress();
                    addMsg("Machine Physical Address Is " + localPhyAdd + "");
                    addMsg("Checking Licensing for TiaERP@App Application...");
                    foreach (DataRow row in dt_.Rows)
                    {
                        if (row[0].ToString().ToUpper().Contains("PHYSICALADDRESS"))
                        {
                            string PhyAdds = row[1].ToString().Trim();
                            PhyAdds = PhyAdds.Replace("-", "");
                            if (localPhyAdd == PhyAdds)
                            {                                
                                isValidAddress = true;
                                break;
                            }
                        }                        

                    }
                    foreach (DataRow row in dt_.Rows)
                    {                        
                        if (isValidAddress == true)
                        {
                            if (row[0].ToString().ToUpper().Trim() == "TIAERPAPP")
                            {
                                try
                                {
                                    AppReg = Convert.ToBoolean(row[1].ToString().Trim());
                                }
                                catch(Exception ex)
                                {
                                    AppReg = false;
                                }
                                break;
                            }
                        }
                        else
                        {
                            addMsg("Sorry U Dont Have License To Run This Program !");
                            break;
                        }
                    }
                    if (isValidAddress == true && AppReg == true)
                    {
                        foreach (DataRow row in dt_.Rows)
                        {
                            if (AppReg == true)
                            {
                                if (row[0].ToString().ToUpper().Trim() == "APPREDATE")
                                {
                                    try
                                    {
                                        CipAppReDate = Convert.ToDateTime(row[1].ToString().Trim());
                                        if (CipAppReDate >= DateTime.Now.Date)
                                        {
                                            foreach (DataRow row1 in dt_.Rows)
                                            {
                                                if (row1[0].ToString().ToUpper().Trim() == "CLIENTCODE")
                                                {
                                                    CipPCODE = row1[1].ToString().ToUpper().Trim();
                                                    break;
                                                }
                                            }
                                            string PWms = "", pBothRmsWms = "";
                                            if (CipPCODE != "")
                                            {
                                                foreach (DataRow row1 in dt_.Rows)
                                                {
                                                    if (row1[0].ToString().ToUpper().Trim() == "NOOFAPPLOGINUSER")
                                                    {
                                                        NoOfLoginUser = row1[1].ToString().ToUpper().Trim();
                                                    }
                                                    
                                                    if (row1[0].ToString().ToUpper().Trim() == "PWMS")
                                                    {
                                                        PWms = row1[1].ToString().ToUpper().Trim();
                                                    }
                                                    if (row1[0].ToString().ToUpper().Trim() == "PBOTHRMSWMS")
                                                    {
                                                        pBothRmsWms = row1[1].ToString().ToUpper().Trim();
                                                    }
                                                    
                                                }
                                            }
                                            if (pBothRmsWms == "TRUE")
                                            {
                                                Stype = "2";
                                            }
                                            else
                                            {
                                                if (PWms == "TRUE")
                                                {
                                                    Stype = "1";
                                                }
                                                else
                                                {
                                                    Stype = "0";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception();
                                        }
                                    }
                                    catch
                                    {
                                        addMsg("Sorry Your Licensing For TiaERP@App Is Expired !");
                                    }
                                }
                                if (CipPCODE != "")
                                {
                                    break;
                                }
                            }
                            else
                            {
                                addMsg("Sorry U Dont Have Licensing For TiaERP@App !");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    addMsg("Sorry U Dont Have Licence To Run This Program !!!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetPhyAddress()
        {
            const int MIN_MAC_ADDR_LENGTH = 12; 
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }
            return macAddress;
        }

        public void StartChecking()
        {
            ReadCipherSetData();
            if (CipPCODE == "")
            {
                addMsg("Process Stop. Due To Problem In Getting Data From Cipher.");
                ShowBallonPopup("2", "Process Stop. Due To Problem In Getting Data From Cipher.", strWebsitename);
                StopService(strWebsitename);
            }
            else
            {
                if (SetDataToTextBox())
                {
                    AfterDbConnect();
                }
            }
        }

        public void AfterDbConnect()
        {
            if (IsDbConnect)
            {
                if (AddServiceToIIS())
                {
                    addMsg("Setting Database Connection To TiaERP@App Service...");
                    //ServicePath = "http://" + txtserver.Text.Trim() + ":" + Port + "/";
                    ServicePath = "http://192.168.9.15" + ":" + Port + "/";
                    string res = FireUrl(ServicePath + "/Connection/GetCon?Main=" + MainCon + "&GBC=" + GBCCon + "&UserCnt=" + NoOfLoginUser + "&SType=" + Stype);
                    if (res == "OK")
                    {
                        addMsg("Trying To Start TiaERP@App Service On Public Ip...");
                        ExecuteQuery("update multiplefirm set printQuser='" + txtprintUser.Text.Trim() + "',appactivedatetime=getdate()");
                        //startTimer();
                        
                        if (CheckInternet())
                        {
                            if (GetPartyStatusFromGBC())
                            {
                                InetAddress = GetPublicIPAddress();
                                ServicePath = "http://" + InetAddress.Trim() + ":" + Port + "/";
                                string url = ServicePath + "/Connection/Ping";
                                if (FireUrl(url, "1") != "")
                                {
                                    OnStart(0);
                                    IsServiceStart = true;
                                }
                                else
                                {
                                    addMsg("Failed To Start Service On Public Ip.");
                                    IsServiceStart = false;
                                }
                                url = GBCUrl + "/Values/UpdateFromStartService?Pcode=" + CipPCODE + "&ServicePath=" + ServicePath + "&InetAddress=" + InetAddress + "&Port=" + Port + "&MCAddress=" + localPhyAdd + "&Database=" + txtdatabase.Text.Trim() + "&ServerName=" + txtserver.Text.Trim() + "&Auth=" + cmbAuth.SelectedIndex.ToString() + "&userId=" + userid.Text.Trim() + "&Pass=" + pass.Text.Trim();
                                string output = FireUrl(url);
                                string[] returnval = output.Split('/');
                                string UpVal = returnval[0].ToString();
                                string PinVal = returnval[1].ToString();
                                string MsgPing = "";
                                if (UpVal == "1")
                                {
                                    addMsg("GBC Server Updated Successfully.");
                                    MsgPing = ReturnPingVal(PinVal);
                                    addMsg(MsgPing);
                                    IsServiceStart = true;
                                }
                                else
                                {
                                    IsServiceStart = false;
                                    addMsg("GBC Server Update Failed.");
                                    MsgPing = ReturnPingVal(PinVal);
                                    addMsg(MsgPing);
                                    ServicePath = "http://" + txtserver.Text.Trim() + ":" + Port + "/";
                                    OnStart(1);
                                }
                                
                            }
                        }
                        else
                        {
                            IsServiceStart = false;
                            OnStart(1);
                        }
                    }
                    else
                    {
                        IsServiceStart = false;
                        addMsg("Error From TiaERpService when Updating Connection " + res);
                        addMsg("Process Stop, Due To Connection Not Update On TiaAppWebService.");
                        ShowBallonPopup("2", "Process Stop, Due To Connection Not Update On TiaAppWebService.", strWebsitename);
                        StopService(strWebsitename);
                    }
                }
                else
                {
                    IsServiceStart = false;
                    addMsg("Process Stop, Due To Connection To IIS Is Not Possible .");
                    ShowBallonPopup("2", "Process Stop, Due To Connection To IIS Is Not Possible .", strWebsitename);
                    StopService(strWebsitename);
                }
            }
            else
            {
                IsServiceStart = false;
                ShowBallonPopup("2", "Process Stop, Due To Database Connection Was Not Established.", strWebsitename);
                addMsg("Process Stop, Due To Database Connection Was Not Established.");
                StopService(strWebsitename);
            }
        }

        public string ReturnPingVal(string PinVal)
        {
            string Rval = "";
            if (PinVal == "1")
            {
                Rval = "GBC Server Connection Ping And Service Status is Active";
            }
            else
            {
                Rval = "GBC Server Connection Not Ping And Service Status is Unactive";
            }
            return Rval;
        }
        public void OnStart(int flag)
        {
            if (flag == 1)
            {
                addMsg("TiaERP@App Service Start On Local Area.");
                addMsg("Assigned PatH: " + ServicePath);
                ShowBallonPopup("1", "Assigned Path: " + ServicePath + "\n" + strWebsitename + "Service Start On Local Area.", strWebsitename + " Start Succefully!");
            }
            else
            {
                addMsg("TiaERP@App Service Start On Public Ip.");
                addMsg("Assigned PatH: " + ServicePath);
                ShowBallonPopup("1", "Assigned Path: " + ServicePath + "\n" + strWebsitename + "Service Start On Public Ip.", strWebsitename + " Start Succefully!");
            }
            //Thread.Sleep(11000);
            this.Hide();
        }

        public bool GetPartyStatusFromGBC()
        {
            addMsg("Checking Party Licence From GBC Server.");
            string url = GBCUrl + "/Values/PingGBCService";
            if (FireUrl(url) == "1")
            {
                return GetStatus(GBCUrl + "/Values/CheckPcode?pcode=" + CipPCODE);
            }
            else
            {
                addMsg("Problem Occurred While Getting Response From GBC Server.");
                return false;
            }
        }

        public void startTimer()
        {
            starttimr = true;
        }

        public bool GetStatus(string url)
        {
            try
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
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                party obj = serializer.Deserialize<party>(json);
                addMsg("TiaERP@App Service Running For Client Id " + CipPCODE + "");
                if (obj != null)
                {
                    if (obj.IsActive == true)
                    {
                        if (obj.ValidUpToDate >= DateTime.Now.Date)
                        {
                            addMsg("Reply from GBC, Party is registered...");
                            return true;
                        }
                        else
                        {
                            addMsg("Your License is currently deactivated by GBC. Please contact with GBC Administrator.");
                            ShowBallonPopup("2", "Your License is currently deactivated by GBC. Please contact with GBC Administrator.", strWebsitename);
                            StopService(strWebsitename);
                            return false;
                        }
                    }
                    else
                    {
                        addMsg("Licensing for TiaERP@App Application has expired. Please contact with GBC administrator.");
                        ShowBallonPopup("2", "Licensing for TiaERP@App Application has expired. Please contact with GBC administrator.", strWebsitename);
                        StopService(strWebsitename);
                        return false;
                    }
                }
                else
                {
                    addMsg("Party is not registered on GBC Server. Please contact with GBC Administrator.");
                    ShowBallonPopup("2", "Party is not registered on GBC Server. Please contact with GBC Administrator.", strWebsitename);
                    StopService(strWebsitename);
                    return false;
                }

            }
            catch (Exception ex)
            {
                addMsg("Error occurred while excuting URL " + url + " " + ex.Message);
                ShowBallonPopup("2", "Error occurred while excuting URL " + url + " " + ex.Message, strWebsitename);
                StopService(strWebsitename);
                return false;
            }
        }

        public bool SetDataToTextBox()
        {
            try
            {
                txtserver.Text = ConfigurationManager.AppSettings["Server"];
                txtdatabase.Text = ConfigurationManager.AppSettings["DBName"];
                userid.Text = ConfigurationManager.AppSettings["Userid"];
                pass.Text = ConfigurationManager.AppSettings["Pass"];
                string auths = ConfigurationManager.AppSettings["Auth"];
                if (auths != null && auths != "")
                {
                    cmbAuth.SelectedIndex = Convert.ToInt32(auths.ToString());
                }
                txtprintUser.Text = ConfigurationManager.AppSettings["PrintUser"];
                if (txtserver.Text == "")
                {
                    MessageBox.Show("Please Enter Required Details.");
                    SettingPage();
                }
                else
                {
                    return ConnectToDatabase();
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool CheckInternet()
        {
            int cnt = 0;
            addMsg("Trying to ping google Server..");
            String host = "4.2.2.2";
        a: try
            {
                Ping myPing = new Ping();
                //"google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    addMsg("Internet connection found...");
                    return true;
                }
                else
                {
                    if (cnt == 0)
                    {
                        cnt++;
                        host = "8.8.8.8";
                        goto a;
                    }
                    addMsg("Internet connection not found...");
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (cnt == 0)
                {
                    cnt++;
                    host = "8.8.8.8";
                    goto a;
                }
                addMsg("Error occurred while checking Internet connection.." + ex.Message);
                return false;
            }

        }

        public bool ConnectToDatabase()
        {
            string servername = txtserver.Text.Trim();
            string databasename = txtdatabase.Text.Trim();
            string auth = cmbAuth.SelectedIndex.ToString();
            string userId = userid.Text.Trim();
            string password = pass.Text.Trim();
            string PrintQUser = txtprintUser.Text.Trim();
            bool isValid = false;
            if (String.IsNullOrEmpty(servername) || String.IsNullOrEmpty(databasename) || String.IsNullOrEmpty(PrintQUser))
            {
                isValid = false;
            }
            else
            {
                if (auth == "1")
                {
                    if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(password))
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    isValid = true;
                }

            }
            if (isValid)
            {
                addMsg("Checking Database Connection.");
                addMsg("Please Wait, It Will Take Some Time...");
                ////Checking Main database Connection
                try
                {
                    Connections(servername, databasename, auth, userId, password);
                    OldAppActiveDate = AppActiveDateTime;
                    IsDbConnect = true;
                    var Config = ConfigurationManager.OpenExeConfiguration(System.Windows.Forms.Application.ExecutablePath);

                    Config.AppSettings.Settings.Remove("Server");
                    Config.AppSettings.Settings.Add("Server", servername);

                    Config.AppSettings.Settings.Remove("DBName");
                    Config.AppSettings.Settings.Add("DBName", databasename);

                    Config.AppSettings.Settings.Remove("Auth");
                    Config.AppSettings.Settings.Add("Auth", auth);

                    Config.AppSettings.Settings.Remove("Userid");
                    Config.AppSettings.Settings.Add("Userid", userId);

                    Config.AppSettings.Settings.Remove("Pass");
                    Config.AppSettings.Settings.Add("Pass", password);

                    Config.AppSettings.Settings.Remove("PrintUser");
                    Config.AppSettings.Settings.Add("PrintUser", PrintQUser);

                    ConfigurationManager.RefreshSection("appSettings");
                    Config.Save(ConfigurationSaveMode.Full);
                    ConfigurationManager.RefreshSection("appSettings");
                    return true;
                }
                catch (Exception ex)
                {
                    IsDbConnect = false;
                    string msg;
                    int ErrNo = ex.Message.GetHashCode();
                    if (ErrNo == 1147457002 || ex.Message.Contains(databasename))
                    {
                        msg = "Please Enter Valid Database Name";
                    }
                    else if (ErrNo == -1614262360)
                    {
                        msg = "Please Enter Valid Server Name";
                    }
                    else if (ErrNo == 1390019596)
                    {
                        msg = "Invalid UserName Or Password";
                    }
                    else if (ErrNo == 1471440600)
                    {
                        msg = "Please Select SQL Server Authentication.";
                    }
                    else
                    {
                        msg = ex.Message;
                    }
                    addMsg(msg);
                    MessageBox.Show(msg);
                }
                return false;
            }
            else
            {
                MessageBox.Show("Please Enter All Deatils.");
                return false;
            }
        }

        public void Connections(string servername, string databasename, string auth, string userId, string password)
        {
            string strCon = "Data Source = " + servername + ";Initial Catalog = " + databasename + ";Timeout=10;Application Name=TIA3T_Main;Integrated Security=" + (((auth == "1") ? true : false) ? "False;User ID=" + userId + ";Password=" + password + "" : "True") + "";
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = strCon;
            conn.Open();
            conn.Close();
            MainCon = strCon;
            DataTable dt = GetDataTable(strCon, "select * from multiplefirm");
            if (dt.Rows.Count != 0)
            {
                gbcDataBaseName = dt.Rows[0]["DatabaseName"].ToString();
                AppActiveDateTime = dt.Rows[0]["AppActiveDateTime"].ToString();
            }

            gbcDataBaseName = gbcDataBaseName + ACYR();
            GBCCon = strCon.Replace(databasename, gbcDataBaseName);
            conn.ConnectionString = GBCCon;
            conn.Open();
            conn.Close();
        }

        public Form1()
        {
            InitializeComponent();
            RichTextBox.CheckForIllegalCrossThreadCalls = false;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        public void addMsg(string msg)
        {
            string datetime = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
            txtlog.Text = txtlog.Text + datetime + " " + msg + "\n";
            txtlog.SelectionStart = txtlog.Text.Length;
            txtlog.ScrollToCaret();
            ServiceLog(datetime + " " + msg + "\n");
        }

        #region Cipher
        public DataTable GetCipherData()
        {
            try
            {
                string CryptPassword = "Good Health Pharmacy Rajat Sankul,Opp. Bus Stand Ganeshpeth,Nagpur-18. Ph.No. : (0712) 3294004, 3240909, 9370707169.";
                Mantra.CryptoStuff OBJCryptoStuff = new Mantra.CryptoStuff();
                string file_path;
                //
                file_path = Path.Combine(Environment.CurrentDirectory, "");
                //file_path = Path.Combine(Environment.CurrentDirectory, "..\\..");
                CiphertextFilePath = file_path + "\\ciphertext.dat";

                DataTable dt = null;
                string DecipheredFilePath = file_path + "\\deciphered.txt";
                if (File.Exists(CiphertextFilePath))
                {
                    if (File.Exists(DecipheredFilePath))
                    {
                        File.SetAttributes(DecipheredFilePath, FileAttributes.Normal);
                        File.Delete(DecipheredFilePath);
                    }
                    OBJCryptoStuff.DecryptFile(CryptPassword, CiphertextFilePath, DecipheredFilePath);
                    dt = GetCsvData(file_path, "deciphered.txt");
                    if (File.Exists(DecipheredFilePath))
                    {
                        File.SetAttributes(DecipheredFilePath, FileAttributes.Normal);
                        File.Delete(DecipheredFilePath);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                addMsg("Error occured in GetCipherData() " + ex.Message);
                return null;
            }
        }
        public DataTable GetCsvData(string strFolderPath, string strFileName)
        {
            string strConnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFolderPath + ";Extended Properties=Text;";
            OleDbConnection conn = new OleDbConnection(strConnString);
            try
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + strFileName + "]", conn);
                OleDbDataAdapter da = new OleDbDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                da.Dispose();
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                addMsg("Error Occurred while reading Cipher Data" + ex.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }

        }
        #endregion

        public void ShowBallonPopup(string type, string msg, string title = "")
        {
            switch (type)
            {
                case "1":
                    notifyIcon2.BalloonTipIcon = ToolTipIcon.Info;
                    break;
                case "2":
                    notifyIcon2.BalloonTipIcon = ToolTipIcon.Error;
                    break;
                case "3":
                    notifyIcon2.BalloonTipIcon = ToolTipIcon.Warning;
                    break;
            }
            if (title == "")
            {
                title = "TiaERP@App Service";
            }
            notifyIcon2.BalloonTipTitle = title;
            notifyIcon2.BalloonTipText = msg;
            notifyIcon2.ShowBalloonTip(6000);
        }

        public string FireUrl(string url, string flag = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json, text/javascript, */*";
                request.Method = "GET";
                request.ServicePoint.Expect100Continue = false;
                  
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
                if (flag == "1")
                {
                    return "1";
                }
                else
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string result = serializer.Deserialize<string>(json);
                    return result;
                }
            }
            catch (Exception ex)
            {
                addMsg("Error occurred while excuting URL " + url + " " + ex.Message);
                return "";
            }
        }

        public party FireUrl1(string url)
        {
            try
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
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                party obj = serializer.Deserialize<party>(json);
                if (obj != null)
                {
                    if (obj.IsActive == true)
                    {
                        if (obj.ValidUpToDate >= DateTime.Now.Date)
                        {
                            if (String.IsNullOrEmpty(obj.SqlServerName) || String.IsNullOrEmpty(obj.DataBaseName))
                            {
                                addMsg("Enter Required Information...");
                                MessageBox.Show("Enter Required Information...");
                                SettingPage();
                                return null;
                            }
                            else
                            {
                                addMsg("Reply from GBC, Party is registered...");
                                txtserver.Text = obj.SqlServerName;
                                txtdatabase.Text = obj.DataBaseName;
                                cmbAuth.SelectedIndex = (Convert.ToBoolean(obj.SqlServerAuth)) ? 1 : 0;
                                userid.Text = obj.SqlUserId;
                                pass.Text = obj.SqlPassword;
                                btnConnect_Click(null, null);
                                return null;
                            }
                        }
                        else
                        {
                            addMsg("Your License is currently deactivated by GBC. Please contact with GBC Administrator.");
                            return null;
                        }
                    }
                    else
                    {
                        addMsg("Licensing for TiaERP@App Application has expired. Please contact with GBC administrator.");
                        return null;
                    }
                }
                else
                {
                    addMsg("Party is not registered. Please contact with GBC Administrator.");
                    return null;
                }

            }
            catch (Exception ex)
            {
                addMsg("Error occurred while excuting URL " + url + " " + ex.Message);
                return null;
            }
        }

        public void SettingPage()
        {
            ReqiredDetail = true;
            grpSetting.Enabled = true;
            grpSetting.BringToFront();
            txtserver.Focus();
            txtserver.BackColor = System.Drawing.Color.Aqua;
            cmbAuth.SelectedIndex = 0;
        }



        public string GetPublicIPAddress()
        {
            int cnt = 1;
        a: try
            {
                addMsg("Try " + cnt + ": Getting public IP address..");
                string externalIP = "";
                externalIP = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                return externalIP;
            }
            catch
            {
                if (cnt <= 3)
                {
                    cnt++;
                    goto a;
                }
                if (TestingInetCon)
                {
                    return "http://localhost:9979/";
                }
                else
                {
                    throw;
                }
            }
        }

        public bool AddServiceToIIS()
        {
            try
            {
                string path = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["folderpath"])) ? "C:\\inetpub\\wwwroot\\Publish\\TiaAppService" : ConfigurationManager.AppSettings["folderpath"].ToString();
                addMsg("Process start to add TiaAppWebService in IIS");
                ServerManager serverMgr = new ServerManager();
                // abc
                string strApplicationPool = ConfigurationManager.AppSettings["ApplicationPool"]; //"ASP.NET v4.0";
                Port = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["Port"])) ? "9979" : ConfigurationManager.AppSettings["Port"].ToString();
                //check if website name already exists in IIS
                Boolean bWebsite = IsWebsiteExists(strWebsitename);
                if (!helperBW.CancellationPending)
                {
                    if (!bWebsite)
                    {
                        addMsg("Checking for physical path of TiaAppWebService folder...");
                        string path1 = Path.Combine(path) + "\\web.config";
                        if (File.Exists(path1))
                        {
                            addMsg("Trying to start TiaAppWebService...");
                            Site mySite = serverMgr.Sites.Add(strWebsitename.ToString(), "http", "*:" + Port + ":", path);
                            mySite.ApplicationDefaults.ApplicationPoolName = strApplicationPool;
                            mySite.TraceFailedRequestsLogging.Enabled = true;
                            mySite.TraceFailedRequestsLogging.Directory = path;
                            if (!helperBW.CancellationPending)
                            {
                                serverMgr.CommitChanges();
                            }
                            //return true;
                            return StartService(strWebsitename);
                        }
                        else
                        {
                            addMsg("TiaAppWebService folder not found...");
                            return false;
                        }
                    }
                    else
                    {
                        if (!helperBW.CancellationPending)
                        {
                            addMsg("Trying to start TiaAppWebService...");
                            return StartService(strWebsitename);
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ae)
            {
                ShowBallonPopup("2", ae.Message);
                return false;
            }

        }        
        public bool IsWebsiteExists(string strWebsitename)
        {
            try
            {
                addMsg("Checking TiaAppWebService is present or not...");
                ServerManager serverMgr = new ServerManager();
                Boolean flagset = false;
                SiteCollection sitecollection = serverMgr.Sites;

                foreach (Site site in sitecollection)
                {
                    if (site.Name == strWebsitename.ToString())
                    {
                        try
                        {
                            Site s1 = serverMgr.Sites[strWebsitename]; // you can pass the site name or the site ID
                            serverMgr.Sites.Remove(s1);
                            serverMgr.CommitChanges();
                            flagset = false;
                        }
                        catch
                        {
                            flagset = true;
                        }
                        addMsg("TiaAppWebService is present");
                        break;
                    }
                    else
                    {
                        flagset = false;
                    }
                }
                return flagset;
            }
            catch (Exception ex)
            {
                addMsg("Error occurred while excuting IsWebsiteExists() " + ex.Message);
                return false;
            }
        }
        public bool StartService(string strWebsitename)
        {
            try
            {
                ServerManager serverMgr = new ServerManager();
                var site = serverMgr.Sites.FirstOrDefault(p => p.Name == strWebsitename);
                if (site != null)
                {
                    Thread.Sleep(1000);
                    site.Start();
                    addMsg(strWebsitename + " Start Successfully!!!");
                    return true;
                }
                else
                {
                    addMsg(strWebsitename + " is not  exists.");
                    return false;
                }
            }
            catch (Exception Ex)
            {
                addMsg("StartService() " + Ex.Message);
                return false;
            }
        }
        public void StopService(string strWebsitename)
        {
            try
            {
                ServerManager serverMgr = new ServerManager();
                var site = serverMgr.Sites.FirstOrDefault(p => p.Name == strWebsitename);
                if (site != null)
                {
                    site.Stop();
                    string url = GBCUrl + "/Values/StopService?Pcode=" + CipPCODE;
                    FireUrl(url);
                    IsServiceStart = false;
                    startDt = System.DateTime.Now;
                    timer1.Stop();
                    timer2.Start();
                }
            }
            catch
            {
            }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            StopProcess();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            txtlog.Text = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " Process Start.." + "\n";
            Thread.Sleep(1000);
            helperBW = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = BackgroundProcessLogicMethod(helperBW, arg);

            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }
            if (starttimr)
            {
                Thread.Sleep(1000);
                //timer1.Start();
            }
            if (ReqiredDetail)
            {
                this.ActiveControl = txtserver;
                txtserver.Focus();
            }
        }
        private int BackgroundProcessLogicMethod(BackgroundWorker bw, int a)
        {
            int result = 0;
            if (!bw.CancellationPending)
            {
                lblstate.Text = "Trying to start";
                StartChecking();
            }

            return result;
        }
        private void Form1_Load_1(object sender, EventArgs e)//
        {
            startDt = System.DateTime.Now;
            timer1.Stop();
            timer2.Start();
            btnConn.Enabled = false;
            startProcess();
            btnConn.Enabled = true;
        }


        public void startProcess()//
        {
            this.Show();
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync(2000);
            }
        }

        public void StopProcess()
        {
            try
            {
                try
                {
                    backgroundWorker1.CancelAsync();
                    StopService(strWebsitename);
                    ShowBallonPopup("2", strWebsitename + " Service Stop.", strWebsitename);
                    this.Hide();
                }
                catch
                {
                    StopService(strWebsitename);
                    ShowBallonPopup("2", strWebsitename + " Service Stop.", strWebsitename);
                    this.Hide();
                }
                txtlog.Text = "";
                lblstate.Text = "Stop";
                addMsg(strWebsitename + " Stop Successfully!!!");
                ShowBallonPopup("1", "Stop Successfully!!!");
            }
            catch
            {
            }
        }
        private void btnConn_Click(object sender, EventArgs e)
        {
            startProcess();
        }
        private void btnDis_Click(object sender, EventArgs e)
        {
            StopProcess();
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            startProcess();
        }
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopProcess();
        }
        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            StopService(strWebsitename);
            ShowBallonPopup("2", strWebsitename + " Service Stop.", strWebsitename);
            System.Windows.Forms.Application.Exit();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                MessageBox.Show("Task manager tried to close me");
            }
            StopService(strWebsitename);
            ShowBallonPopup("2", strWebsitename + " Service Stop.", strWebsitename);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtserver_Enter(object sender, EventArgs e)
        {
            txtserver.BackColor = System.Drawing.Color.Aqua;
        }

        private void txtdatabase_Enter(object sender, EventArgs e)
        {
            txtdatabase.BackColor = System.Drawing.Color.Aqua;
        }

        private void cmbAuth_Enter(object sender, EventArgs e)
        {
            cmbAuth.BackColor = System.Drawing.Color.Aqua;
        }

        private void userid_Enter(object sender, EventArgs e)
        {
            userid.BackColor = System.Drawing.Color.Aqua;
        }

        private void pass_Enter(object sender, EventArgs e)
        {
            pass.BackColor = System.Drawing.Color.Aqua;
        }

        private void txtserver_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtserver_Leave(object sender, EventArgs e)
        {
            txtserver.BackColor = System.Drawing.Color.White;
        }

        private void txtdatabase_Leave(object sender, EventArgs e)
        {
            txtdatabase.BackColor = System.Drawing.Color.White;
        }

        private void cmbAuth_Leave(object sender, EventArgs e)
        {
            cmbAuth.BackColor = System.Drawing.Color.White;
        }

        private void userid_Leave(object sender, EventArgs e)
        {
            userid.BackColor = System.Drawing.Color.White;
        }

        private void pass_Leave(object sender, EventArgs e)
        {
            pass.BackColor = System.Drawing.Color.White;
        }

        private void cmbAuth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAuth.SelectedIndex.ToString() == "1")
            {
                if (!String.IsNullOrEmpty(txtserver.Text))
                {

                    btnConn.Focus();
                    userid.BackColor = System.Drawing.Color.Aqua;
                }
                groupBox2.Enabled = true;
            }
            else
            {
                groupBox2.Enabled = false;
                userid.Focus();
                userid.BackColor = System.Drawing.Color.White;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (ConnectToDatabase())
            {
                AfterDbConnect();
            }

        }

        public DataTable GetDataTable(string con, string cmdtxt)
        {
            try
            {
                SqlDataAdapter adp = new SqlDataAdapter(cmdtxt, con);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        public bool ExecuteQuery(string StrSql)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection con = new SqlConnection(MainCon);
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

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            grpSetting.Enabled = true;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            MessageBox.Show("Tick");
        }

        private void txtlog_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                addMsg("Timer2_Tick!!! Attempt to start Timer1.");
                if (!IsServiceStart)
                {
                    if (startDt.AddMinutes(3) <= System.DateTime.Now)
                    {
                        addMsg("Automatic start service called from timer1");
                        startDt = System.DateTime.Now;
                        startProcess();
                    }
                }
                else
                {
                    string servername = txtserver.Text.Trim();
                    string databasename = txtdatabase.Text.Trim();
                    string auth = cmbAuth.SelectedIndex.ToString();
                    string userId = userid.Text.Trim();
                    string password = pass.Text.Trim();
                    DateTime oldTime, newTime;
                    if (servername != "" && databasename != "" && auth != "")
                    {
                        Connections(servername, databasename, auth, userId, password);
                        oldTime = Convert.ToDateTime(OldAppActiveDate.ToString());
                        newTime = Convert.ToDateTime(AppActiveDateTime.ToString());
                        int Comp = DateTime.Compare(oldTime, newTime);
                        if (Comp < 0)
                        {
                            addMsg("Timer2 stop!!! Timer1 start.");
                            timer1.Start();
                            timer2.Stop();
                        }
                    }
                }
            }
            catch { }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                InetAddress = GetPublicIPAddress();
                ServicePath = "http://" + InetAddress.Trim() + ":" + Port + "/";
                string url1 = ServicePath + "/Connection/Ping";
                if (FireUrl(url1, "1") != "")
                {
                    url1 = GBCUrl + "/Values/UpdateFromStartServicePath?Pcode=" + CipPCODE + "&ServicePath=" + ServicePath + "&ServiceStatus=True";
                    if (FireUrl(url1) == "1")
                    {
                        addMsg("Server Path Updated Successfully On GBC Server.");
                        addMsg("Assigned PatH: " + ServicePath);
                    }
                    else
                    {
                        addMsg("Server Path Update Failed On  GBC Server.");
                    }
                }
                else
                {
                    addMsg("Server Path Update Failed And Failed To Start Service On Public Ip.");
                }
            }
            catch(Exception ex)
            {
                addMsg("Failed To Start Service On Public Ip." + ex.Message);
            }
            try
            {
                if (CipAppReDate >= DateTime.Now.Date)
                {
                    ServicePath = "http://" + txtserver.Text.Trim() + ":" + Port + "/";
                    string url = ServicePath + "/Connection/ping";
                    if (FireUrl(url, "1") != "")
                    {
                        ExecuteQuery("update multiplefirm set appactivedatetime=getdate()");
                        addMsg("Server Update With Required Data.");
                    }
                    else
                    {
                        StopService(strWebsitename);
                        ShowBallonPopup("2", strWebsitename + " Service Stop. Due To Not Connecting To TiaAppService", strWebsitename);
                    }
                }
                else
                {
                    StopService(strWebsitename);
                    addMsg("Sorry Your Licensing For TiaERP@App Is Expired !");
                    ShowBallonPopup("2", strWebsitename + " Sorry Your Licensing For TiaERP@App Is Expired !", strWebsitename);
                }
            }
            catch (Exception ex)
            {
                addMsg("Error occurred In updating Active Time"+ ex.Message);
            }
            try
            {
                if (!IsServiceStart)
                {
                    if (startDt.AddMinutes(3) <= System.DateTime.Now)
                    {
                        addMsg("Automatic start service called from timer1");
                        startDt = System.DateTime.Now;
                        startProcess();
                    }
                }
            }
            catch
            {
            }
        }
        public void ServiceLog(string Msg)
        {
            try
            {
                //string file_path = Path.Combine(Environment.CurrentDirectory, "");
                string file_path = ConfigurationManager.AppSettings["folderpath"];
                file_path = file_path + "\\StartTiaAppServiceLog_" + System.DateTime.Now.ToString("ddMMyy") + ".txt";
                using (StreamWriter sw = (File.Exists(file_path)) ? File.AppendText(file_path) : File.CreateText(file_path))
                {
                    sw.WriteLine(Msg);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                addMsg("Failed to write Service Log File. " + ex.Message);
            }
        }

    }
}

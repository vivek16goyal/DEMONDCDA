using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TiaWebSer.Models;

namespace TiaWebSer.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
            //DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/VrImg"));
            //var files = dirInfo.GetFiles();
            //string filename = dirInfo.FullName + @"\" + "O216000006.jpeg";
            //string contentType = "application/jpeg";
            ////Parameters to file are
            ////1. The File Path on the File Server
            ////2. The content type MIME type
            ////3. The parameter for the file save by the browser
            //return File(filename, contentType, "Report.jpeg");
        }
        public List<string> Demo()
        {
            List<string> ls = new List<string>();
            ls.Add("Yashu");
            ls.Add("Wardha");
            return ls;
        }
      
    }
}

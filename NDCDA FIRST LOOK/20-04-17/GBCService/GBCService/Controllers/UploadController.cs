using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using GBCService.Models;

namespace GBCService.Controllers
{
    public class UploadController : ApiController
    {
        public HttpResponseMessage Post()
        {            
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/VrImg");
            //string fileSaveLocation = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(HttpContext.Current.Server.MapPath("")).FullName).FullName, "VrImg");
            bool exists = System.IO.Directory.Exists(fileSaveLocation);

            if (!exists)
                System.IO.Directory.CreateDirectory(fileSaveLocation);

            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                Request.Content.ReadAsMultipartAsync(provider);
                string name = "";
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                    name = file.LocalFileName;
                }
                if (name != "" && name.Contains("."))
                {
                    string vrno = Path.GetFileName(name).Split('.')[0];
                    GBCCon gbc_con = new GBCCon();
                    OB obj = gbc_con.db_ob.SingleOrDefault(p => p.VrId == vrno);
                    obj.ImgName = Path.GetFileName(name);
                    gbc_con.SaveChanges();
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }
}

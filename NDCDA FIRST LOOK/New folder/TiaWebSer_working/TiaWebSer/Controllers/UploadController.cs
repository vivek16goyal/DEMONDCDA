using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using TiaWebSer.Models;

namespace TiaWebSer.Controllers
{
    public class UploadController : ApiController
    {

        public async Task<HttpResponseMessage> Post()
        {
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
            // data will be loaded.
           // string fileSaveLocation = HttpContext.Current.Server.MapPath("~/UploadedImage");
            string fileSaveLocation = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(HttpContext.Current.Server.MapPath("")).FullName).FullName, "VrImg");
            bool exists = System.IO.Directory.Exists(fileSaveLocation);

            if (!exists)
                System.IO.Directory.CreateDirectory(fileSaveLocation);

            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string name="";
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                    name = file.LocalFileName;
                }
                if (name != "" && name.Contains("."))
                {
                    string vrno = Path.GetFileName(name).Split('.')[0];
                    GBCDbConn gbc_con = new GBCDbConn();
                    OB obj = gbc_con.Db_ob.SingleOrDefault(p => p.VRNO == vrno);
                    obj.PresImg = Path.GetFileName(name);
                    gbc_con.SaveChanges();
                }
                

                // Send OK Response along with saved file names to the client.
                return Request.CreateResponse(HttpStatusCode.OK, files);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        public async Task<HttpResponseMessage> PostInside()
        {
            // Check whether the POST operation is MultiPart?
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
            // data will be loaded.
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/UploadedImage");
            //string fileSaveLocation = System.IO.Path.Combine(Directory.GetParent(Directory.GetParent(HttpContext.Current.Server.MapPath("")).FullName).FullName, "VrImg");
            bool exists = System.IO.Directory.Exists(fileSaveLocation);

            if (!exists)
                System.IO.Directory.CreateDirectory(fileSaveLocation);

            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();
            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);
                string name = "";
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                    name = file.LocalFileName;
                }
                
                // Send OK Response along with saved file names to the client.
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;

namespace TiaWebSer.Controllers
{
    public class DownloadController : ApiController
    {
        //public HttpResponseMessage Get([FromUri]string filename)
        //{
        //    string path = HttpContext.Current.Server.MapPath("~/" + filename);
            
        //    try
        //    {
        //        MemoryStream responseStream = new MemoryStream();
        //        Stream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
        //        bool fullContent = true;
        //        if (this.Request.Headers.Range != null)
        //        {
        //            fullContent = false;

        //            // Currently we only support a single range.
        //            RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


        //            // From specified, so seek to the requested position.
        //            if (range.From != null)
        //            {
        //                fileStream.Seek(range.From.Value, SeekOrigin.Begin);

        //                // In this case, actually the complete file will be returned.
        //                if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
        //                {
        //                    fileStream.CopyTo(responseStream);
        //                    fullContent = true;
        //                }
        //            }
        //            if (range.To != null)
        //            {
        //                // 10-20, return the range.
        //                if (range.From != null)
        //                {
        //                    long? rangeLength = range.To - range.From;
        //                    int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
        //                    byte[] buffer = new byte[length];
        //                    fileStream.Read(buffer, 0, length);
        //                    responseStream.Write(buffer, 0, length);
        //                }
        //                // -20, return the bytes from beginning to the specified value.
        //                else
        //                {
        //                    int length = (int)Math.Min(range.To.Value, fileStream.Length);
        //                    byte[] buffer = new byte[length];
        //                    fileStream.Read(buffer, 0, length);
        //                    responseStream.Write(buffer, 0, length);
        //                }
        //            }
        //            // No Range.To
        //            else
        //            {
        //                // 10-, return from the specified value to the end of file.
        //                if (range.From != null)
        //                {
        //                    if (range.From < fileStream.Length)
        //                    {
        //                        int length = (int)(fileStream.Length - range.From.Value);
        //                        byte[] buffer = new byte[length];
        //                        fileStream.Read(buffer, 0, length);
        //                        responseStream.Write(buffer, 0, length);
        //                    }
        //                }
        //            }
        //        }
        //        // No Range header. Return the complete file.
        //        else
        //        {
        //            fileStream.CopyTo(responseStream);
        //        }
        //        fileStream.Close();
        //        responseStream.Position = 0;

        //        HttpResponseMessage response = new HttpResponseMessage();
        //        response.StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent;
        //        response.Content = new StreamContent(responseStream);
        //        return response;
        //    }
        //    catch (IOException)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //    }
        //}

        public HttpResponseMessage Get()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            String filePath = HostingEnvironment.MapPath("~/VrImg/O216000006.jpeg");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            result.Content = new ByteArrayContent(memoryStream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return result;
        }

        public FileContentResult GetFile()
        {
            byte[] fileContents;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                String filePath = HostingEnvironment.MapPath("~/VrImg/O216000006.jpeg");
                using (Bitmap image = new Bitmap(WebRequest.Create(filePath).GetResponse().GetResponseStream()))
                    image.Save(memoryStream, ImageFormat.Jpeg);
                fileContents = memoryStream.ToArray();
            }
            return new FileContentResult(fileContents, "image/jpg");
        }
    }
}

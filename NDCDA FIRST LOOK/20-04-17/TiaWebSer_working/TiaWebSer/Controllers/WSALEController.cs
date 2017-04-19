using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using TiaWebSer.Models;

namespace TiaWebSer.Controllers
{
    public class WSALEController : ApiController
    {
        [HttpPost]
        public Order SaveBill(Order ord)
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

    }
}

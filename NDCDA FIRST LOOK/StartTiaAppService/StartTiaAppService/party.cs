using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartTiaAppService
{
    public class party
    {
        public string Pcode { get; set; }
        public string ServicePath { get; set; }
        public string InetAddress { get; set; }
        public string MCAddress { get; set; }
        public string Port { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? ValidUpToDate { get; set; }
        public DateTime? ServiceLastActiveDate { get; set; }
        public DateTime? AppLastActiveDate { get; set; }
        public int? PingCount { get; set; }
        public bool? IsActive { get; set; }
        public bool? ServiceStatus { get; set; }
        public string SqlServerName { get; set; }
        public bool? SqlServerAuth { get; set; }
        public string SqlUserId { get; set; }
        public string SqlPassword { get; set; }
        public string DataBaseName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string FDName { get; set; }
        public bool? IsDBConnected { get; set; }
    }
}

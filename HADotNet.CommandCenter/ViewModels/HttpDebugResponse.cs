using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.ViewModels
{
    public class HttpDebugResponse
    {
        public int Status { get; set; }
        public string StatusName => ((HttpStatusCode)Status).ToString();
        public string Headers { get; set; }
        public string Body { get; set; }
        public string NetworkStatus { get; set; }
    }
}

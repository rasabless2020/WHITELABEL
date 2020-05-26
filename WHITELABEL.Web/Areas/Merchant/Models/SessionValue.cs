using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.Areas.Merchant.Models
{
    public class SessionValue
    {
        public List<ListSession> SessionList { get; set; }
    } 

    public class ListSession
    {
        public string SessionID { get; set; }
        public string SessionType { get; set; }
    }
}
namespace WHITELABEL.Web.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;


    public class DomainNameCheck
    {
        public static bool DomainChecking(string domain, string host)
        {
            bool domaincheck = domain.Contains(host);
            if (domaincheck == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
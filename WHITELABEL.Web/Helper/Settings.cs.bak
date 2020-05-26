namespace WHITELABEL.Web.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using WHITELABEL.Data;
    using WHITELABEL.Data.Models;
    using WHITELABEL.Web.Models;
    using WHITELABEL.Web.Helper;

    public class Settings
    {
        public static string GetValue(string Key)
        {
            string Value = string.Empty;
            var db = new DBContext();
            var item = db.TBL_API_SETTING.Where(x => x.NAME == Key).FirstOrDefault();
            if (item != null)
            {
                Value = item.VALUE;
            }
            return Value;
        }
    }
}
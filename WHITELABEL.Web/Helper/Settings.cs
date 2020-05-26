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
    using System.Text.RegularExpressions;
    using System;

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
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }
    }
}
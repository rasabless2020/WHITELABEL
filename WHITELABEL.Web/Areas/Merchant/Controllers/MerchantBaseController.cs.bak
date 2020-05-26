using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantBaseController : Controller
    {
        public string getSession = null;
        public TBL_MASTER_MEMBER CurrentMerchant
        {
            get
            {
                if (Session["MerchantUserId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["MerchantUserId"];
                Session["MerchantId"] = UserId;
                getSession = UserId.ToString();

                var db = new DBContext();
                return db.TBL_MASTER_MEMBER.Find(UserId);
            }
            set
            {
                Session["MerchantUserId"] = value.MEM_ID;
            }
        }

        public List<string> GetTreeMember(long memId)
        {
            var db = new DBContext();
            List<string> SessionValue = new List<string>();
            //long valid = long.Parse(Session["MerchantId"].ToString());
            long memberId = memId;
            //var UserId = (long)Session["MerchantUserId"];
            var DistList = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memberId).FirstOrDefault();
            if (DistList != null)
            {
                var Userintroducer = Encrypt.EncryptMe(DistList.INTRODUCER.ToString())+","+ "Distributor";
                var user_intro = DistList.INTRODUCER;
                SessionValue.Add(Userintroducer.ToString());
                string DistType = "Distributor";
                var SuperDis = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == user_intro).FirstOrDefault();
                var SuperType = "Super";
                var superIntroducer = Encrypt.EncryptMe(SuperDis.INTRODUCER.ToString())+","+ "Super";
                var Super_intro = SuperDis.INTRODUCER;
                SessionValue.Add(superIntroducer.ToString());
                var SuperIntr= db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Super_intro).FirstOrDefault();
                var whiteLevelType = "White Level";
                var White_le= Encrypt.EncryptMe(SuperIntr.INTRODUCER.ToString())+"," + "White Level";
                var WhitelevIntro = SuperIntr.INTRODUCER;
                SessionValue.Add(White_le.ToString());
                var WLInto = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == WhitelevIntro).FirstOrDefault();
                
            }
            return SessionValue;
        }

        //// GET: Merchant/MerchantBase
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using System.Text;
namespace WHITELABEL.Web.Controllers
{
    public class PopOverController : Controller
    {
        // GET: PopOver
        public ActionResult Index()
        {
            var db = new DBContext();
            var emp_list = db.TBL_MASTER_MEMBER.ToList();
            ViewBag.AllEmp = emp_list;
            return View();
        }
        public ActionResult GetOperatorList()
        {
            var db = new DBContext();
            var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            ViewBag.operatorList = OperatorList;

            return View();
        }
    }
}
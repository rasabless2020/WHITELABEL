using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WHITELABEL.Web.Controllers
{
    public class InstantpaycallbackApiController : ApiController
    {
        [HttpGet]
        [Route("InstantPayCallBackAPIStatus")]
        public async Task<IHttpActionResult> InstantPayCallBackAPIStatus(string ipay_id, string agent_id, string opr_id, string status, string res_code, string res_msg)
        {
            try
            {
                var db = new DBContext();
                TBL_API_CALLBACK_LOGS objapi = new TBL_API_CALLBACK_LOGS()
                {
                    CORELATIONID = ipay_id,
                    SERVICE_TYPE = "",
                    TRANSACTION_NO = ipay_id,
                    OPERATOR_ID = opr_id,
                    STATUS = status,
                    RES_CODE = res_code,
                    RES_MSG = res_msg,
                    CALLBACK_DATETIME = DateTime.Now,
                    CALLBACK_DATAUPDATE = false,
                    CALLBACK_BY = "Instantpay"
                };
                db.TBL_API_CALLBACK_LOGS.Add(objapi);
                await db.SaveChangesAsync();
                return Ok(new { statuscode = res_code });
            }
            catch (Exception ex)
            {
                return Ok(new { statuscode = 1 });
                throw;
            }
        }

    }
}

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
    public class CyberPlatCallBackURLApiController : ApiController
    {
        [HttpGet]
        [Route("CyberPlatCallBackURLApi")]
        public async Task<IHttpActionResult> CyberPlatCallBackURLApiStatus(string CIPLTransID, string DateTime, string OperatorTransID, string DealerTransID, string ErrorDesc, string ErrorCode,string Signature)
        {
            try
            {
                var db = new DBContext();
                TBL_API_CALLBACK_LOGS objapi = new TBL_API_CALLBACK_LOGS()
                {
                    CORELATIONID = CIPLTransID,
                    SERVICE_TYPE = DealerTransID,
                    TRANSACTION_NO = CIPLTransID,
                    OPERATOR_ID = OperatorTransID,
                    STATUS = ErrorDesc,
                    RES_CODE = ErrorCode,
                    RES_MSG = Signature,
                    CALLBACK_DATETIME = Convert.ToDateTime(DateTime),
                    CALLBACK_DATAUPDATE = false,
                    CALLBACK_BY = "CyberPlatAPI"
                };
                db.TBL_API_CALLBACK_LOGS.Add(objapi);
                await db.SaveChangesAsync();
                return Ok(new { statuscode = ErrorCode });
            }
            catch (Exception ex)
            {
                return Ok(new { statuscode = 1 });
                throw;
            }
        }

    }
}

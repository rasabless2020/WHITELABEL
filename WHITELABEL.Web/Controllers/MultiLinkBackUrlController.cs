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
    public class MultiLinkBackUrlController : ApiController
    {
        [HttpGet]
        [Route("MultiLinkBackURLStatusChecking")]        
        public async Task<IHttpActionResult> MultiLinkBackURLStatusChecking(string accountId,string txid,string transtype)

        {
            try
            {
                var db = new DBContext();
                string accountid_value = accountId;
                string TransactionId = txid;
                string TransType = transtype;
                TBL_API_CALLBACK_LOGS objapi = new TBL_API_CALLBACK_LOGS()
                {
                    CORELATIONID = accountId,
                    SERVICE_TYPE = "",
                    TRANSACTION_NO = txid,
                    OPERATOR_ID = "",
                    STATUS = transtype,
                    RES_CODE = transtype,
                    RES_MSG = transtype,
                    CALLBACK_DATETIME = DateTime.Now,
                    CALLBACK_DATAUPDATE = false,
                    CALLBACK_BY = "Multilink"
                };
                db.TBL_API_CALLBACK_LOGS.Add(objapi);
                db.SaveChangesAsync();
                return Ok(new { message = transtype });
            }
            catch (Exception ex)
            {
                return Ok(new { message = 1 });
                throw;
            }
            
        }
        [HttpGet]
        [Route("InstantPayCallBackAPIStatus")]
        public async Task<IHttpActionResult> InstantPayCallBackAPIStatus(string ipay_id, string agent_id, string opr_id, string status, string res_code, string res_msg)
        {
            try
            {
                var db = new DBContext();
                return Ok(new { statuscode = res_code, status = status });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 1, message = ex.Message });
                throw;
            }
        }

    }
}

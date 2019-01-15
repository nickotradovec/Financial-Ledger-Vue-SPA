using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using Ledger.DataObjects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ledger.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AccountList : Controller {

        [HttpGet ("[action]")]
        public async Task<IActionResult> AccountsList () {
            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new AccountQuery (db);
                var result = await query.GetAllAccounts ();
                return new OkObjectResult (result);
            }
        }
    }

    [Route ("api/[controller]")]
    [ApiController]
    public class Account {

        public class AccountData {
            public Int32 accountId { set; get; }
            public string accountName { set; get; }
            public string commence { set; get; }
            public string cease { set; get; }
            public double initialBalance { set; get; }
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> GetAccountDetails ([FromQuery (Name = "id")] int id = 0) {
            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new AccountQuery (db);
                var result = await query.GetAccount (id);
                if (result == null)
                    return new NotFoundResult ();
                return new OkObjectResult (result);
            }
        }

        [HttpPost ("[action]")]
        public async Task<IActionResult> AddNewAccount (AccountData data) {

            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new AccountQuery (db);
                var result = await query.AddAccount (data.accountName, data.commence, data.cease, data.initialBalance);
                return new OkResult ();
            }
        }

        [HttpPost ("[action]")]
        public async Task<IActionResult> UpdateAccount (AccountData data) {

            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new AccountQuery (db);
                var result = await query.UpdateAccount (data.accountId, data.accountName, data.commence, data.cease, data.initialBalance);
                return new OkResult ();
            }
        }
    }
}
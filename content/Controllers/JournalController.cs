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
    public class JournalList : Controller {

        [HttpGet ("[action]")]
        public async Task<IActionResult> JournalEntries () {
            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new JournalQuery (db);
                var result = await query.GetAllJournals ();
                return new OkObjectResult (result);
            }
        }
    }

    [Route ("api/[controller]")]
    [ApiController]
    public class JournalMaintenance {

        [HttpGet ("{id}")]
        public async Task<IActionResult> GetJournalDetails ([FromQuery (Name = "id")] int id = 0) {
            using (var db = new AppDb ()) {
                await db.Connection.OpenAsync ();
                var query = new JournalQuery (db);
                var result = await query.GetJournal (id);
                if (result == null)
                    return new NotFoundResult ();
                return new OkObjectResult (result);
            }
        }

        /*[HttpPost ("[action]")]
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
        } */
    }
}
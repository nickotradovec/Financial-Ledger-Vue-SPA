using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Ledger.DataObjects {
    public class Journal {
        public int journalId { get; set; }
        public decimal journalAmount { get; set; }
        public DateTime journalDate { get; set; }
        public string journalName { get; set; }
        public string comment { get; set; }

        public List<JournalDetail> details { get; set; }

        [JsonIgnore]
        public string journalDate_string { get; set; }
        private AppDb Db { get; set; }
        public Journal (AppDb db = null) {
            Db = db;
        }
    }

    public class JournalDetail {
       
        public string debitCredit { get; set; }
        public decimal amount { get; set; }
        public Account account { get; set; }
    }
}
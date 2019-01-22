using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Ledger.DataObjects {
    public class Account {
        public int accountId { get; set; }
        public decimal initialBalance { get; set; }
        public DateTime commence { get; set; }
        public DateTime cease { get; set; }
        public string accountName { get; set; }

        [JsonIgnore]
        public String cease_string { get; set; }
        public String commence_string { get; set; }
        
        public Boolean IsValid (DateTime dtmCheck) {
            return (dtmCheck >= commence && dtmCheck <= cease);
        }

        private AppDb Db { get; set; }
        public Account (AppDb db = null) {
            Db = db;
        }

        public void SetDatesFromStrings () {
            commence = DateTime.Parse (commence_string);
            cease = DateTime.Parse (cease_string);
        }

        public void SetStringsFromDates () {
            commence_string = commence.ToString ("yyyy-MM-dd");
            cease_string = cease.ToString ("yyyy-MM-dd");
        }
    }
}
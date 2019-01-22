using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace Ledger.DataObjects {
    public class JournalQuery {
        private readonly AppDb Db;
        public JournalQuery (AppDb db) {
            Db = db;
        }

        public async Task<List<Journal>> GetAllJournals () {
            var cmd = Db.Connection.CreateCommand ();
            cmd.CommandText = "select * from journalEntries";
            return await ReadJournalsAsync (await cmd.ExecuteReaderAsync ());
        }

        public async Task<Journal> GetJournal (int id) {
            Console.WriteLine ($"Get Journal details for id: {id.ToString()}");
            var cmd = Db.Connection.CreateCommand () as MySqlCommand;
            cmd.CommandText = @"SELECT * from journalEntries where journalid = @journalid";
            cmd.Parameters.Add (new MySqlParameter { ParameterName = "@journalid", DbType = DbType.Int32, Value = id, });
            var vchQuery = await ReadJournalsAsync (await cmd.ExecuteReaderAsync ());
            
            if (vchQuery.Count <= 0) { return null;}
            Journal vch = vchQuery[0];

            cmd.CommandText = @"select jd.debitCredit, jd.amount, jd.accountId, a.name from journalDetails jd inner join accounts a on jd.accountId = a.accountId where jd.journalId = @journalId";
            vch.details = await ReadJournalDetailsAsync (await cmd.ExecuteReaderAsync ());
            return vch;
        }

        /*public async Task<Boolean> AddAccount (string name, string commence, string cease, double initialBalance) {
            var cmd = Db.Connection.CreateCommand () as MySqlCommand;
            cmd.CommandText = @"insert into accounts (name, commence, cease, initialBalance) values(@name, @commence, @cease, @initialBalance)";
            AddParameter(cmd, @"name", DbType.String, name);
            AddParameter(cmd, @"commence", DbType.Date, DateTime.Parse(commence));
            AddParameter(cmd, @"cease", DbType.Date, DateTime.Parse(cease));
            AddParameter(cmd, @"initialBalance", DbType.Double, Math.Round(initialBalance, 2));
            await cmd.ExecuteReaderAsync ();
            return true;
        }

        public async Task<Boolean> UpdateAccount (Int32 accountid, string name, string commence, string cease, double initialBalance) {
            var cmd = Db.Connection.CreateCommand () as MySqlCommand;
            cmd.CommandText = @"update accounts set commence=@commence, cease=@cease, name=@name, initialBalance=@initialBalance where accountId=@accountid";
            AddParameter(cmd, @"accountid", DbType.Int32, accountid);
            AddParameter(cmd, @"name", DbType.String, name);
            AddParameter(cmd, @"commence", DbType.Date, DateTime.Parse(commence));
            AddParameter(cmd, @"cease", DbType.Date, DateTime.Parse(cease));
            AddParameter(cmd, @"initialBalance", DbType.Double, Math.Round(initialBalance, 2));
            await cmd.ExecuteReaderAsync ();
            return true;
        } */

        private async Task<List<Journal>> ReadJournalsAsync (DbDataReader reader) {
            var journals = new List<Journal> ();
            using (reader) {
                while (await reader.ReadAsync ()) {
                    var post = new Journal (Db) {
                        journalId = await reader.GetFieldValueAsync<int> (0),
                        journalName = await reader.GetFieldValueAsync<string> (1),
                        journalDate = await reader.GetFieldValueAsync<DateTime> (2),
                        comment = await reader.GetFieldValueAsync<string> (3),
                        journalAmount = await reader.GetFieldValueAsync<decimal> (4)
                    };
                    journals.Add (post);
                }
            }
            return journals;
        }

        private async Task<List<JournalDetail>> ReadJournalDetailsAsync (DbDataReader reader) {
            var journals = new List<JournalDetail> ();
            using (reader) {
                while (await reader.ReadAsync ()) {

                    var post = new JournalDetail {
                        debitCredit = reader.GetFieldValue<string> (0),
                        amount = await reader.GetFieldValueAsync<decimal> (1),
                        account = new Account {
                            // just need our id and name here.
                            accountId = await reader.GetFieldValueAsync<int> (2),
                            accountName = await reader.GetFieldValueAsync<string> (3)
                        }
                    };
                    journals.Add (post);
                }
            }
            return journals;
        }

        private void AddParameter (MySqlCommand cmd, string paramName, DbType dbType, object value) {
            cmd.Parameters.Add (new MySqlParameter {
                    ParameterName = paramName,
                    DbType = dbType,
                    Value = value,
            });
        }
    }
}
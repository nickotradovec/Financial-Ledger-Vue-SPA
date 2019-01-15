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
    public class AccountQuery {
        private readonly AppDb Db;
        public AccountQuery (AppDb db) {
            Db = db;
        }

        public async Task<List<Account>> GetAllAccounts () {
            var cmd = Db.Connection.CreateCommand ();
            cmd.CommandText = "select * from accounts";
            return await ReadAccountsAsync (await cmd.ExecuteReaderAsync ());
        }

        public async Task<Account> GetAccount (int id) {
            Console.WriteLine ($"Get Account details for id: {id.ToString()}");
            var cmd = Db.Connection.CreateCommand () as MySqlCommand;
            //cmd.CommandText = @"SELECT * from accounts where accountid = 1";
            cmd.CommandText = @"SELECT * from accounts where accountid = @accountid";
            cmd.Parameters.Add (new MySqlParameter {
                ParameterName = "@accountid",
                    DbType = DbType.Int32,
                    Value = id,
            });
            cmd.Prepare ();
            var result = await ReadAccountsAsync (await cmd.ExecuteReaderAsync ());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<Boolean> AddAccount (string name, string commence, string cease, double initialBalance) {
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
        }

        private async Task<List<Account>> ReadAccountsAsync (DbDataReader reader) {
            var accounts = new List<Account> ();
            using (reader) {
                while (await reader.ReadAsync ()) {
                    var post = new Account (Db) {
                        AccountId = await reader.GetFieldValueAsync<int> (0),
                        AccountName = await reader.GetFieldValueAsync<string> (3),
                        Commence = await reader.GetFieldValueAsync<DateTime> (1),
                        Cease = await reader.GetFieldValueAsync<DateTime> (2),
                        InitialAccountBalance = await reader.GetFieldValueAsync<decimal> (4)
                    };
                    accounts.Add (post);
                }
            }
            return accounts;
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
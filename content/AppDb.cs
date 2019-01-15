using System;
using MySql.Data;
using MySql.Data.MySqlClient;
public class AppDb : IDisposable {
    public MySqlConnection Connection;

    public AppDb () {
        // TODO: grab this from the JSON config durr
        Connection = new MySqlConnection (@"Server=localhost;Port=3306;Database=ledger;Uid=testapp1;Pwd=nopassword");
    }

    public AppDb (string connectionString) {
        Connection = new MySqlConnection (connectionString);
    }

    public void Dispose () {
        Connection.Close ();
    }
}
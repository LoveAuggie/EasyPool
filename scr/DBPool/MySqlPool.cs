using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyPool;
using MySql.Data.MySqlClient;

namespace EasyPool
{
    public class MySqlPool : PoolObject<MySqlConnection>
    {
        public string ConnectString { get; private set; }
        public MySqlPool(string connStr)
        {
            ConnectString = connStr;
        }

        public override MySqlConnection Create()
        {
            var conn = new MySqlConnection(ConnectString);
            conn.Open();
            return conn;
        }

        public override bool CanUse(MySqlConnection conn)
        {
            return conn.State == System.Data.ConnectionState.Open;
        }

        public override bool CloseAndDispose(MySqlConnection conn)
        {
            conn.Close();
            conn.Dispose();
            return true;
        }
    }
}

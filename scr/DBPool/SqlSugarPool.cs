using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace EasyPool
{
    public class SqlSugarPool : PoolObject<SqlSugarClient>
    {
        public string ConnectString { get; private set; }

        public DbType DBType { get; private set; }

        public SqlSugarPool(string ConnStr, DbType sugarType)
        {
            this.ConnectString = ConnStr;
            this.DBType = sugarType;
        }

        public override bool CanUse(SqlSugarClient conn)
        {
            return true;
        }

        public override bool CloseAndDispose(SqlSugarClient conn)
        {
            conn.Close();
            return true;
        }

        public override SqlSugarClient Create()
        {
            var config = new ConnectionConfig() 
            {
                IsAutoCloseConnection = false,
                DbType = this.DBType,
                ConnectionString = this.ConnectString
            };

            var conn =  new SqlSugarClient(config);
            conn.Open();
            return conn;
        }
    }
}

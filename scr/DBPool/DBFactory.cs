using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SqlSugar;

namespace EasyPool
{
    public class DBFactory
    {
        public static MyPool<MySqlConnection> CreateMySqlPool(string connStr)
        {
            MySqlPool msp = new MySqlPool(connStr);
            return new MyPool<MySqlConnection>(msp);
        }

        public static MyPool<SqlSugarClient> CreateSqlSugarPool(string connStr, DbType sugarType)
        {
            SqlSugarPool ssp = new SqlSugarPool(connStr, sugarType);
            return new MyPool<SqlSugarClient>(ssp);
        }
    }
}

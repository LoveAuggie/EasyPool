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
        /// <summary>
        /// 创建Mysql连接池
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="maxCount">最大可用连接数（0为不限制）</param>
        /// <param name="TimeOut">连接过期时间（秒）</param>
        /// <returns></returns>
        public static MyPool<MySqlConnection> CreateMySqlPool(string connStr, int maxCount = 0, int TimeOut = 600)
        {
            MySqlPool msp = new MySqlPool(connStr);
            return new MyPool<MySqlConnection>(msp, maxCount, TimeOut);
        }

        /// <summary>
        /// 创建SqlSugar连接池
        /// </summary>
        /// <param name="connStr">连接字符串</param>
        /// <param name="sugarType">数据库类型</param>
        /// <param name="maxCount">最大可用连接数（0为不限制）</param>
        /// <param name="TimeOut">连接过期时间（秒）</param>
        /// <returns></returns>
        public static MyPool<SqlSugarClient> CreateSqlSugarPool(string connStr, DbType sugarType, int maxCount = 0, int TimeOut = 600)
        {
            SqlSugarPool ssp = new SqlSugarPool(connStr, sugarType);
            return new MyPool<SqlSugarClient>(ssp, maxCount, TimeOut);
        }
    }
}

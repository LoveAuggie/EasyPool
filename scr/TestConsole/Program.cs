using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyPool;
using MySql.Data.MySqlClient;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MyPool<MySqlConnection> pool = new MyPool<MySqlConnection>(new Mycon());

            for (int i = 0; i < 20; i++)
            {
                int curTask = i;
                Task.Factory.StartNew(() => 
                {
                    var tasknum = curTask;
                    int sum = 0;
                    while (sum < 100) 
                    {
                        pool.Run((db) => 
                        {
                            using (var cmd = db.CreateCommand())
                            {
                                cmd.CommandText = "select * from sys_menus";
                                using (var dr = cmd.ExecuteReader())
                                {
                                    Console.Write(".");
                                }
                            }
                            System.Threading.Thread.Sleep(100);
                        });
                    }
                });
            }

            while (true)
            {
                Console.ReadKey();
            }
        }
    }

    class Mycon : PoolObject<MySqlConnection>
    {
        private static int TotalCount=0;

        private string ConnStr;
        public Mycon() 
        {
            ConnStr = "Database=jcpt_auth;Data Source=10.1.55.44;Port=3306;User Id=auggie;Password=123456;Charset=utf8;";
        }

        public override bool CanUse(MySqlConnection t)
        {
            return t.State == System.Data.ConnectionState.Open;
        }

        public override bool CloseAndDispose(MySqlConnection t)
        {
            t.Close();
            t.Dispose();
            return true;
        }

        public override MySqlConnection Create()
        {
            Console.WriteLine($"CreateNum => {++TotalCount}");
            var con = new MySqlConnection(ConnStr);
            con.Open();
            return con;
        }
    }
}

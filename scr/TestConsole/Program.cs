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
        static int TCount = 0;
        static MyPool<MySqlConnection> pool;
        static void Main(string[] args)
        {
            pool = DBFactory.CreateMySqlPool("Database=jcpt_auth;Data Source=10.1.55.44;Port=3306;User Id=auggie;Password=123456;Charset=utf8;Pooling=false", TimeOut:20);

            for (int i = 0; i < 20; i++)
            {
                int curTask = i;
                Task.Factory.StartNew(() => 
                {
                    RunAct(curTask);
                });
            }
            bool first = false;
            while (true)
            {
                Console.WriteLine("当前连接数：" + pool.CurrentCount);

                if (pool.CurrentCount == 20)
                {
                    first = true;
                }
                if (first && pool.CurrentCount < 5)
                {
                    first = false;
                    for (int i = 0; i < 10; i++)
                    {
                        int curTask = i;
                        Task.Factory.StartNew(() =>
                        {
                            RunAct(curTask);
                        });
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void RunAct(int curTask)
        {
            Console.Title = $"当前线程数：{++TCount}";
            var tasknum = curTask;
            int sum = 0;
            var total = (curTask + 1) * 100;
            while (sum++ < total)
            {
                pool.Run((db) =>
                {
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandText = "select * from sys_menus";
                        using (var dr = cmd.ExecuteReader())
                        {
                            //Console.Write(".");
                        }
                    }
                    if (sum < 100)
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1);
                    }
                });
            }

            Console.Title = $"当前线程数：{--TCount}";
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

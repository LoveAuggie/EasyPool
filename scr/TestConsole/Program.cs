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

            pool.Run((con) => 
            {

            });
        }
    }

    class Mycon : PoolObject<MySqlConnection>
    {
        public override bool CanUse(MySqlConnection t)
        {
            throw new NotImplementedException();
        }

        public override bool CloseAndDispose(MySqlConnection t)
        {
            throw new NotImplementedException();
        }

        public override MySqlConnection Create()
        {
            throw new NotImplementedException();
        }
    }
}

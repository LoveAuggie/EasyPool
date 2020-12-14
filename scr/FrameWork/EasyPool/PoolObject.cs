using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPool
{
    public abstract class PoolObject<T>
    {
        public DateTime CreateTime = DateTime.Now;

        internal T Instance { get; set; }

        public abstract T Create();

        public abstract bool CanUse(T t);

        public abstract bool CloseAndDispose(T t);
    }
}

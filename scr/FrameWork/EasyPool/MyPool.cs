using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPool
{
    public class MyPool<T> where T: class
    {
        public ConcurrentQueue<T> list = new ConcurrentQueue<T>();

        public ConcurrentDictionary<int, T> used = new ConcurrentDictionary<int, T>();

        private PoolObject<T> _Pobject = null;

        public MyPool(PoolObject<T> pObjcet)
        {
            _Pobject = pObjcet;
        }

        public void Run(Action<T> act)
        {
            T t = default(T);
            try
            {
                t = this.Get();
                act.Invoke(t);
            }
            catch (Exception)
            {
                if (t != default(T))
                {
                    _Pobject.CanUse(t);
                    if (!_Pobject.CanUse(t))
                    {
                        var code = t.GetHashCode();
                        used.TryRemove(code, out T temp);
                        t = default(T);
                    }
                }
            }
            finally
            {
                if (t != default(T))
                {
                    Return(t);
                }
            }
        }

        private T Get()
        {
            if (!list.TryDequeue(out T t))
            {
                t = _Pobject.Create();
            }
            used.TryAdd(t.GetHashCode(), t);
            return t;
        }

        private void Return(T poolObject)
        {
            var code = poolObject.GetHashCode();
            used.TryRemove(code, out T temp);

            list.Enqueue(poolObject);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EasyPool
{
    public class MyPool<T> where T : class
    {
        private object LockObj = new object();

        /// <summary>
        /// 当前列表
        /// </summary>
        private List<SingelObject<T>> list = new List<SingelObject<T>>();

        /// <summary>
        /// 使用中的数据列表
        /// </summary>
        private List<SingelObject<T>> used = new List<SingelObject<T>>();

        private PoolObject<T> _Pobject = null;

        // 默认超时时间60秒
        private int _TimeOut = 10 * 60;

        private int _MaxCount;

        private Timer OutCheckTimer;

        public int CurrentCount
        {
            get
            {
                return list.Count + used.Count;
            }
        }

        public MyPool(PoolObject<T> pObjcet, int MaxCount = 0, int TimeOut = 600)
        {
            _Pobject = pObjcet;
            _MaxCount = MaxCount;
            _TimeOut = TimeOut;

            OutCheckTimer = new Timer();
            OutCheckTimer.Elapsed += OutCheckTimer_Elapsed;
            OutCheckTimer.Interval = 1000;
            OutCheckTimer.Enabled = true;
        }

        private void OutCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (LockObj)
                {
                    OutCheckTimer.Enabled = false;

                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        var st = list[i];
                        if (st.LastUseTime.AddSeconds(_TimeOut) < DateTime.Now)
                        {
                            list.RemoveAt(i);
                            _Pobject.CloseAndDispose(st.Obj);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                OutCheckTimer.Enabled = true;
            }
        }

        public bool Run(Action<T> act)
        {
            SingelObject<T> st = null;
            st = this.Get();
            T t = default(T);
            if (st == null)
            {
                return false;
            }

            try
            {
                t = st.Obj;
                act.Invoke(t);
            }
            catch { }
            finally
            {
                this.Return(st);
            }
            return true;
        }

        private SingelObject<T> Get()
        {
            lock (LockObj)
            {
                if (this._MaxCount > 0 && this.CurrentCount >= this._MaxCount)
                    return null;

                int sum = 0;
                while (sum++ <= 5)
                {
                    SingelObject<T> st = null;
                    if (list.Count > 0)
                    {
                        st = list.First();
                        list.RemoveAt(0);
                    }
                    else
                    {
                        var t = _Pobject.Create();

                        st = new SingelObject<T>()
                        {
                            Obj = t
                        };
                    }
                    if (st.Obj != default(T) && _Pobject.CanUse(st.Obj))
                    {
                        st.LastUseTime = DateTime.Now;
                        used.Add(st);

                        return st;
                    }
                    else
                    {
                        list.Remove(st);
                    }
                    System.Threading.Thread.Sleep(200);
                }
                return null;
            }
        }

        private void Return(SingelObject<T> poolObject)
        {
            lock (LockObj)
            {
                used.Remove(poolObject);

                /* 每次都放在最前面，这样若是需要的连接数比较少，
                 * 而创建的连接数又比较多的话，会导致队列尾的若干个连接一直不会被用到
                 * 从而可以被检查线程自动释放掉
                */
                list.Insert(0, poolObject);
            }
        }
    }

    internal class SingelObject<T>
    {
        public T Obj;

        public DateTime LastUseTime;
    }
}

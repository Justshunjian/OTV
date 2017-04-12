using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using Quartz;
using Quartz.Impl;

namespace Otv.Utils
{
    /// <summary>
    /// 弹幕Queue管理类
    /// </summary>
    public class QueueManager
    {
        public int QueueSize { get; set; }

        public static List<string> removeKeys = new List<string>();

        #region 清理内存数据 release
        /// <summary>
        /// 清理内存数据
        /// </summary>
        public static void release()
        {
            removeKeys.Clear();
            sFiltersList.Clear();
            sPoolList.Clear();
            m_DanMuMap.Clear();
        } 
        #endregion

        #region 内置过滤词
        private static List<string> sFiltersList = new List<string>();
        public static bool loadFilers()
        {
            bool ret = false;
            try
            {
                using (UsersContext context = new UsersContext())
                {
                    List<T_Filter> list = context.T_Filters.Where(t => t.State).ToList();
                    sFiltersList.Clear();
                    list.ForEach(t =>
                    {
                        sFiltersList.Add(t.Value);
                    });

                    ret = true;
                }
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 判断是否包含非法词
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool filter(string content)
        {
            try
            {
                var list = sFiltersList.Where(t => content.Contains(t) == true).ToList();
                if (list != null && list.Count > 0)
                {
                    return false;
                }
            }catch(Exception){
                return false;
            }
            
            return true;
        }
        #endregion

        #region 内置弹幕数据
        private static List<string> sPoolList = new List<string>();

        /// <summary>
        /// 从数据库加载预置弹幕数据
        /// </summary>
        public static bool loadPool()
        {
            bool ret = false;
            try
            {
                using (UsersContext context = new UsersContext())
                {
                    List<T_DanMu> list = context.T_DanMus.Where(t=>t.State).ToList();
                    sPoolList.Clear();
                    list.ForEach(t =>
                    {
                        sPoolList.Add(t.Value);
                    });

                    ret = true;
                }
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }

        #endregion

        #region 弹幕Queue单例设计模式
        /// <summary>
        /// 节目弹幕字典数据
        /// </summary>
        private static Dictionary<String,Queue<DanmuBean>> m_DanMuMap;
        private Queue<DanmuBean> m_Queue;
        /// <summary>
        /// 线程对象
        /// </summary>
        private readonly Thread m_thread;
        /// <summary>
        /// 线程信号量
        /// </summary>
        private readonly ManualResetEvent m_threadEvent;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly Object m_lockObj;
  
        private static readonly QueueManager instance = new QueueManager();

        private QueueManager()
        {
            this.m_lockObj = new object();
            this.m_threadEvent = new ManualResetEvent(false);

            m_Queue = new Queue<DanmuBean>();

            //读数据库
            using(UsersContext context = new UsersContext ()){
                T_Setting setting = context.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.OTV_REQ_COUNT));
                QueueSize = Int32.Parse(setting.Value);
            }

            //初始化队列长度
            m_DanMuMap = new Dictionary<String, Queue<DanmuBean>>();

            //启动弹幕数据接收线程
            this.m_thread = new Thread(this.execute);
            this.m_thread.Start();

            //启动定时刷新m_DanMuMap中的数据
            initQuartz();
        }
        #endregion

        #region 弹幕数据处理线程 execute
        /// <summary>
        /// 弹幕数据处理线程
        /// </summary>
        private void execute()
        {
            do
            {
                //线程挂起，等待接收数据
                this.m_threadEvent.WaitOne();
                //从链表中取出邮件发送方的数据

                try
                {
                    lock (this.m_lockObj)
                    {
                        DanmuBean bean = this.m_Queue.Dequeue();
                        if (bean != null)
                        {
                            try
                            {
                                //判断节目是否存在
                                if (m_DanMuMap.Keys.Contains(bean.Cuid))
                                {
                                    Queue<DanmuBean> queue;
                                    if (m_DanMuMap.TryGetValue(bean.Cuid, out queue))
                                    {
                                        if (queue.Count >= QueueSize)
                                        {
                                            queue.Dequeue();
                                        }
                                        queue.Enqueue(bean);
                                    }
                                }
                                else
                                {
                                    Queue<DanmuBean> queue = new Queue<DanmuBean>(QueueSize);
                                    queue.Enqueue(bean);
                                    m_DanMuMap.Add(bean.Cuid, queue);
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                        //终止线程
                        if (this.m_Queue.Count == 0)
                        {
                            this.m_threadEvent.Reset();
                        }
                    }

                }
                catch (Exception)
                {

                }
            } while (true);
        } 
        #endregion

        #region 获取实例 Instance
        /// <summary>
        /// 获取实例
        /// </summary>
        public static QueueManager Instance { get { return instance; } }
        #endregion

        #region 定时丢弃DanMuMap中过期的数据 initQuartz

        public class DanmuJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                //执行任务
                DanMuMapUpdate();
            }
        } 

         private void initQuartz()
         {
             //工厂
            ISchedulerFactory factory = new StdSchedulerFactory();
            //启动
            IScheduler scheduler = factory.GetScheduler();

            //启动
            scheduler.Start();

            //描述工作
            IJobDetail job = JobBuilder.Create<DanmuJob>().WithIdentity("Danmujob", "Danmujobs").Build();


             // Trigger the job to run now, and then repeat every 10 minutes
            ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("Danmutrigger", "Danmujobs")
                    .WithSimpleSchedule(x=>x.WithIntervalInMinutes(10).RepeatForever())
                    .Build();

            //执行
            scheduler.ScheduleJob(job, trigger);
         }

        /// <summary>
        /// 定时丢弃DanMuMap中过期的数据
        /// </summary>
        public static void DanMuMapUpdate()
        {
            int size = m_DanMuMap.Count;

            int splitNum = 2;
            int threadNum = size / splitNum + 1;
            int j = 0;
            for (int i = 0; i < threadNum; i++)
            {
                int n = i * splitNum;
                Dictionary<String, Queue<DanmuBean>> map = new Dictionary<string, Queue<DanmuBean>>();

                foreach (KeyValuePair<String, Queue<DanmuBean>> kvp in m_DanMuMap)
                {
                    if (j >= n)
                    {
                        map.Add(kvp.Key, kvp.Value);
                        j++;
                        if (j >= splitNum || j >= size)
                        {
                            j = 0;
                            UpdateThread updateThread = new UpdateThread(map);
                            Thread t = new Thread(updateThread.execute);
                            t.Priority = ThreadPriority.Highest;
                            t.Start();
                            t.Join();
                            break;
                        }
                    }

                    j++;
                }
            }

            //移除
            foreach (string key in removeKeys)
            {
                m_DanMuMap.Remove(key);
            }

            removeKeys.Clear();
        }

        public class UpdateThread
        {
            private Dictionary<String, Queue<DanmuBean>> map;
            public UpdateThread(Dictionary<String, Queue<DanmuBean>> m)
            {
                this.map = m;
            }

            public void execute()
            {
                foreach (KeyValuePair<String, Queue<DanmuBean>> kvp in this.map)
                {
                    if (kvp.Value.Count == 0)
                    {
                        //m_DanMuMap.Remove(kvp.Key);
                        removeKeys.Add(kvp.Key);
                        continue;
                    }

                    DanmuBean[] beans = kvp.Value.ToArray();
                    foreach (DanmuBean b in beans)
                    {
                        TimeSpan ts = DateTime.Now - b.DateTime;
                        if (ts.Minutes >= 10)
                        {
                            kvp.Value.Dequeue();
                        }
                    }

                    if (kvp.Value.Count == 0)
                    {
                        //m_DanMuMap.Remove(kvp.Key);
                        removeKeys.Add(kvp.Key);
                        continue;
                    }
                }
            }
        } 
        #endregion

        #region 队列新增数据 +Put
        /// <summary>
        /// 队列新增数据
        /// </summary>
        /// <param name="bean">弹幕数据实体Bean</param>
        public void Put(DanmuBean bean)
        {
            lock (this.m_lockObj)
            {
                this.m_Queue.Enqueue(bean); ;
                if (this.m_Queue.Count != 0)
                {
                    this.m_threadEvent.Set();
                }
            }
        } 
        #endregion

        #region 获取数据列表 +Get
        /// <summary>
        /// 获取数据列表
        /// </summary>
        public List<DanmuBean> Get(string chnId,string uid)
        {
            List<DanmuBean> list = new List<DanmuBean> ();
            Queue<DanmuBean> queue;
            try
            {
                if (m_DanMuMap.TryGetValue(chnId, out queue))
                {
                    //list = queue.Where(t=>t.Uid!=uid).Take(QueueSize).ToArray().AsQueryable().ToList();
                    list = queue.Take(QueueSize).ToArray().AsQueryable().ToList();
                    if (list.Count == 0)//使用内置的弹幕数据
                    {
                        //随机条数
                        Random ran = new Random();
                        int size = sPoolList.Count > QueueSize ? QueueSize : sPoolList.Count;
                        int RandKey = ran.Next(1, size);
                        int len = sPoolList.Count;
                        for (int i = 0; i < RandKey; i++)
                        {
                            int idx = ran.Next(len);

                            DanmuBean bean = new DanmuBean()
                            {
                                Content = sPoolList[idx]
                            };
                            list.Add(bean);
                        }
                    }
                }
                else
                {
                    //随机条数
                    Random ran = new Random();
                    int size = sPoolList.Count > QueueSize ? QueueSize : sPoolList.Count;
                    int RandKey = ran.Next(1, size);
                    int len = sPoolList.Count;
                    for (int i = 0; i < RandKey; i++)
                    {
                        int idx = ran.Next(len);
                        DanmuBean bean = new DanmuBean()
                        {
                            Content = sPoolList[idx]
                        };
                        list.Add(bean);
                    }
                }

                list = list.Distinct(new MyDistinct()).ToList();
            }
            catch (Exception)
            {
            }

            return list;

        }
        #endregion


        public class MyDistinct : IEqualityComparer<DanmuBean>
        {
            public bool Equals(DanmuBean x, DanmuBean y)
            {
                if (x.Content.ToLower() == y.Content.ToLower())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(DanmuBean obj)
            {
                return 0;
            }
        } 
    }
}
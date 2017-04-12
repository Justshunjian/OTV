using Otv.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otv.Utils
{
    /// <summary>
    /// 文件名:QuartzUtils.cs
    ///	功能描述:Quartz定时工具类
    ///
    /// 作者:吕凤凯
    /// 创建时间:2016/6/2 10:42:24
    /// 
    /// </summary>
    class QuartzUtils
    {
        private static IScheduler scheduler = null;

        public class EmailJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                //执行任务
                hotChnReset();
            }

            #region 清除所有节目热度值
            private void hotChnReset()
            {
                using (UsersContext context = new UsersContext())
                {
                    context.Database.ExecuteSqlCommand("update [T_HotChns] set [HeatValue] = 0", new SqlParameter[] { });
                }
            } 
            #endregion
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="type">0：每天,1：每周</param>
        /// <param name="weekday">星期几</param>
        /// <param name="hour">小时</param>
        /// <param name="minute">分钟</param>
        public static bool init(int type, string weekday, int hour, int minute)
        {
            try
            {
                //释放定时器
                release();

                //工厂
                ISchedulerFactory factory = new StdSchedulerFactory();
                //启动
                if (scheduler == null)
                    scheduler = factory.GetScheduler();

                //启动
                scheduler.Start();

                //描述工作
                IJobDetail job = JobBuilder.Create<EmailJob>().WithIdentity("Ipjob", "Ipjobs").Build();
                //触发器
                ITrigger trigger = null;
                if (type == 0)
                {//每天定时执行
                    trigger = TriggerBuilder.Create()
                    .WithIdentity("Iptrigger", "Ipjobs")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute))//每天8:40执行
                    .Build();
                }
                else if (type == 1)
                {//每周星期几定时执行
                    DayOfWeek wday = (DayOfWeek)System.Enum.Parse(typeof(DayOfWeek), weekday);
                    trigger = TriggerBuilder.Create()
                    .WithIdentity("Iptrigger", "Ipjobs")
                    .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(wday, hour, minute))//每周相应时间执行
                    .Build();
                }

                //执行
                scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否定时器
        /// </summary>
        public static void release()
        {
            if (scheduler != null && !scheduler.IsShutdown)
            {
                scheduler.Shutdown(true);
                scheduler = null;
            }
        }

        #region 加载定时器 load
        /// <summary>
        /// 加载定时器
        /// </summary>
        public static bool load()
        {
            bool ret = false;
            try
            {
                using (UsersContext context = new UsersContext())
                {
                    T_Setting setting = context.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_HOUR));
                    int hour = Int32.Parse(setting.Value);
                    setting = context.T_Settings.Find(SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_MINUTE));
                    int minute = Int32.Parse(setting.Value);
                    ret = init(0, DayOfWeek.Monday.ToString(), hour, minute);
                }
            }
            catch (Exception)
            {

            }
            return ret;
        }
        #endregion

    }
}
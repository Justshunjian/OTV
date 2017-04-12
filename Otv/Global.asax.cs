using Otv.Models;
using Otv.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Otv
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            init();

            System.Diagnostics.EventLog.WriteEntry("OTV启动", "OTV Web应用程序启动", System.Diagnostics.EventLogEntryType.Information);
        }

        protected void Application_End()
        {
            System.Diagnostics.EventLog.WriteEntry("OTV关闭", "OTV Web应用程序关闭", System.Diagnostics.EventLogEntryType.Information);

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void init()
        {
            //清理数据
            QueueManager.release();
            CuidUtils.release();

            //删除表[T_HotChns] 和 [T_Records]数据
            using (UsersContext db = new UsersContext())
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                db.Database.ExecuteSqlCommand(string.Format("select * into T_HotChns_{0} from T_HotChns", timestamp));
                db.Database.ExecuteSqlCommand("delete from T_HotChns");
                db.Database.ExecuteSqlCommand("delete from T_Records");
            }

            //启动节目热度定时清零设置
            if (!QuartzUtils.load())
            {
                System.Diagnostics.EventLog.WriteEntry("OTV定时设置", "OTV 节目热度定时清除设置失败", System.Diagnostics.EventLogEntryType.Information);
            }

            if (!QueueManager.loadPool())
            {
                System.Diagnostics.EventLog.WriteEntry("OTV预置弹幕评论读取", "OTV 预置Pool实时更新失败", System.Diagnostics.EventLogEntryType.Information);
            }

            if (!QueueManager.loadFilers())
            {
                System.Diagnostics.EventLog.WriteEntry("OTV预置过滤词读取", "OTV 预置Filter实时更新失败", System.Diagnostics.EventLogEntryType.Information);
            }
        }
    }
}
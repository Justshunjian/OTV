using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using Otv.Models;
using Otv.Utils;
using WebMatrix.WebData;
using System.Linq;

namespace DanMu.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 确保每次启动应用程序时只初始化一次 ASP.NET Simple Membership
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                //Database.SetInitializer<UsersContext>(null);
                ///执行策略，如果数据库不存在则创建初始数据库
                Database.SetInitializer<UsersContext>(new CreateDatabaseIfNotExists<UsersContext>());

                try
                {
                    using (var context = new UsersContext())
                    {
                        bool bExist = context.Database.Exists();
                        if (!bExist)
                        {
                            // 创建不包含 Entity Framework 迁移架构的 SimpleMembership 数据库
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                        //创建权限
                        if (context.T_Roles.ToList().Count <= 0)
                        {
                            List<T_Role> roles = new List<T_Role>
                            {
                                new T_Role{RoleName = "admin"}
                            
                            };
                            DbSet<T_Role> RoleSet = context.Set<T_Role>();
                            RoleSet.AddOrUpdate(m => m.RoleName, roles.ToArray());
                            context.SaveChanges();
                        }

                        //创建设置初始数据
                        if (context.T_Settings.ToList().Count < 4)
                        {
                            List<T_Setting> settings = new List<T_Setting>
                            {
                                new T_Setting{Key = SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_COUNT),Value="100"},
                                new T_Setting{Key = SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.OTV_REQ_COUNT),Value="15"},
                                new T_Setting{Key = SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_HOUR),Value="9"},
                                new T_Setting{Key = SettingUtils.GetEnumStr(Otv.Utils.SettingUtils.SettingKeys.HOT_CHN_RESET_QUARTZ_MINUTE),Value="10"},

                            };
                            DbSet<T_Setting> SettingSet = context.Set<T_Setting>();
                            SettingSet.AddOrUpdate(t=>t.Key,settings.ToArray());
                            context.SaveChanges();
                        }
                        
                        //创建管理员
                        if (context.T_Users.ToList().Count <= 0)
                        {
                            List<T_User> users = new List<T_User>
                            {
                                new T_User{ User = "admin",Pwd=MD5Utils.encode("ws280-1"),CreateDate=DateTime.Now,Email="FengKai.Lv@availink.com",RoleID=0,Desc="系统管理员",LastLoginDate=DateTime.Now },
                                new T_User{ User = "lvfk",Pwd=MD5Utils.encode("ws280-1"),CreateDate=DateTime.Now,Email="FengKai.Lv@availink.com",RoleID=0,Desc="系统管理员",LastLoginDate=DateTime.Now }
                            };
                            DbSet<T_User> UserSet = context.Set<T_User>();
                            UserSet.AddOrUpdate(m => new { m.User }, users.ToArray());

                            context.SaveChanges();
                        }
                    }
            
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("无法初始化 ASP.NET Simple Membership 数据库。有关详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}

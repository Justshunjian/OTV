using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Otv.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("ServerConnection")
        {

        }

        public DbSet<T_User> T_Users { get; set; }
        public DbSet<T_Role> T_Roles { get; set; }
        public DbSet<T_PwdUpdate> T_PwdUpdates { get; set; }
        public DbSet<T_Record> T_Records { get; set; }
        public DbSet<T_HotChn> T_HotChns { get; set; }
        public DbSet<T_Setting> T_Settings { get; set; }
        public DbSet<T_Filter> T_Filters { get; set; }
        public DbSet<T_DanMu> T_DanMus { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new PwdUpdateMap());
            modelBuilder.Configurations.Add(new RecordMap());
            modelBuilder.Configurations.Add(new HotChnMap());
            modelBuilder.Configurations.Add(new SettingMap());
            modelBuilder.Configurations.Add(new FilterMap());
            modelBuilder.Configurations.Add(new DanMuMap());
        }

    }
}
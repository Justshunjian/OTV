using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Otv.Models
{
    public class DanmuBean
    {
        /// <summary>
        /// 节目唯一编号
        /// </summary>
        public string Cuid { get; set; }
        /// <summary>
        /// 弹幕内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 机器唯一标识
        /// </summary>
        public string Uid { get; set; }
        /// <summary>
        /// 服务器端时间
        /// </summary>
        public DateTime DateTime { get; set; }
    }

    public class DmBean
    {
        /// <summary>
        /// 节目唯一编号
        /// </summary>
        public string Cuid { get; set; }
        /// <summary>
        /// 弹幕内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 机器唯一标识
        /// </summary>
        public string Uid { get; set; }
        public string IP { get; set; }
        public string Date { get; set; }
        // <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
    }

    [Table("T_Records")]
    public class T_Record
    {
        /// <summary>
        /// 节目唯一编号
        /// </summary>
        public string Cuid { get; set; }
        /// <summary>
        /// 弹幕内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 机器唯一标识
        /// </summary>
        public string Uid { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
    }

    public class RecordMap : EntityTypeConfiguration<T_Record>
    {
        public RecordMap()
        {
            this.HasKey(t => new { t.Cuid, t.Date });

            this.Property(t => t.Cuid).IsRequired();
            this.Property(t => t.Content).IsRequired();
            this.Property(t => t.Uid).IsRequired();
            this.Property(t => t.Date).IsRequired();
            this.ToTable("T_Records");
        }
    }

    public class DMBean
    {
        public long id { get; set; }
        public long Id { get; set; }
        public string Value { get; set; }
        public bool State { get; set; }
        public string  Date { get; set; }
    }

    [Table("T_DanMus")]
    public class T_DanMu
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public bool State { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
    }

    public class DanMuMap : EntityTypeConfiguration<T_DanMu>
    {
        public DanMuMap()
        {
            this.HasKey(t => t.Id);

            this.Property(t => t.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Value).IsRequired().HasMaxLength(100);
            this.Property(t => t.State).IsRequired();
            this.Property(t => t.Date).IsRequired();
            this.Property(t => t.Area).IsRequired();
            this.ToTable("T_DanMus");
        }
    }
}
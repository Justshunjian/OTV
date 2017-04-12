using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Otv.Models
{
    public class ChnModel
    {
        public string Cuid { get; set; }
        public string Sat { get; set; }
        public string CInfo { get; set; }
        public int TvRadio { get; set; }
        public int Frequency { get; set; }
        public int Polar { get; set; }
        public string Area { get; set; }
    }

    [Table("T_HotChns")]
    public class T_HotChn
    {
        /// <summary>
        /// 节目唯一编号
        /// </summary>
        public string Cuid { get; set; }
        /// <summary>
        /// 节目信息
        /// </summary>
        public string ChnInfo { get; set; }
        /// <summary>
        /// 热度值
        /// </summary>
        public long HeatValue { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
    }

    public class HotChnMap : EntityTypeConfiguration<T_HotChn>
    {
        public HotChnMap()
        {
            this.HasKey(t => t.Cuid);
            this.Property(t => t.Cuid).IsRequired();
            this.Property(t => t.ChnInfo).IsRequired();
            this.Property(t => t.HeatValue).IsRequired();
            this.Property(t => t.Area).IsRequired();
            this.ToTable("T_HotChns");
        }
    }
}
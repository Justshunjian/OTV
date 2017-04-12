using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Otv.Models
{
    [Table("T_Settings")]
    public class T_Setting
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }

    public class SettingMap : EntityTypeConfiguration<T_Setting>
    {
        public SettingMap()
        {
            this.HasKey(t => t.Key);

            this.Property(t => t.Key).IsRequired();
            this.Property(t => t.Value).IsRequired();

            this.ToTable("T_Settings");
        }
    }

}
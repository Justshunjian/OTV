using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;

namespace Otv.Models
{
    public class FilterBean
    {
        public long id { get; set; }
        public long Id { get; set; }
        public string Value { get; set; }
        public bool State { get; set; }
        public string Date { get; set; }
    }

    [Table("T_Filters")]
    public class T_Filter
    {
        public long Id { get; set; }
        public string Value { get; set; }
        public bool State { get; set; }
        public DateTime Date { get; set; }
    }

    public class FilterMap : EntityTypeConfiguration<T_Filter>
    {
        public FilterMap()
        {
            this.HasKey(t => t.Id);

            this.Property(t => t.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Value).IsRequired().HasMaxLength(30);
            this.Property(t => t.State).IsRequired();
            this.Property(t => t.Date).IsRequired();

            this.ToTable("T_Filters");
        }
    }
}
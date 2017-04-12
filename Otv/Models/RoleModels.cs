using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Otv.Models
{
    [Table("T_Roles")]
    public class T_Role
    {
        public string RoleName { get; set; }
    }

    public class RoleMap : EntityTypeConfiguration<T_Role>
    {
        public RoleMap()
        {
            this.HasKey(t => t.RoleName);
            this.Property(t => t.RoleName).IsRequired().HasMaxLength(30);

            this.ToTable("T_Roles");
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Otv.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name="用户名")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="密码")]
        public string Pwd { get; set; }
        [Required]
        [Display(Name="验证码")]
        public string VerificationCode { get; set; }
    }

    public class ResetModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "验证码")]
        public string VerificationCode { get; set; }
    }

    public class ResetPwdModel
    {
        [Required]
        [Display(Name = "用户名")]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("^[a-z0-9A-Z_-]{6,12}$")]
        [Display(Name = "新密码")]
        public string Pwd { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        public string RePwd { get; set; }
        [Required]
        [Display(Name = "日期")]
        public string Date { get; set; }
        [Required]
        [Display(Name = "验证码")]
        public string VerificationCode { get; set; }
    }

    [Table("T_Users")]
    public class T_User
    {
        public string User { get; set; }
        public string Pwd { get; set; }
        public long RoleID { get; set; }
        public string Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Desc { get; set; }
    }

    public class UserMap : EntityTypeConfiguration<T_User>
    {
        public UserMap()
        {
            //Key
            this.HasKey(t => t.User);

            this.Property(t => t.User).HasMaxLength(50);
            this.Property(t => t.Pwd).IsRequired();
            this.Property(t => t.RoleID).IsRequired();
            this.Property(t => t.Email).IsRequired();
            this.Property(t => t.CreateDate).IsRequired();
            this.Property(t => t.LastLoginDate).IsRequired();
            this.Property(t => t.Desc).IsRequired();

            // Table & Column Mappings
            this.ToTable("T_Users");
            //this.Property(t => t.User_Name).HasColumnName("用户名");
        }
    }

    [Table("T_PwdUpdates")]
    public class T_PwdUpdate
    {
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string CreateDate { get; set; }
        public string Desc { get; set; }
    }

    public class PwdUpdateMap : EntityTypeConfiguration<T_PwdUpdate>
    {
        public PwdUpdateMap()
        {
            //Key
            this.HasKey(t => new { t.User,t.CreateDate});

            this.Property(t => t.User).HasMaxLength(50);
            this.Property(t => t.Pwd).IsRequired();
            this.Property(t => t.Email).IsRequired();
            this.Property(t => t.CreateDate).IsRequired();
            this.Property(t => t.Desc).IsRequired();

            // Table & Column Mappings
            this.ToTable("T_PwdUpdates");
        }
    }

}
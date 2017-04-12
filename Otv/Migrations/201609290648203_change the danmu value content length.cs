namespace Otv.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changethedanmuvaluecontentlength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.T_DanMus", "Value", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.T_DanMus", "Value", c => c.String(nullable: false, maxLength: 30));
        }
    }
}

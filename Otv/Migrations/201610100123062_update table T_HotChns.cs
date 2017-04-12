namespace Otv.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetableT_HotChns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.T_HotChns", "ChnInfo", c => c.String(nullable: false));
            DropColumn("dbo.T_HotChns", "ChnId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.T_HotChns", "ChnId", c => c.String(nullable: false));
            DropColumn("dbo.T_HotChns", "ChnInfo");
        }
    }
}

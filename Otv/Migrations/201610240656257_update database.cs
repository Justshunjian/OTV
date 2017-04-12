namespace Otv.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.T_Records", "Uid", c => c.String(nullable: false));
            DropColumn("dbo.T_Records", "Type");
            DropColumn("dbo.T_Records", "Live");
        }
        
        public override void Down()
        {
            AddColumn("dbo.T_Records", "Live", c => c.Int(nullable: false));
            AddColumn("dbo.T_Records", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.T_Records", "Uid");
        }
    }
}

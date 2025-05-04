namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterCategory : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "Position", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Category", "Position", c => c.Int(nullable: false));
        }
    }
}

namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHoaDonNe : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HoaDon", "CCCD", c => c.String(nullable: false, maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HoaDon", "CCCD");
        }
    }
}

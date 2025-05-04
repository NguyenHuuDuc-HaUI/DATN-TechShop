namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHoaDon : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.HoaDon", "TongHoaDon");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HoaDon", "TongHoaDon", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

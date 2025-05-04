namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ALterProductNiemYet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "GiaNiemYet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Product", "GiaBan", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "GiaBan", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Product", "GiaNiemYet");
        }
    }
}

namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDBThongKe : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ThongKe",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ThoiGian = c.DateTime(nullable: false),
                        SoTruyCap = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ThongKe");
        }
    }
}

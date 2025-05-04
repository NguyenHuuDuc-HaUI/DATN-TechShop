namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PMH_CV : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.PhieuMuaHang", "ID_ChucVu");
            AddForeignKey("dbo.PhieuMuaHang", "ID_ChucVu", "dbo.ChucVu", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhieuMuaHang", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.PhieuMuaHang", new[] { "ID_ChucVu" });
        }
    }
}

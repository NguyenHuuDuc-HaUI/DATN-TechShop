namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addKNNV_CV : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.NhanVien", "ID_ChucVu");
            AddForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.NhanVien", new[] { "ID_ChucVu" });
        }
    }
}

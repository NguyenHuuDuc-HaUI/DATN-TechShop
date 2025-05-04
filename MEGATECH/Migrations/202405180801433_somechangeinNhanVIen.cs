namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class somechangeinNhanVIen : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.NhanVien", new[] { "ID_ChucVu" });
            AlterColumn("dbo.NhanVien", "ID_ChucVu", c => c.Int());
            CreateIndex("dbo.NhanVien", "ID_ChucVu");
            AddForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.NhanVien", new[] { "ID_ChucVu" });
            AlterColumn("dbo.NhanVien", "ID_ChucVu", c => c.Int(nullable: false));
            CreateIndex("dbo.NhanVien", "ID_ChucVu");
            AddForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu", "ID", cascadeDelete: true);
        }
    }
}

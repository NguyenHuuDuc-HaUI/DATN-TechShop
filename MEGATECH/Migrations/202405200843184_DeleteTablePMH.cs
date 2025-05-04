namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteTablePMH : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChiTietPhieuMuaHang", "ID_Product", "dbo.Product");
            DropForeignKey("dbo.ChiTietPhieuMuaHang", "ID_PhieuMuaHang", "dbo.PhieuMuaHang");
            DropForeignKey("dbo.PhieuMuaHang", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.ChiTietPhieuMuaHang", new[] { "ID_PhieuMuaHang" });
            DropIndex("dbo.ChiTietPhieuMuaHang", new[] { "ID_Product" });
            DropIndex("dbo.PhieuMuaHang", new[] { "ID_ChucVu" });
            DropTable("dbo.ChiTietPhieuMuaHang");
            DropTable("dbo.PhieuMuaHang");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PhieuMuaHang",
                c => new
                    {
                        MaPhieu = c.String(nullable: false, maxLength: 50),
                        ID_ChucVu = c.Int(nullable: false),
                        TongHoaDon = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PhuongThucThanhToan = c.Int(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.MaPhieu);
            
            CreateTable(
                "dbo.ChiTietPhieuMuaHang",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ID_PhieuMuaHang = c.String(maxLength: 50),
                        ID_Product = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.PhieuMuaHang", "ID_ChucVu");
            CreateIndex("dbo.ChiTietPhieuMuaHang", "ID_Product");
            CreateIndex("dbo.ChiTietPhieuMuaHang", "ID_PhieuMuaHang");
            AddForeignKey("dbo.PhieuMuaHang", "ID_ChucVu", "dbo.ChucVu", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ChiTietPhieuMuaHang", "ID_PhieuMuaHang", "dbo.PhieuMuaHang", "MaPhieu");
            AddForeignKey("dbo.ChiTietPhieuMuaHang", "ID_Product", "dbo.Product", "MaSanPham");
        }
    }
}

namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table0339 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChiTietPhieuMuaHang",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ID_PhieuMuaHang = c.String(maxLength: 50),
                        ID_Product = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Product", t => t.ID_Product)
                .ForeignKey("dbo.PhieuMuaHang", t => t.ID_PhieuMuaHang)
                .Index(t => t.ID_PhieuMuaHang)
                .Index(t => t.ID_Product);
            
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
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                    })
                .PrimaryKey(t => t.MaPhieu);
            
            AddColumn("dbo.HoaDon", "ID_KhachHang", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.ChucVu", "TenChucVu", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.ChucVu", "MoTa", c => c.String(maxLength: 500));
            DropColumn("dbo.ChiTietHoaDon", "DVT");
            DropColumn("dbo.HoaDon", "IDKhachHang");
            DropColumn("dbo.NhanVien", "ID_Username");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NhanVien", "ID_Username", c => c.String(maxLength: 50));
            AddColumn("dbo.HoaDon", "IDKhachHang", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.ChiTietHoaDon", "DVT", c => c.String());
            DropForeignKey("dbo.ChiTietPhieuMuaHang", "ID_PhieuMuaHang", "dbo.PhieuMuaHang");
            DropForeignKey("dbo.ChiTietPhieuMuaHang", "ID_Product", "dbo.Product");
            DropIndex("dbo.ChiTietPhieuMuaHang", new[] { "ID_Product" });
            DropIndex("dbo.ChiTietPhieuMuaHang", new[] { "ID_PhieuMuaHang" });
            AlterColumn("dbo.ChucVu", "MoTa", c => c.String());
            AlterColumn("dbo.ChucVu", "TenChucVu", c => c.String(nullable: false));
            DropColumn("dbo.HoaDon", "ID_KhachHang");
            DropTable("dbo.PhieuMuaHang");
            DropTable("dbo.ChiTietPhieuMuaHang");
        }
    }
}

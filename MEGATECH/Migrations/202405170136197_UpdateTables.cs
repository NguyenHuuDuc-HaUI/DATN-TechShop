namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NhanVien", "ID_Username", "dbo.TaiKhoan");
            DropForeignKey("dbo.TaiKhoan", "NhanVien_ID", "dbo.NhanVien");
            DropForeignKey("dbo.NhanVien", "TaiKhoan_TenDangNhap", "dbo.TaiKhoan");
            DropForeignKey("dbo.TaiKhoan", "ID_NhanVien", "dbo.NhanVien");
            DropForeignKey("dbo.ProductImage", "ProductID", "dbo.Product");
            DropForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu");
            DropIndex("dbo.TaiKhoan", new[] { "ID_NhanVien" });
            DropIndex("dbo.TaiKhoan", new[] { "NhanVien_ID" });
            DropIndex("dbo.NhanVien", new[] { "ID_Username" });
            DropIndex("dbo.NhanVien", new[] { "ID_ChucVu" });
            DropIndex("dbo.NhanVien", new[] { "TaiKhoan_TenDangNhap" });
            DropIndex("dbo.ProductImage", new[] { "ProductID" });
            DropPrimaryKey("dbo.PhanQuyen");
            AddColumn("dbo.NhanVien", "TenDangNhap", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.NhanVien", "MatKhau", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.NhanVien", "TenHienThi", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.NhanVien", "IsActiveAccount", c => c.Boolean(nullable: false));
            AddColumn("dbo.PhanQuyen", "IDChucVu", c => c.Int(nullable: false));
            AlterColumn("dbo.NhanVien", "ID_ChucVu", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.PhanQuyen", new[] { "IDChucVu", "MaChucNang" });
            CreateIndex("dbo.PhanQuyen", "IDChucVu");
            CreateIndex("dbo.PhanQuyen", "MaChucNang");
            CreateIndex("dbo.NhanVien", "ID_ChucVu");
            AddForeignKey("dbo.PhanQuyen", "MaChucNang", "dbo.ChucNangQuyen", "MaChucNang", cascadeDelete: true);
            AddForeignKey("dbo.PhanQuyen", "IDChucVu", "dbo.ChucVu", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu", "ID", cascadeDelete: true);
            DropColumn("dbo.PhanQuyen", "TenDangNhap");
            DropTable("dbo.TaiKhoan");
            DropTable("dbo.ProductImage");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductImage",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProductID = c.String(maxLength: 50),
                        Image = c.String(),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TaiKhoan",
                c => new
                    {
                        TenDangNhap = c.String(nullable: false, maxLength: 50),
                        MatKhau = c.String(nullable: false, maxLength: 50),
                        ID_NhanVien = c.String(maxLength: 50),
                        TenHienThi = c.String(nullable: false, maxLength: 150),
                        IsActive = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(nullable: false),
                        Modifiedby = c.String(),
                        NhanVien_ID = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.TenDangNhap);
            
            AddColumn("dbo.PhanQuyen", "TenDangNhap", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.NhanVien", "TaiKhoan_TenDangNhap", c => c.String(maxLength: 50));
            DropForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu");
            DropForeignKey("dbo.PhanQuyen", "IDChucVu", "dbo.ChucVu");
            DropForeignKey("dbo.PhanQuyen", "MaChucNang", "dbo.ChucNangQuyen");
            DropIndex("dbo.NhanVien", new[] { "ID_ChucVu" });
            DropIndex("dbo.PhanQuyen", new[] { "MaChucNang" });
            DropIndex("dbo.PhanQuyen", new[] { "IDChucVu" });
            DropPrimaryKey("dbo.PhanQuyen");
            AlterColumn("dbo.NhanVien", "ID_ChucVu", c => c.Int());
            DropColumn("dbo.PhanQuyen", "IDChucVu");
            DropColumn("dbo.NhanVien", "IsActiveAccount");
            DropColumn("dbo.NhanVien", "TenHienThi");
            DropColumn("dbo.NhanVien", "MatKhau");
            DropColumn("dbo.NhanVien", "TenDangNhap");
            AddPrimaryKey("dbo.PhanQuyen", new[] { "TenDangNhap", "MaChucNang" });
            CreateIndex("dbo.ProductImage", "ProductID");
            CreateIndex("dbo.NhanVien", "TaiKhoan_TenDangNhap");
            CreateIndex("dbo.NhanVien", "ID_ChucVu");
            CreateIndex("dbo.NhanVien", "ID_Username");
            CreateIndex("dbo.TaiKhoan", "NhanVien_ID");
            CreateIndex("dbo.TaiKhoan", "ID_NhanVien");
            AddForeignKey("dbo.NhanVien", "ID_ChucVu", "dbo.ChucVu", "ID");
            AddForeignKey("dbo.ProductImage", "ProductID", "dbo.Product", "MaSanPham");
            AddForeignKey("dbo.TaiKhoan", "ID_NhanVien", "dbo.NhanVien", "ID");
            AddForeignKey("dbo.NhanVien", "TaiKhoan_TenDangNhap", "dbo.TaiKhoan", "TenDangNhap");
            AddForeignKey("dbo.TaiKhoan", "NhanVien_ID", "dbo.NhanVien", "ID");
            AddForeignKey("dbo.NhanVien", "ID_Username", "dbo.TaiKhoan", "TenDangNhap");
        }
    }
}

namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTitleAllTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Category", "Title", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Product", "Title", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.NhaCungCap", "Title", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.ProductCategory", "Title", c => c.String(nullable: false, maxLength: 500));
            DropColumn("dbo.Category", "TenDanhMuc");
            DropColumn("dbo.Product", "TenSanPham");
            DropColumn("dbo.NhaCungCap", "TenNhaCungCap");
            DropColumn("dbo.ProductCategory", "TenLoaiSanPham");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductCategory", "TenLoaiSanPham", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.NhaCungCap", "TenNhaCungCap", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.Product", "TenSanPham", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.Category", "TenDanhMuc", c => c.String(nullable: false, maxLength: 150));
            DropColumn("dbo.ProductCategory", "Title");
            DropColumn("dbo.NhaCungCap", "Title");
            DropColumn("dbo.Product", "Title");
            DropColumn("dbo.Category", "Title");
        }
    }
}

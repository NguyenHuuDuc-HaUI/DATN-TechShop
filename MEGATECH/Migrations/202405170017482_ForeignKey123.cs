namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKey123 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChiTietHoaDon", "OrderID", c => c.String(maxLength: 50));
            AlterColumn("dbo.ChiTietHoaDon", "ProductID", c => c.String(maxLength: 50));
            AlterColumn("dbo.ProductImage", "ProductID", c => c.String(maxLength: 50));
            AlterColumn("dbo.Product", "ProductCategoryID", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Product", "SupplierID", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.ChiTietHoaDon", "OrderID");
            CreateIndex("dbo.ChiTietHoaDon", "ProductID");
            CreateIndex("dbo.Product", "ProductCategoryID");
            CreateIndex("dbo.Product", "SupplierID");
            CreateIndex("dbo.ProductImage", "ProductID");
            AddForeignKey("dbo.ChiTietHoaDon", "OrderID", "dbo.HoaDon", "MaHoaDon", cascadeDelete: true);
            AddForeignKey("dbo.ChiTietHoaDon", "ProductID", "dbo.Product", "MaSanPham", cascadeDelete: true);
            AddForeignKey("dbo.Product", "SupplierID", "dbo.NhaCungCap", "MaNhaCungCap", cascadeDelete: true);
            AddForeignKey("dbo.Product", "ProductCategoryID", "dbo.ProductCategory", "MaLoaiSanPham", cascadeDelete: true);
            AddForeignKey("dbo.ProductImage", "ProductID", "dbo.Product", "MaSanPham", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImage", "ProductID", "dbo.Product");
            DropForeignKey("dbo.Product", "ProductCategoryID", "dbo.ProductCategory");
            DropForeignKey("dbo.Product", "SupplierID", "dbo.NhaCungCap");
            DropForeignKey("dbo.ChiTietHoaDon", "ProductID", "dbo.Product");
            DropForeignKey("dbo.ChiTietHoaDon", "OrderID", "dbo.HoaDon");
            DropIndex("dbo.ProductImage", new[] { "ProductID" });
            DropIndex("dbo.Product", new[] { "SupplierID" });
            DropIndex("dbo.Product", new[] { "ProductCategoryID" });
            DropIndex("dbo.ChiTietHoaDon", new[] { "ProductID" });
            DropIndex("dbo.ChiTietHoaDon", new[] { "OrderID" });
            AlterColumn("dbo.Product", "SupplierID", c => c.String(nullable: false));
            AlterColumn("dbo.Product", "ProductCategoryID", c => c.String(nullable: false));
            AlterColumn("dbo.ProductImage", "ProductID", c => c.String());
            AlterColumn("dbo.ChiTietHoaDon", "ProductID", c => c.String());
            AlterColumn("dbo.ChiTietHoaDon", "OrderID", c => c.String());
        }
    }
}

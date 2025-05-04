namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterTableCommonAbstract : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.HoaDon", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.Product", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.PhieuMuaHang", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.ChucVu", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.NhanVien", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.NhaCungCap", "ModifiedDate", c => c.DateTime());
            AlterColumn("dbo.ProductCategory", "ModifiedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductCategory", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NhaCungCap", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.NhanVien", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ChucVu", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PhieuMuaHang", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Product", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.HoaDon", "ModifiedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Category", "ModifiedDate", c => c.DateTime(nullable: false));
        }
    }
}

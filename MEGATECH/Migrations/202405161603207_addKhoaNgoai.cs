namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addKhoaNgoai : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TaiKhoan", "ID_NhanVien");
            CreateIndex("dbo.NhanVien", "ID_Username");
            AddForeignKey("dbo.NhanVien", "ID_Username", "dbo.TaiKhoan", "TenDangNhap");
            AddForeignKey("dbo.TaiKhoan", "ID_NhanVien", "dbo.NhanVien", "ID");
        }
        
        public override void Down()
        {

        }
    }
}

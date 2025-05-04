namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropKN_NV_CV : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NhanVien", "ChucVu_ID", "dbo.ChucVu");
            DropIndex("dbo.NhanVien", new[] { "ChucVu_ID" });
            DropColumn("dbo.NhanVien", "ChucVu_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NhanVien", "ChucVu_ID", c => c.Int());
            CreateIndex("dbo.NhanVien", "ChucVu_ID");
            AddForeignKey("dbo.NhanVien", "ChucVu_ID", "dbo.ChucVu", "ID");
        }
    }
}

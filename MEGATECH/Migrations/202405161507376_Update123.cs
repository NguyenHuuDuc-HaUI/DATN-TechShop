namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update123 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.NhanVien", "ID_Username", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.NhanVien", "ID_Username", c => c.String());
        }
    }
}

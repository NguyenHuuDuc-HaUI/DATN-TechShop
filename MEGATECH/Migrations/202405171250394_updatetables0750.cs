namespace MEGATECH.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetables0750 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NhaCungCap", "SeoTitle", c => c.String(maxLength: 150));
            AddColumn("dbo.NhaCungCap", "SeoDescription", c => c.String(maxLength: 250));
            AddColumn("dbo.NhaCungCap", "SeoKeywords", c => c.String(maxLength: 150));
        }
        
        public override void Down()
        {
            DropColumn("dbo.NhaCungCap", "SeoKeywords");
            DropColumn("dbo.NhaCungCap", "SeoDescription");
            DropColumn("dbo.NhaCungCap", "SeoTitle");
        }
    }
}

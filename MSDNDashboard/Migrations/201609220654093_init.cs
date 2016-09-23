namespace MSDNDashboard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        BlogID = c.Int(nullable: false),
                        JobID = c.Int(nullable: false),
                        Url = c.String(),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BlogID)
                .ForeignKey("dbo.Jobs", t => t.JobID, cascadeDelete: true)
                .Index(t => t.JobID);
            
            CreateTable(
                "dbo.Configs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartTimestamp = c.DateTime(nullable: false),
                        FinishTimestamp = c.DateTime(nullable: false),
                        IsManual = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        TotalNumber = c.Int(nullable: false),
                        CurrentNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Blogs", "JobID", "dbo.Jobs");
            DropIndex("dbo.Blogs", new[] { "JobID" });
            DropTable("dbo.Jobs");
            DropTable("dbo.Configs");
            DropTable("dbo.Blogs");
        }
    }
}

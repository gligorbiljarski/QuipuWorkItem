namespace QuipuWorkItem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateClientSchema : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Addresses", "Client_ID", "dbo.Clients");
            DropIndex("dbo.Addresses", new[] { "Client_ID" });
            AddColumn("dbo.Clients", "FirstName", c => c.String(nullable: false));
            AddColumn("dbo.Clients", "LastName", c => c.String(nullable: false));
            AddColumn("dbo.Clients", "Email", c => c.String(nullable: false));
            DropColumn("dbo.Addresses", "Client_ID");
            DropColumn("dbo.Clients", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "Name", c => c.String(nullable: false));
            AddColumn("dbo.Addresses", "Client_ID", c => c.Int());
            DropColumn("dbo.Clients", "Email");
            DropColumn("dbo.Clients", "LastName");
            DropColumn("dbo.Clients", "FirstName");
            CreateIndex("dbo.Addresses", "Client_ID");
            AddForeignKey("dbo.Addresses", "Client_ID", "dbo.Clients", "ID");
        }
    }
}

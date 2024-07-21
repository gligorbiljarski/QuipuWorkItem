namespace QuipuWorkItem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAddressModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        AddressText = c.String(),
                        Client_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.Client_ID)
                .Index(t => t.Client_ID);
            
            AddColumn("dbo.Clients", "Name", c => c.String(nullable: false));
            AddColumn("dbo.Clients", "BirthDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Clients", "FirstName");
            DropColumn("dbo.Clients", "LastName");
            DropColumn("dbo.Clients", "Email");
            DropColumn("dbo.Clients", "DateOfBirth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Clients", "DateOfBirth", c => c.DateTime(nullable: false));
            AddColumn("dbo.Clients", "Email", c => c.String(nullable: false));
            AddColumn("dbo.Clients", "LastName", c => c.String(nullable: false));
            AddColumn("dbo.Clients", "FirstName", c => c.String(nullable: false));
            DropForeignKey("dbo.Addresses", "Client_ID", "dbo.Clients");
            DropIndex("dbo.Addresses", new[] { "Client_ID" });
            DropColumn("dbo.Clients", "BirthDate");
            DropColumn("dbo.Clients", "Name");
            DropTable("dbo.Addresses");
        }
    }
}

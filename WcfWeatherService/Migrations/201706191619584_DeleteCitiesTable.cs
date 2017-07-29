namespace WcfWeatherService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteCitiesTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Cities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        CountryCode = c.String(maxLength: 2),
                    })
                .PrimaryKey(t => t.CityId);
            
        }
    }
}

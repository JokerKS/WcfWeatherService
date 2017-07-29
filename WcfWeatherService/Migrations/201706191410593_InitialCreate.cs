namespace WcfWeatherService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
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
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientId = c.String(nullable: false, maxLength: 8),
                        Login = c.String(maxLength: 25),
                        Password = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Code = c.String(nullable: false, maxLength: 2),
                        Name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Code);
            
            CreateTable(
                "dbo.OpenWeather",
                c => new
                    {
                        CityId = c.Int(nullable: false),
                        City = c.String(nullable: false, maxLength: 128),
                        Cloudiness = c.Int(nullable: false),
                        Temp = c.Double(nullable: false),
                        Pressure = c.Int(nullable: false),
                        Humidity = c.Int(nullable: false),
                        TempMin = c.Double(nullable: false),
                        TempMax = c.Double(nullable: false),
                        Speed = c.Double(nullable: false),
                        Direction = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CityId, t.City });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OpenWeather");
            DropTable("dbo.Countries");
            DropTable("dbo.Clients");
            DropTable("dbo.Cities");
        }
    }
}

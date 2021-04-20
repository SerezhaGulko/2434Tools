using Microsoft.EntityFrameworkCore.Migrations;

namespace _2434Tools.Migrations
{
    public partial class AddBannerURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerURL",
                table: "Livers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerURL",
                table: "Livers");
        }
    }
}

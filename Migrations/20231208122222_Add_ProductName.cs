using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KizspyWebApp.Migrations
{
    public partial class Add_ProductName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CartItems");
        }
    }
}

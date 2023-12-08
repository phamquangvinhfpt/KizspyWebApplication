using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KizspyWebApp.Migrations
{
    public partial class CartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "CartItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartItems");
        }
    }
}

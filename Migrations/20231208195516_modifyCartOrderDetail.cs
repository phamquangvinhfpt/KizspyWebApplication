using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KizspyWebApp.Migrations
{
    public partial class modifyCartOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderDetails");
        }
    }
}

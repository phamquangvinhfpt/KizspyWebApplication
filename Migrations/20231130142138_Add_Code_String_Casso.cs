using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KizspyWebApp.Migrations
{
    public partial class Add_Code_String_Casso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Casso_Code",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Casso_Code",
                table: "Users");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KizspyWebApp.Migrations
{
    public partial class Add_Casso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CassoTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CusumBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    When = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankSubAccId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAccId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VirtualAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VirtualAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorresponsiveName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorresponsiveAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorresponsiveBankId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorresponsiveBankName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CassoTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CassoTranId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CassoTransactions");

            migrationBuilder.DropTable(
                name: "SystemTransactions");
        }
    }
}

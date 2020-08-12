using Microsoft.EntityFrameworkCore.Migrations;
using RCB.JavaScript.Models;

namespace RCB.JavaScript.Migrations
{
    public partial class CharacterCountAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<CountAnalysis>(
                name: "CountAnalysis",
                table: "Characters",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CountAnalysis",
                table: "Characters",
                column: "CountAnalysis")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Characters_CountAnalysis",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CountAnalysis",
                table: "Characters");
        }
    }
}

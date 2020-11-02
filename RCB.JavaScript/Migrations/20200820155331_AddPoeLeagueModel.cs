using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using RCB.JavaScript.Models;

namespace RCB.JavaScript.Migrations
{
    public partial class AddPoeLeagueModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    LeagueId = table.Column<string>(type: "text", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RegisterAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DelveEvent = table.Column<bool>(type: "boolean", nullable: false),
                    Rules = table.Column<List<PoeLeagueRules>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.LeagueId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_LeagueId",
                table: "Leagues",
                column: "LeagueId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}

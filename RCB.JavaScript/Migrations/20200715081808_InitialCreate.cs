using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using RCB.JavaScript.Models;
using RCB.JavaScript.Models.Utils;

namespace RCB.JavaScript.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<string>(nullable: false),
                    LeagueName = table.Column<string>(nullable: false),
                    CharacterName = table.Column<string>(nullable: false),
                    AccountName = table.Column<string>(nullable: false),
                    Account = table.Column<PoeAccount>(type: "jsonb", nullable: true),
                    Level = table.Column<int>(nullable: false),
                    LifeUnreserved = table.Column<long>(nullable: false),
                    EnergyShield = table.Column<long>(nullable: false),
                    Class = table.Column<string>(nullable: false),
                    Dead = table.Column<bool>(nullable: false),
                    Online = table.Column<bool>(nullable: false),
                    Rank = table.Column<int>(nullable: false, defaultValue: 15001),
                    Depth = table.Column<PoeDepth>(type: "jsonb", nullable: true),
                    Experience = table.Column<long>(nullable: false),
                    PobCode = table.Column<string>(nullable: false),
                    Pob = table.Column<PathOfBuilding>(type: "jsonb", nullable: false),
                    Items = table.Column<List<PoeItem>>(type: "jsonb", nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterId",
                table: "Characters",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Items",
                table: "Characters",
                column: "Items")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Pob",
                table: "Characters",
                column: "Pob")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterName_AccountName",
                table: "Characters",
                columns: new[] { "CharacterName", "AccountName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ZdrojId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Data");

            migrationBuilder.EnsureSchema(
                name: "Identity");

           
         

          
         

            
            migrationBuilder.CreateTable(
                name: "TableLokalita",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Klasifikace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Baterie = table.Column<int>(type: "int", nullable: false),
                    DA = table.Column<bool>(type: "bit", nullable: false),
                    Zasuvka = table.Column<bool>(type: "bit", nullable: false),
                    RegionID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableLokalita", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableLokalita_TableRegion_RegionID",
                        column: x => x.RegionID,
                        principalSchema: "Data",
                        principalTable: "TableRegion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

          
            migrationBuilder.CreateTable(
                name: "TableOdstavka",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Distributor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Od = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Do = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Popis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LokalitaID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableOdstavka", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableOdstavka_TableLokalita_LokalitaID",
                        column: x => x.LokalitaID,
                        principalSchema: "Data",
                        principalTable: "TableLokalita",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableDieslovani",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Vstup = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Odchod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdOdstavky = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdTechnik = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDieslovani", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_TableOdstavka_IdOdstavky",
                        column: x => x.IdOdstavky,
                        principalSchema: "Data",
                        principalTable: "TableOdstavka",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_TableTechnik_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DebugModel",
                schema: "Data",
                columns: table => new
                {
                    IdLog = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdOdstavky = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OdstavkyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdDieslovani = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebugModel", x => x.IdLog);
                    table.ForeignKey(
                        name: "FK_DebugModel_TableDieslovani_IdDieslovani",
                        column: x => x.IdDieslovani,
                        principalSchema: "Data",
                        principalTable: "TableDieslovani",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DebugModel_TableOdstavka_OdstavkyID",
                        column: x => x.OdstavkyID,
                        principalSchema: "Data",
                        principalTable: "TableOdstavka",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebugModel_IdDieslovani",
                schema: "Data",
                table: "DebugModel",
                column: "IdDieslovani");

            migrationBuilder.CreateIndex(
                name: "IX_DebugModel_OdstavkyID",
                schema: "Data",
                table: "DebugModel",
                column: "OdstavkyID");

        


            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IdOdstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdOdstavky");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TableLokalita_RegionID",
                schema: "Data",
                table: "TableLokalita",
                column: "RegionID");


            migrationBuilder.CreateIndex(
                name: "IX_TableOdstavka_LokalitaID",
                schema: "Data",
                table: "TableOdstavka",
                column: "LokalitaID");

         




            

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebugModel",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TablePohotovost",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TableDieslovani",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TableOdstavka",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableTechnik",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableLokalita",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TableRegion",
                schema: "Data");


            migrationBuilder.DropTable(
                name: "TableFirma",
                schema: "Data");
        }
    }
}

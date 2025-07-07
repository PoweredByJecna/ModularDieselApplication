using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Data");

            migrationBuilder.EnsureSchema(
                name: "Identity");

          

            migrationBuilder.CreateTable(
                name: "TableFirma",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableFirma", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TableZdroj",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Odber = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableZdroj", x => x.ID);
                });

        

            migrationBuilder.CreateTable(
                name: "TableRegion",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmaID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRegion", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableRegion_TableFirma_FirmaID",
                        column: x => x.FirmaID,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableTechnik",
                schema: "Data",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Taken = table.Column<bool>(type: "bit", nullable: false),
                    FirmaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTechnik", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableTechnik_TableFirma_FirmaId",
                        column: x => x.FirmaId,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableTechnik_User_IdUser",
                        column: x => x.IdUser,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          
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
                    ZdrojId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_TableLokalita_TableZdroj_ZdrojId",
                        column: x => x.ZdrojId,
                        principalSchema: "Data",
                        principalTable: "TableZdroj",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TablePohotovost",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Zacatek = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Konec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdTechnik = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablePohotovost", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TablePohotovost_TableTechnik_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnik",
                        principalColumn: "Id",
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
                    IdOdstavky = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    odstavkyID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IdDieslovani = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebugModel_TableOdstavka_odstavkyID",
                        column: x => x.odstavkyID,
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
                name: "IX_DebugModel_odstavkyID",
                schema: "Data",
                table: "DebugModel",
                column: "odstavkyID");

         

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
                name: "IX_TableLokalita_ZdrojId",
                schema: "Data",
                table: "TableLokalita",
                column: "ZdrojId");

            migrationBuilder.CreateIndex(
                name: "IX_TableOdstavka_LokalitaID",
                schema: "Data",
                table: "TableOdstavka",
                column: "LokalitaID");

            migrationBuilder.CreateIndex(
                name: "IX_TablePohotovost_IdTechnik",
                schema: "Data",
                table: "TablePohotovost",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TableRegion_FirmaID",
                schema: "Data",
                table: "TableRegion",
                column: "FirmaID");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_FirmaId",
                schema: "Data",
                table: "TableTechnik",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnik_IdUser",
                schema: "Data",
                table: "TableTechnik",
                column: "IdUser");

      
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
                name: "TableZdroj",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableFirma",
                schema: "Data");
        }
    }
}

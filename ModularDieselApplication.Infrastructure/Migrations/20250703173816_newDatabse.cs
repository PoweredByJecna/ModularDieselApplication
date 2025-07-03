using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newDatabse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Data");


            migrationBuilder.CreateTable(
                name: "DebugModel",
                schema: "Data",
                columns: table => new
                {
                    IdLog = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebugModel", x => x.IdLog);
                });

           
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
                name: "TableRegiony",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nazev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmaID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRegiony", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableRegiony_TableFirma_FirmaID",
                        column: x => x.FirmaID,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableTechnici",
                schema: "Data",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Taken = table.Column<bool>(type: "bit", nullable: false),
                    FirmaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTechnici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableTechnici_TableFirma_FirmaId",
                        column: x => x.FirmaId,
                        principalSchema: "Data",
                        principalTable: "TableFirma",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableTechnici_User_IdUser",
                        column: x => x.IdUser,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

           
            migrationBuilder.CreateTable(
                name: "LokalityTable",
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
                    table.PrimaryKey("PK_LokalityTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LokalityTable_TableRegiony_RegionID",
                        column: x => x.RegionID,
                        principalSchema: "Data",
                        principalTable: "TableRegiony",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LokalityTable_TableZdroj_ZdrojId",
                        column: x => x.ZdrojId,
                        principalSchema: "Data",
                        principalTable: "TableZdroj",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TablePohotovosti",
                schema: "Data",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Zacatek = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Konec = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdTechnik = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TablePohotovosti", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TablePohotovosti_TableTechnici_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TablePohotovosti_User_IdUser",
                        column: x => x.IdUser,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "OdstavkyTable",
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
                    table.PrimaryKey("PK_OdstavkyTable", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OdstavkyTable_LokalityTable_LokalitaID",
                        column: x => x.LokalitaID,
                        principalSchema: "Data",
                        principalTable: "LokalityTable",
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
                    IDodstavky = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdTechnik = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDieslovani", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_OdstavkyTable_IDodstavky",
                        column: x => x.IDodstavky,
                        principalSchema: "Data",
                        principalTable: "OdstavkyTable",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableDieslovani_TableTechnici_IdTechnik",
                        column: x => x.IdTechnik,
                        principalSchema: "Data",
                        principalTable: "TableTechnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LokalityTable_RegionID",
                schema: "Data",
                table: "LokalityTable",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_LokalityTable_ZdrojId",
                schema: "Data",
                table: "LokalityTable",
                column: "ZdrojId");

            migrationBuilder.CreateIndex(
                name: "IX_OdstavkyTable_LokalitaID",
                schema: "Data",
                table: "OdstavkyTable",
                column: "LokalitaID");

          

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IDodstavky",
                schema: "Data",
                table: "TableDieslovani",
                column: "IDodstavky");

            migrationBuilder.CreateIndex(
                name: "IX_TableDieslovani_IdTechnik",
                schema: "Data",
                table: "TableDieslovani",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TablePohotovosti_IdTechnik",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdTechnik");

            migrationBuilder.CreateIndex(
                name: "IX_TablePohotovosti_IdUser",
                schema: "Data",
                table: "TablePohotovosti",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_TableRegiony_FirmaID",
                schema: "Data",
                table: "TableRegiony",
                column: "FirmaID");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnici_FirmaId",
                schema: "Data",
                table: "TableTechnici",
                column: "FirmaId");

            migrationBuilder.CreateIndex(
                name: "IX_TableTechnici_IdUser",
                schema: "Data",
                table: "TableTechnici",
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
                name: "TableDieslovani",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TablePohotovosti",
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
                name: "OdstavkyTable",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "TableTechnici",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "LokalityTable",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TableRegiony",
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

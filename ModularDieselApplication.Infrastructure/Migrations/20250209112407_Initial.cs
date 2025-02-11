using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dieslovani",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Log",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Pohotovosti",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

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
                name: "Odstavky",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Technici",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Lokality",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Regiony",
                schema: "Data");

            migrationBuilder.DropTable(
                name: "Firma",
                schema: "Data");
        }
    }
}

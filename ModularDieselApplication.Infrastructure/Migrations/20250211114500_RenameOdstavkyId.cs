using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOdstavkyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.RenameColumn(
                name: "IdRegion",
                schema: "Data",
                table: "TableRegiony",
                newName: "ID");

                migrationBuilder.RenameColumn(
                name: "IdOdstavky",
                schema: "Data",
                table: "OdstavkyTable",
                newName: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

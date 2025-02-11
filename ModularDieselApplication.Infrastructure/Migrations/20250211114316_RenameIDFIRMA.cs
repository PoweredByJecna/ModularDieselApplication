using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameIDFIRMA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
               migrationBuilder.RenameColumn(
                name: "IDFirmy",
                schema: "Data",
                table: "TableFirma",
                newName: "ID");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

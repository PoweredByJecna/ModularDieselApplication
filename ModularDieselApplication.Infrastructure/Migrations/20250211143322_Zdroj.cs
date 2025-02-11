using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Zdroj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "TableZdroj",
                schema: "Identity",
                newName: "TableZdroj",
                newSchema: "Data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}

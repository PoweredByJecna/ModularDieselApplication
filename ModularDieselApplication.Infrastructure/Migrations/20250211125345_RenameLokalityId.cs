using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameLokalityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "LokalitaId",
                schema: "Data",
                table: "OdstavkyTable",
                newName: "LokalitaID");


           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}

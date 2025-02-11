using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TableUserFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdPohotovst",
                schema: "Data",
                table: "TablePohotovosti",
                newName: "ID");


            migrationBuilder.AddColumn<string>(
                name: "Jmeno",
                schema: "Identity",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prijmeni",
                schema: "Identity",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");


        


  
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}

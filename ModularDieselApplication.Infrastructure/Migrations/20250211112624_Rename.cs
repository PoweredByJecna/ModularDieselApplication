using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularDieselApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "Jmeno",
                schema: "Data",
                table: "TableTechnici");

            migrationBuilder.DropColumn(
                name: "Prijmeni",
                schema: "Data",
                table: "TableTechnici");

         

            migrationBuilder.RenameColumn(
                name: "IdTechnika",
                schema: "Data",
                table: "TableTechnici",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "NazevRegionu",
                schema: "Data",
                table: "TableRegiony",
                newName: "Nazev");



            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "Data",
                table: "LokalityTable",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "NazevFirmy",
                schema: "Data",
                table: "TableFirma",
                newName: "Nazev");

            migrationBuilder.RenameColumn(
                name: "IdDieslovani",
                schema: "Data",
                table: "TableDieslovani",
                newName: "ID");


            migrationBuilder.AlterColumn<DateTime>(
                name: "Vstup",
                schema: "Data",
                table: "TableDieslovani",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Odchod",
                schema: "Data",
                table: "TableDieslovani",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

   

       

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        
        }
    }
}

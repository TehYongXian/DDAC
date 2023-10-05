using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcflowershoplab1.Migrations
{
    /// <inheritdoc />
    public partial class biketableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowerTable",
                table: "FlowerTable");

            migrationBuilder.RenameTable(
                name: "FlowerTable",
                newName: "BikeTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BikeTable",
                table: "BikeTable",
                column: "BikeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BikeTable",
                table: "BikeTable");

            migrationBuilder.RenameTable(
                name: "BikeTable",
                newName: "FlowerTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowerTable",
                table: "FlowerTable",
                column: "BikeID");
        }
    }
}

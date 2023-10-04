using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcflowershoplab1.Migrations
{
    /// <inheritdoc />
    public partial class BikeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FlowerType",
                table: "FlowerTable",
                newName: "BikeType");

            migrationBuilder.RenameColumn(
                name: "FlowerProducedDate",
                table: "FlowerTable",
                newName: "BikeProducedDate");

            migrationBuilder.RenameColumn(
                name: "FlowerPrice",
                table: "FlowerTable",
                newName: "BikePrice");

            migrationBuilder.RenameColumn(
                name: "FlowerName",
                table: "FlowerTable",
                newName: "BikeName");

            migrationBuilder.RenameColumn(
                name: "FlowerID",
                table: "FlowerTable",
                newName: "BikeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BikeType",
                table: "FlowerTable",
                newName: "FlowerType");

            migrationBuilder.RenameColumn(
                name: "BikeProducedDate",
                table: "FlowerTable",
                newName: "FlowerProducedDate");

            migrationBuilder.RenameColumn(
                name: "BikePrice",
                table: "FlowerTable",
                newName: "FlowerPrice");

            migrationBuilder.RenameColumn(
                name: "BikeName",
                table: "FlowerTable",
                newName: "FlowerName");

            migrationBuilder.RenameColumn(
                name: "BikeID",
                table: "FlowerTable",
                newName: "FlowerID");
        }
    }
}

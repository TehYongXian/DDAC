using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcflowershoplab1.Migrations
{
    /// <inheritdoc />
    public partial class AddImageKeyToFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FLowerTable",
                table: "FLowerTable");

            migrationBuilder.RenameTable(
                name: "FLowerTable",
                newName: "FlowerTable");

            migrationBuilder.AlterColumn<string>(
                name: "FlowerType",
                table: "FlowerTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FlowerName",
                table: "FlowerTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageKey",
                table: "FlowerTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowerTable",
                table: "FlowerTable",
                column: "FlowerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowerTable",
                table: "FlowerTable");

            migrationBuilder.DropColumn(
                name: "ImageKey",
                table: "FlowerTable");

            migrationBuilder.RenameTable(
                name: "FlowerTable",
                newName: "FLowerTable");

            migrationBuilder.AlterColumn<string>(
                name: "FlowerType",
                table: "FLowerTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FlowerName",
                table: "FLowerTable",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FLowerTable",
                table: "FLowerTable",
                column: "FlowerID");
        }
    }
}

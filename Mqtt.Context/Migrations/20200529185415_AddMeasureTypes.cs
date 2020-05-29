using Microsoft.EntityFrameworkCore.Migrations;

namespace com.b_velop.Mqtt.Context.Migrations
{
    public partial class AddMeasureTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureValues_MeasureType_MeasureTypeName",
                table: "MeasureValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeasureType",
                table: "MeasureType");

            migrationBuilder.RenameTable(
                name: "MeasureType",
                newName: "MeasureTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeasureTypes",
                table: "MeasureTypes",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureValues_MeasureTypes_MeasureTypeName",
                table: "MeasureValues",
                column: "MeasureTypeName",
                principalTable: "MeasureTypes",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeasureValues_MeasureTypes_MeasureTypeName",
                table: "MeasureValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeasureTypes",
                table: "MeasureTypes");

            migrationBuilder.RenameTable(
                name: "MeasureTypes",
                newName: "MeasureType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeasureType",
                table: "MeasureType",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasureValues_MeasureType_MeasureTypeName",
                table: "MeasureValues",
                column: "MeasureTypeName",
                principalTable: "MeasureType",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

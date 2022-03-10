using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_VaccinationSlot_VaccinationSlotId",
                table: "Vaccinations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaccinationSlot",
                table: "VaccinationSlot");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.RenameTable(
                name: "VaccinationSlot",
                newName: "VaccinationSlots");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "Admin");

            migrationBuilder.AddColumn<bool>(
                name: "Reserved",
                table: "VaccinationSlots",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaccinationSlots",
                table: "VaccinationSlots",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admin",
                table: "Admin",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_VaccinationSlots_VaccinationSlotId",
                table: "Vaccinations",
                column: "VaccinationSlotId",
                principalTable: "VaccinationSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_VaccinationSlots_VaccinationSlotId",
                table: "Vaccinations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VaccinationSlots",
                table: "VaccinationSlots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admin",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "Reserved",
                table: "VaccinationSlots");

            migrationBuilder.RenameTable(
                name: "VaccinationSlots",
                newName: "VaccinationSlot");

            migrationBuilder.RenameTable(
                name: "Admin",
                newName: "Admins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VaccinationSlot",
                table: "VaccinationSlot",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_VaccinationSlot_VaccinationSlotId",
                table: "Vaccinations",
                column: "VaccinationSlotId",
                principalTable: "VaccinationSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

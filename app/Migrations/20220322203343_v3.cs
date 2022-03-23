using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "VaccinationSlots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$menoKjiBr3ewxMKOXJhwrPUE8avlPjpYcicxuITMxvW29ez5");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$APn07rPFB0UlELBndUYQV9efJG2cMlAxFzWzPNvQq6vmTcSn");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$C4t/Mm46towqEtwlGnmuaounq2HFCFVAR0eETYBh7I0q3SUW");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorId",
                table: "VaccinationSlots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$TPB3XeZef6YSliBzRpnJEvRNE10G7URoVtxgRk69qK79wH/R");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$AwtMgbYOEAK1rPF/aebuXWRRR4+WGI/b9AnZSE6rpg/XKMcy");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$PGbYKdwWhrfnKEdEPf6Lcnmb3tBNhILtbqyP9iCwWVKAFNrA");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

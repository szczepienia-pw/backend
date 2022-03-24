using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class v4 : Migration
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
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Doctors",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$eADhyFswnID7A2eLofKm7vzkCx7wysB23lrJOIWyL8o868kt");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$wsEcsdNf6hvp1wu72fIsv/ySElRfkfHFx5NeFlUGHROiQOp9");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$ZhbqRfltk33d0T4u6+JhV0h2HGzK3pSY+M5F6G8fS6LAnHlH");

            migrationBuilder.AddForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VaccinationSlots_Doctors_DoctorId",
                table: "VaccinationSlots");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Doctors");

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
    }
}

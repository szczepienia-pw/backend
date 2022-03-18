using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "VaccinationSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSlots_DoctorId",
                table: "VaccinationSlots",
                column: "DoctorId");

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

            migrationBuilder.DropIndex(
                name: "IX_VaccinationSlots_DoctorId",
                table: "VaccinationSlots");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "VaccinationSlots");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$mqeo9mY32GPrrbVp6xp9TYQAu8eqJDYk5SUp2reRAyYZvpzq");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$6EB6EjuT8drcPhVQg7AUyOoWjcjjtcJqIyFEAMPyvTVHNkR/");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$/KNLglM2hKDJJORqyro48zoP+fZJ7WEqLU8asX8rkIJRwctd");
        }
    }
}

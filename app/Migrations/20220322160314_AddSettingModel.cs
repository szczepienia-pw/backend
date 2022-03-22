using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    public partial class AddSettingModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$HwBWQkzs+jupwhHvNYpzV8xfg0bLz/midDQjkxDXRYQ6y/Ni");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$DliiEII2gp5KY/vw0l311BFwSndF4aJ9EWbq9UpSL61XBUWQ");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$10000$rapEF1Kim78mfT3sgThrQ5rqu6kuPi0wWZVM/x8ulOKAlgyV");

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Type", "Value" },
                values: new object[] { 1, 0, "bugmail@szczepienia.pw" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");

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
        }
    }
}

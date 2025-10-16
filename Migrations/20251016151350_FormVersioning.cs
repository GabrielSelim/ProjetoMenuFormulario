using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormEngineAPI.Migrations
{
    /// <inheritdoc />
    public partial class FormVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormVersion",
                table: "Menus",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OriginalFormId",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseLatestVersion",
                table: "Menus",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "FormVersion",
                table: "FormSubmissions",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OriginalFormId",
                table: "FormSubmissions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Forms",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "Forms",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginalFormId",
                table: "Forms",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 16, 15, 13, 49, 860, DateTimeKind.Utc).AddTicks(5513), "$2a$11$UfGeH305iO.rGe.dFrURL.AAewIx7Gfd/4iDejjJD/rGeHZTY3u8a", new DateTime(2025, 10, 16, 15, 13, 49, 860, DateTimeKind.Utc).AddTicks(5519) });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_OriginalFormId",
                table: "Menus",
                column: "OriginalFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_OriginalFormId",
                table: "Forms",
                column: "OriginalFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_OriginalFormId_IsLatest",
                table: "Forms",
                columns: new[] { "OriginalFormId", "IsLatest" });

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Forms_OriginalFormId",
                table: "Forms",
                column: "OriginalFormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Forms_OriginalFormId",
                table: "Menus",
                column: "OriginalFormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Forms_OriginalFormId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Forms_OriginalFormId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_OriginalFormId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Forms_OriginalFormId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_OriginalFormId_IsLatest",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "FormVersion",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "OriginalFormId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "UseLatestVersion",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "FormVersion",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "OriginalFormId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "OriginalFormId",
                table: "Forms");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "Forms",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 13, 18, 52, 2, 332, DateTimeKind.Utc).AddTicks(1115), "$2a$11$IkRglqJZ.T2mdd5D8ngasOXskx1SKOyD5rF9o8oTTOTE98c80Ah8S", new DateTime(2025, 10, 13, 18, 52, 2, 332, DateTimeKind.Utc).AddTicks(1121) });
        }
    }
}

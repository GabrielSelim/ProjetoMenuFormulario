using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormEngineAPI.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowSubmissaoV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "FormSubmissions",
                type: "JSON",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAprovacao",
                table: "FormSubmissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "FormSubmissions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataExclusao",
                table: "FormSubmissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataSubmissao",
                table: "FormSubmissions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoIp",
                table: "FormSubmissions",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "FormSubmissions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRejeicao",
                table: "FormSubmissions",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FormSubmissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "FormSubmissions",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioAprovadorId",
                table: "FormSubmissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Versao",
                table: "FormSubmissions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "HistoricoFormSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FormSubmissionId = table.Column<int>(type: "int", nullable: false),
                    Acao = table.Column<int>(type: "int", nullable: false),
                    DataAcao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusAnterior = table.Column<int>(type: "int", nullable: true),
                    NovoStatus = table.Column<int>(type: "int", nullable: true),
                    EnderecoIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DadosAlteracao = table.Column<string>(type: "JSON", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoFormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricoFormSubmissions_FormSubmissions_FormSubmissionId",
                        column: x => x.FormSubmissionId,
                        principalTable: "FormSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoricoFormSubmissions_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 13, 18, 52, 2, 332, DateTimeKind.Utc).AddTicks(1115), "$2a$11$IkRglqJZ.T2mdd5D8ngasOXskx1SKOyD5rF9o8oTTOTE98c80Ah8S", new DateTime(2025, 10, 13, 18, 52, 2, 332, DateTimeKind.Utc).AddTicks(1121) });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_DataAprovacao",
                table: "FormSubmissions",
                column: "DataAprovacao");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_DataSubmissao",
                table: "FormSubmissions",
                column: "DataSubmissao");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_Excluido",
                table: "FormSubmissions",
                column: "Excluido");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FormId_Status",
                table: "FormSubmissions",
                columns: new[] { "FormId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_Status",
                table: "FormSubmissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_UserId_Status",
                table: "FormSubmissions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_UsuarioAprovadorId",
                table: "FormSubmissions",
                column: "UsuarioAprovadorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFormSubmissions_Acao",
                table: "HistoricoFormSubmissions",
                column: "Acao");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFormSubmissions_DataAcao",
                table: "HistoricoFormSubmissions",
                column: "DataAcao");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFormSubmissions_FormSubmissionId",
                table: "HistoricoFormSubmissions",
                column: "FormSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFormSubmissions_FormSubmissionId_DataAcao",
                table: "HistoricoFormSubmissions",
                columns: new[] { "FormSubmissionId", "DataAcao" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoFormSubmissions_UsuarioId",
                table: "HistoricoFormSubmissions",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSubmissions_Users_UsuarioAprovadorId",
                table: "FormSubmissions",
                column: "UsuarioAprovadorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSubmissions_Users_UsuarioAprovadorId",
                table: "FormSubmissions");

            migrationBuilder.DropTable(
                name: "HistoricoFormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_DataAprovacao",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_DataSubmissao",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_Excluido",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_FormId_Status",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_Status",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_UserId_Status",
                table: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_FormSubmissions_UsuarioAprovadorId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "DataAprovacao",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "DataExclusao",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "DataSubmissao",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "EnderecoIp",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "MotivoRejeicao",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "UsuarioAprovadorId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Versao",
                table: "FormSubmissions");

            migrationBuilder.AlterColumn<string>(
                name: "DataJson",
                table: "FormSubmissions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "JSON")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 10, 11, 11, 53, 611, DateTimeKind.Utc).AddTicks(2124), "$2a$11$W2X9EkumwcO69Al3j5LvluYEPMv4hy2wNMDqeuDRn7M1BnS35bEuq", new DateTime(2025, 10, 10, 11, 11, 53, 611, DateTimeKind.Utc).AddTicks(2136) });

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FormId",
                table: "FormSubmissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_UserId",
                table: "FormSubmissions",
                column: "UserId");
        }
    }
}

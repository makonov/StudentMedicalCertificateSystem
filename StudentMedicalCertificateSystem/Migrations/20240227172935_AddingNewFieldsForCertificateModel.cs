using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMedicalCertificateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingNewFieldsForCertificateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "RecoveryDate",
                table: "MedicalCertificates",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "IllnessDate",
                table: "MedicalCertificates",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "CertificateNumber",
                table: "MedicalCertificates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicAnswerPath",
                table: "MedicalCertificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "MedicalCertificates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "MedicalCertificates",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateNumber",
                table: "MedicalCertificates");

            migrationBuilder.DropColumn(
                name: "ClinicAnswerPath",
                table: "MedicalCertificates");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "MedicalCertificates");

            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "MedicalCertificates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RecoveryDate",
                table: "MedicalCertificates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IllnessDate",
                table: "MedicalCertificates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

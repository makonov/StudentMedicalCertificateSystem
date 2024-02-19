using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMedicalCertificateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingRestrectedDeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalCertificates_Clinics_ClinicID",
                table: "MedicalCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalCertificates_Diagnoses_DiagnosisID",
                table: "MedicalCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalCertificates_Clinics_ClinicID",
                table: "MedicalCertificates",
                column: "ClinicID",
                principalTable: "Clinics",
                principalColumn: "ClinicID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalCertificates_Diagnoses_DiagnosisID",
                table: "MedicalCertificates",
                column: "DiagnosisID",
                principalTable: "Diagnoses",
                principalColumn: "DiagnosisID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students",
                column: "GroupID",
                principalTable: "StudentGroups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalCertificates_Clinics_ClinicID",
                table: "MedicalCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalCertificates_Diagnoses_DiagnosisID",
                table: "MedicalCertificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalCertificates_Clinics_ClinicID",
                table: "MedicalCertificates",
                column: "ClinicID",
                principalTable: "Clinics",
                principalColumn: "ClinicID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalCertificates_Diagnoses_DiagnosisID",
                table: "MedicalCertificates",
                column: "DiagnosisID",
                principalTable: "Diagnoses",
                principalColumn: "DiagnosisID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students",
                column: "GroupID",
                principalTable: "StudentGroups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

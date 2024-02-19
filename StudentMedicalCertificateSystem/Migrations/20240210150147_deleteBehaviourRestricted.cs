using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMedicalCertificateSystem.Migrations
{
    /// <inheritdoc />
    public partial class deleteBehaviourRestricted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_EducationalOffices_OfficeID",
                table: "Students",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

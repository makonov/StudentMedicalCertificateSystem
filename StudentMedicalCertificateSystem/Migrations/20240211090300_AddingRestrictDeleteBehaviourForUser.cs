using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMedicalCertificateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingRestrictDeleteBehaviourForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EducationalOffices_OfficeID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "OfficeID",
                table: "AspNetUsers",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EducationalOffices_OfficeID",
                table: "AspNetUsers",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Restrict); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EducationalOffices_OfficeID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "OfficeID",
                table: "AspNetUsers",
                nullable: true,
                oldNullable: false); 

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EducationalOffices_OfficeID",
                table: "AspNetUsers",
                column: "OfficeID",
                principalTable: "EducationalOffices",
                principalColumn: "OfficeID",
                onDelete: ReferentialAction.Cascade); 
        }
    }
}

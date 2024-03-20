using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMedicalCertificateSystem.Migrations
{
    /// <inheritdoc />
    public partial class DeleteGroupsOnCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students",
                column: "GroupID",
                principalTable: "StudentGroups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_StudentGroups_GroupID",
                table: "Students",
                column: "GroupID",
                principalTable: "StudentGroups",
                principalColumn: "GroupID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

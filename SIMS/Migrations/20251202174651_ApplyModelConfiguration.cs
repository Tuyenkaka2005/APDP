using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMS.Migrations
{
    /// <inheritdoc />
    public partial class ApplyModelConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicProgram_Departments_DepartmentId",
                table: "AcademicProgram");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AcademicProgram_AcademicProgramProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AcademicProgramProgramId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicProgram",
                table: "AcademicProgram");

            migrationBuilder.DropColumn(
                name: "AcademicProgramProgramId",
                table: "Students");

            migrationBuilder.RenameTable(
                name: "AcademicProgram",
                newName: "AcademicPrograms");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Students",
                newName: "AcademicProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicProgram_DepartmentId",
                table: "AcademicPrograms",
                newName: "IX_AcademicPrograms_DepartmentId");

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                table: "Students",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalGrade",
                table: "Grades",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalScore",
                table: "Enrollments",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MidtermScore",
                table: "Enrollments",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Enrollments",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicPrograms",
                table: "AcademicPrograms",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AcademicProgramId",
                table: "Students",
                column: "AcademicProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPrograms_Departments_DepartmentId",
                table: "AcademicPrograms",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AcademicPrograms_AcademicProgramId",
                table: "Students",
                column: "AcademicProgramId",
                principalTable: "AcademicPrograms",
                principalColumn: "ProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPrograms_Departments_DepartmentId",
                table: "AcademicPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AcademicPrograms_AcademicProgramId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_AcademicProgramId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicPrograms",
                table: "AcademicPrograms");

            migrationBuilder.RenameTable(
                name: "AcademicPrograms",
                newName: "AcademicProgram");

            migrationBuilder.RenameColumn(
                name: "AcademicProgramId",
                table: "Students",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicPrograms_DepartmentId",
                table: "AcademicProgram",
                newName: "IX_AcademicProgram_DepartmentId");

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                table: "Students",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AddColumn<int>(
                name: "AcademicProgramProgramId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalGrade",
                table: "Grades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalScore",
                table: "Enrollments",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MidtermScore",
                table: "Enrollments",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalScore",
                table: "Enrollments",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicProgram",
                table: "AcademicProgram",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AcademicProgramProgramId",
                table: "Students",
                column: "AcademicProgramProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicProgram_Departments_DepartmentId",
                table: "AcademicProgram",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AcademicProgram_AcademicProgramProgramId",
                table: "Students",
                column: "AcademicProgramProgramId",
                principalTable: "AcademicProgram",
                principalColumn: "ProgramId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class StaffTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_WorkTasks_WorkTaskid",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_WorkTaskid",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "WorkTaskid",
                table: "Staffs");

            migrationBuilder.CreateTable(
                name: "StaffWorkTask",
                columns: table => new
                {
                    StaffsStaffId = table.Column<int>(type: "int", nullable: false),
                    WorkTasksid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffWorkTask", x => new { x.StaffsStaffId, x.WorkTasksid });
                    table.ForeignKey(
                        name: "FK_StaffWorkTask_Staffs_StaffsStaffId",
                        column: x => x.StaffsStaffId,
                        principalTable: "Staffs",
                        principalColumn: "StaffId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffWorkTask_WorkTasks_WorkTasksid",
                        column: x => x.WorkTasksid,
                        principalTable: "WorkTasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffWorkTask_WorkTasksid",
                table: "StaffWorkTask",
                column: "WorkTasksid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffWorkTask");

            migrationBuilder.AddColumn<int>(
                name: "WorkTaskid",
                table: "Staffs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_WorkTaskid",
                table: "Staffs",
                column: "WorkTaskid");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_WorkTasks_WorkTaskid",
                table: "Staffs",
                column: "WorkTaskid",
                principalTable: "WorkTasks",
                principalColumn: "id");
        }
    }
}

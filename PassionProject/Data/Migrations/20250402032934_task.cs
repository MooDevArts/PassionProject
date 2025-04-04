using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PassionProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkTaskid",
                table: "Staffs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkTasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTasks", x => x.id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_WorkTasks_WorkTaskid",
                table: "Staffs");

            migrationBuilder.DropTable(
                name: "WorkTasks");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_WorkTaskid",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "WorkTaskid",
                table: "Staffs");
        }
    }
}

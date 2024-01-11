using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_app.Data.Migrations
{
    public partial class score : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "UserQuiz",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "UserQuiz");
        }
    }
}

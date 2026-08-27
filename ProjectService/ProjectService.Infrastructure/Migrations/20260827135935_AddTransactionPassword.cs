using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionPasswordHash",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionPasswordHash",
                table: "Users");
        }
    }
}

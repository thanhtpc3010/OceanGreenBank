using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Key", "Language", "Value", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    { "vi-LOGIN.ERR_GENERIC", "LOGIN.ERR_GENERIC", "vi", "Đã có lỗi xảy ra. Vui lòng thử lại.", now, "seed" },
                    { "en-LOGIN.ERR_GENERIC", "LOGIN.ERR_GENERIC", "en", "Something went wrong. Please try again.", now, "seed" },
                    { "vi-DASHBOARD.DAILY_TIME", "DASHBOARD.DAILY_TIME", "vi", "Hằng ngày ({{time}})", now, "seed" },
                    { "en-DASHBOARD.DAILY_TIME", "DASHBOARD.DAILY_TIME", "en", "Daily ({{time}})", now, "seed" },
                    { "vi-DASHBOARD.MANAGE_AUTO_EARN", "DASHBOARD.MANAGE_AUTO_EARN", "vi", "Quản lý AutoEarn", now, "seed" },
                    { "en-DASHBOARD.MANAGE_AUTO_EARN", "DASHBOARD.MANAGE_AUTO_EARN", "en", "Manage AutoEarn", now, "seed" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    "vi-LOGIN.ERR_GENERIC",
                    "en-LOGIN.ERR_GENERIC",
                    "vi-DASHBOARD.DAILY_TIME",
                    "en-DASHBOARD.DAILY_TIME",
                    "vi-DASHBOARD.MANAGE_AUTO_EARN",
                    "en-DASHBOARD.MANAGE_AUTO_EARN",
                });
        }
    }
}

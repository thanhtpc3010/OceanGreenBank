using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletCancelTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = new DateTime(2026, 9, 3, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Key", "Language", "Value", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    { "vi-WALLET.CANCELLED_TITLE", "WALLET.CANCELLED_TITLE", "vi", "Thanh toán đã bị hủy", now, "seed" },
                    { "en-WALLET.CANCELLED_TITLE", "WALLET.CANCELLED_TITLE", "en", "Payment cancelled", now, "seed" },
                    { "vi-WALLET.CANCELLED_DESC", "WALLET.CANCELLED_DESC", "vi", "Bạn chưa nạp tiền. Không có khoản phí nào bị trừ.", now, "seed" },
                    { "en-WALLET.CANCELLED_DESC", "WALLET.CANCELLED_DESC", "en", "You did not top up. No fee was charged.", now, "seed" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Translations",
                keyColumn: "Key",
                keyColumnType: "character varying(200)",
                keyValues: new object[]
                {
                    "WALLET.CANCELLED_TITLE", "WALLET.CANCELLED_DESC",
                });
        }
    }
}

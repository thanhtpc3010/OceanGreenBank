using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletSystemBannerTranslations : Migration
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
                    // ================= WALLET =================
                    { "vi-WALLET.TITLE", "WALLET.TITLE", "vi", "Ví {{name}}", now, "seed" },
                    { "en-WALLET.TITLE", "WALLET.TITLE", "en", "{{name}} Wallet", now, "seed" },
                    { "vi-WALLET.DEMO_SUB", "WALLET.DEMO_SUB", "vi", "Cổng thanh toán mô phỏng (demo)", now, "seed" },
                    { "en-WALLET.DEMO_SUB", "WALLET.DEMO_SUB", "en", "Mock payment gateway (demo)", now, "seed" },
                    { "vi-WALLET.LOADING", "WALLET.LOADING", "vi", "Đang tải đơn thanh toán...", now, "seed" },
                    { "en-WALLET.LOADING", "WALLET.LOADING", "en", "Loading payment...", now, "seed" },
                    { "vi-WALLET.BACK", "WALLET.BACK", "vi", "Quay về SmartBank", now, "seed" },
                    { "en-WALLET.BACK", "WALLET.BACK", "en", "Back to SmartBank", now, "seed" },
                    { "vi-WALLET.AMOUNT", "WALLET.AMOUNT", "vi", "Số tiền thanh toán", now, "seed" },
                    { "en-WALLET.AMOUNT", "WALLET.AMOUNT", "en", "Payment amount", now, "seed" },
                    { "vi-WALLET.ORDER_CODE", "WALLET.ORDER_CODE", "vi", "Mã đơn hàng", now, "seed" },
                    { "en-WALLET.ORDER_CODE", "WALLET.ORDER_CODE", "en", "Order code", now, "seed" },
                    { "vi-WALLET.TO_ACCOUNT", "WALLET.TO_ACCOUNT", "vi", "Nạp vào tài khoản", now, "seed" },
                    { "en-WALLET.TO_ACCOUNT", "WALLET.TO_ACCOUNT", "en", "Top up to account", now, "seed" },
                    { "vi-WALLET.DESC", "WALLET.DESC", "vi", "Nội dung", now, "seed" },
                    { "en-WALLET.DESC", "WALLET.DESC", "en", "Description", now, "seed" },
                    { "vi-WALLET.CONFIRM_PAY", "WALLET.CONFIRM_PAY", "vi", "Xác nhận thanh toán", now, "seed" },
                    { "en-WALLET.CONFIRM_PAY", "WALLET.CONFIRM_PAY", "en", "Confirm payment", now, "seed" },
                    { "vi-WALLET.CANCEL_PAY", "WALLET.CANCEL_PAY", "vi", "Hủy thanh toán", now, "seed" },
                    { "en-WALLET.CANCEL_PAY", "WALLET.CANCEL_PAY", "en", "Cancel payment", now, "seed" },
                    { "vi-WALLET.DEMO_NOTE", "WALLET.DEMO_NOTE", "vi", "Đây là bản demo mô phỏng — tiền sẽ được credit vào tài khoản SmartBank ngay khi bạn xác nhận.", now, "seed" },
                    { "en-WALLET.DEMO_NOTE", "WALLET.DEMO_NOTE", "en", "This is a mock demo — funds will be credited to your SmartBank account as soon as you confirm.", now, "seed" },
                    { "vi-WALLET.SUCCESS", "WALLET.SUCCESS", "vi", "Thanh toán thành công!", now, "seed" },
                    { "en-WALLET.SUCCESS", "WALLET.SUCCESS", "en", "Payment successful!", now, "seed" },
                    { "vi-WALLET.SUCCESS_DESC", "WALLET.SUCCESS_DESC", "vi", "Đã nạp {{amount}} VND vào tài khoản {{account}}", now, "seed" },
                    { "en-WALLET.SUCCESS_DESC", "WALLET.SUCCESS_DESC", "en", "Topped up {{amount}} VND into account {{account}}", now, "seed" },
                    { "vi-WALLET.TIME", "WALLET.TIME", "vi", "Thời gian", now, "seed" },
                    { "en-WALLET.TIME", "WALLET.TIME", "en", "Time", now, "seed" },

                    // ================= SYSTEM BANNER =================
                    { "vi-SYSTEM.BANNER_OK", "SYSTEM.BANNER_OK", "vi", "Hệ thống hoạt động bình thường", now, "seed" },
                    { "en-SYSTEM.BANNER_OK", "SYSTEM.BANNER_OK", "en", "System operating normally", now, "seed" },
                    { "vi-SYSTEM.BANNER_MAINT", "SYSTEM.BANNER_MAINT", "vi", "Bảo trì định kỳ 02:00 – 04:00 hằng ngày", now, "seed" },
                    { "en-SYSTEM.BANNER_MAINT", "SYSTEM.BANNER_MAINT", "en", "Scheduled maintenance 02:00 – 04:00 daily", now, "seed" },

                    // ================= PASSWORD (extra) =================
                    { "vi-PASSWORD.ERR_CURRENT_WRONG", "PASSWORD.ERR_CURRENT_WRONG", "vi", "Mật khẩu hiện tại không đúng.", now, "seed" },
                    { "en-PASSWORD.ERR_CURRENT_WRONG", "PASSWORD.ERR_CURRENT_WRONG", "en", "Current password is incorrect.", now, "seed" },
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
                    "WALLET.TITLE", "WALLET.DEMO_SUB", "WALLET.LOADING", "WALLET.BACK", "WALLET.AMOUNT",
                    "WALLET.ORDER_CODE", "WALLET.TO_ACCOUNT", "WALLET.DESC", "WALLET.CONFIRM_PAY", "WALLET.CANCEL_PAY",
                    "WALLET.DEMO_NOTE", "WALLET.SUCCESS", "WALLET.SUCCESS_DESC", "WALLET.TIME",
                    "SYSTEM.BANNER_OK", "SYSTEM.BANNER_MAINT",
                    "PASSWORD.ERR_CURRENT_WRONG",
                });
        }
    }
}

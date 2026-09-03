using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDialogAndRemainingTranslations : Migration
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
                    // ================= COMMON =================
                    { "vi-COMMON.PROCESSING", "COMMON.PROCESSING", "vi", "Đang xử lý...", now, "seed" },
                    { "en-COMMON.PROCESSING", "COMMON.PROCESSING", "en", "Processing...", now, "seed" },
                    { "vi-COMMON.CONFIRM_ACTION", "COMMON.CONFIRM_ACTION", "vi", "Xác nhận thao tác", now, "seed" },
                    { "en-COMMON.CONFIRM_ACTION", "COMMON.CONFIRM_ACTION", "en", "Confirm action", now, "seed" },
                    { "vi-COMMON.CONFIRM_DESC", "COMMON.CONFIRM_DESC", "vi", "Thao tác này sẽ thay đổi dữ liệu", now, "seed" },
                    { "en-COMMON.CONFIRM_DESC", "COMMON.CONFIRM_DESC", "en", "This action will modify data", now, "seed" },

                    // ================= DONATE =================
                    { "vi-DONATE.AMOUNT_PLACEHOLDER", "DONATE.AMOUNT_PLACEHOLDER", "vi", "Nhập số tiền", now, "seed" },
                    { "en-DONATE.AMOUNT_PLACEHOLDER", "DONATE.AMOUNT_PLACEHOLDER", "en", "Enter amount", now, "seed" },
                    { "vi-DONATE.MESSAGE_PLACEHOLDER", "DONATE.MESSAGE_PLACEHOLDER", "vi", "VD: Ủng hộ đồng bào miền Trung", now, "seed" },
                    { "en-DONATE.MESSAGE_PLACEHOLDER", "DONATE.MESSAGE_PLACEHOLDER", "en", "e.g. Support central Vietnam", now, "seed" },
                    { "vi-DONATE.FAILED", "DONATE.FAILED", "vi", "Giao dịch không thành công", now, "seed" },
                    { "en-DONATE.FAILED", "DONATE.FAILED", "en", "Transaction failed", now, "seed" },
                    { "vi-DONATE.FAILED_DESC", "DONATE.FAILED_DESC", "vi", "Vui lòng kiểm tra lại và thử lại.", now, "seed" },
                    { "en-DONATE.FAILED_DESC", "DONATE.FAILED_DESC", "en", "Please check and try again.", now, "seed" },

                    // ================= PASSWORD (dialog) =================
                    { "vi-PASSWORD.PIN_TITLE", "PASSWORD.PIN_TITLE", "vi", "Xác nhận mật khẩu giao dịch", now, "seed" },
                    { "en-PASSWORD.PIN_TITLE", "PASSWORD.PIN_TITLE", "en", "Confirm transaction password", now, "seed" },
                    { "vi-PASSWORD.PIN_DESC", "PASSWORD.PIN_DESC", "vi", "Lớp xác thực thứ 2 bảo vệ giao dịch", now, "seed" },
                    { "en-PASSWORD.PIN_DESC", "PASSWORD.PIN_DESC", "en", "2nd security layer protects your transactions", now, "seed" },
                    { "vi-PASSWORD.PIN_LABEL", "PASSWORD.PIN_LABEL", "vi", "Mật khẩu giao dịch (6 chữ số)", now, "seed" },
                    { "en-PASSWORD.PIN_LABEL", "PASSWORD.PIN_LABEL", "en", "Transaction password (6 digits)", now, "seed" },
                    { "vi-PASSWORD.PIN_HINT", "PASSWORD.PIN_HINT", "vi", "Cài đặt / đổi mã PIN tại Tài khoản → Đổi mật khẩu.", now, "seed" },
                    { "en-PASSWORD.PIN_HINT", "PASSWORD.PIN_HINT", "en", "Set / change your PIN at Account → Change password.", now, "seed" },
                    { "vi-PASSWORD.PIN_REQUIRED", "PASSWORD.PIN_REQUIRED", "vi", "Mật khẩu giao dịch phải là 6 chữ số.", now, "seed" },
                    { "en-PASSWORD.PIN_REQUIRED", "PASSWORD.PIN_REQUIRED", "en", "Transaction password must be 6 digits.", now, "seed" },
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
                    "COMMON.PROCESSING", "COMMON.CONFIRM_ACTION", "COMMON.CONFIRM_DESC",
                    "DONATE.AMOUNT_PLACEHOLDER", "DONATE.MESSAGE_PLACEHOLDER", "DONATE.FAILED", "DONATE.FAILED_DESC",
                    "PASSWORD.PIN_TITLE", "PASSWORD.PIN_DESC", "PASSWORD.PIN_LABEL", "PASSWORD.PIN_HINT", "PASSWORD.PIN_REQUIRED",
                });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountPageTranslations : Migration
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
                    { "vi-ACCOUNT.AVAILABLE_BALANCE", "ACCOUNT.AVAILABLE_BALANCE", "vi", "Số dư khả dụng", now, "seed" },
                    { "en-ACCOUNT.AVAILABLE_BALANCE", "ACCOUNT.AVAILABLE_BALANCE", "en", "Available balance", now, "seed" },
                    { "vi-ACCOUNT.OPENED_FROM", "ACCOUNT.OPENED_FROM", "vi", "Mở từ {{date}}", now, "seed" },
                    { "en-ACCOUNT.OPENED_FROM", "ACCOUNT.OPENED_FROM", "en", "Opened since {{date}}", now, "seed" },
                    { "vi-ACCOUNT.TERM", "ACCOUNT.TERM", "vi", "Kỳ hạn {{months}} tháng", now, "seed" },
                    { "en-ACCOUNT.TERM", "ACCOUNT.TERM", "en", "Term {{months}} months", now, "seed" },
                    { "vi-ACCOUNT.INTEREST", "ACCOUNT.INTEREST", "vi", "Lãi suất {{rate}}%/năm", now, "seed" },
                    { "en-ACCOUNT.INTEREST", "ACCOUNT.INTEREST", "en", "Interest {{rate}}%/year", now, "seed" },
                    { "vi-ACCOUNT.MATURITY", "ACCOUNT.MATURITY", "vi", "Đáo hạn:", now, "seed" },
                    { "en-ACCOUNT.MATURITY", "ACCOUNT.MATURITY", "en", "Maturity:", now, "seed" },
                    { "vi-ACCOUNT.MATURED", "ACCOUNT.MATURED", "vi", "Đã đáo hạn — được rút", now, "seed" },
                    { "en-ACCOUNT.MATURED", "ACCOUNT.MATURED", "en", "Matured — withdrawable", now, "seed" },
                    { "vi-ACCOUNT.DAYS_LEFT", "ACCOUNT.DAYS_LEFT", "vi", "Còn {{days}} ngày", now, "seed" },
                    { "en-ACCOUNT.DAYS_LEFT", "ACCOUNT.DAYS_LEFT", "en", "{{days}} days left", now, "seed" },
                    { "vi-ACCOUNT.STATEMENT", "ACCOUNT.STATEMENT", "vi", "Sao kê", now, "seed" },
                    { "en-ACCOUNT.STATEMENT", "ACCOUNT.STATEMENT", "en", "Statement", now, "seed" },
                    { "vi-ACCOUNT.NO_ACCOUNTS", "ACCOUNT.NO_ACCOUNTS", "vi", "Bạn chưa có tài khoản nào.", now, "seed" },
                    { "en-ACCOUNT.NO_ACCOUNTS", "ACCOUNT.NO_ACCOUNTS", "en", "You have no accounts yet.", now, "seed" },
                    { "vi-ACCOUNT.EDIT_PROFILE", "ACCOUNT.EDIT_PROFILE", "vi", "Đổi thông tin", now, "seed" },
                    { "en-ACCOUNT.EDIT_PROFILE", "ACCOUNT.EDIT_PROFILE", "en", "Edit profile", now, "seed" },
                    { "vi-ACCOUNT.EDIT_PROFILE_SUB", "ACCOUNT.EDIT_PROFILE_SUB", "vi", "Cập nhật hồ sơ cá nhân", now, "seed" },
                    { "en-ACCOUNT.EDIT_PROFILE_SUB", "ACCOUNT.EDIT_PROFILE_SUB", "en", "Update personal profile", now, "seed" },
                    { "vi-ACCOUNT.SECURITY_SUB", "ACCOUNT.SECURITY_SUB", "vi", "Bảo mật tài khoản", now, "seed" },
                    { "en-ACCOUNT.SECURITY_SUB", "ACCOUNT.SECURITY_SUB", "en", "Account security", now, "seed" },
                    { "vi-ACCOUNT.SUPPORT", "ACCOUNT.SUPPORT", "vi", "Hỗ trợ", now, "seed" },
                    { "en-ACCOUNT.SUPPORT", "ACCOUNT.SUPPORT", "en", "Support", now, "seed" },
                    { "vi-ACCOUNT.SUPPORT_SUB", "ACCOUNT.SUPPORT_SUB", "vi", "Liên hệ tổng đài 1900 0000", now, "seed" },
                    { "en-ACCOUNT.SUPPORT_SUB", "ACCOUNT.SUPPORT_SUB", "en", "Contact hotline 1900 0000", now, "seed" },
                    { "vi-ACCOUNT.DANGER_ZONE", "ACCOUNT.DANGER_ZONE", "vi", "Vùng nguy hiểm", now, "seed" },
                    { "en-ACCOUNT.DANGER_ZONE", "ACCOUNT.DANGER_ZONE", "en", "Danger zone", now, "seed" },
                    { "vi-ACCOUNT.DANGER_DESC", "ACCOUNT.DANGER_DESC", "vi", "Xóa tài khoản sẽ xóa toàn bộ hồ sơ, tài khoản ngân hàng và dữ liệu liên quan. Hành động này không thể hoàn tác.", now, "seed" },
                    { "en-ACCOUNT.DANGER_DESC", "ACCOUNT.DANGER_DESC", "en", "Deleting the account will remove all profile, bank accounts and related data. This cannot be undone.", now, "seed" },
                    { "vi-ACCOUNT.DELETING", "ACCOUNT.DELETING", "vi", "Đang xóa...", now, "seed" },
                    { "en-ACCOUNT.DELETING", "ACCOUNT.DELETING", "en", "Deleting...", now, "seed" },
                    { "vi-ACCOUNT.DELETE_MINE", "ACCOUNT.DELETE_MINE", "vi", "Xóa tài khoản của tôi", now, "seed" },
                    { "en-ACCOUNT.DELETE_MINE", "ACCOUNT.DELETE_MINE", "en", "Delete my account", now, "seed" },
                    { "vi-ACCOUNT.OPEN_SAVINGS_SUB", "ACCOUNT.OPEN_SAVINGS_SUB", "vi", "Chọn kỳ hạn và lãi suất cho sổ mới", now, "seed" },
                    { "en-ACCOUNT.OPEN_SAVINGS_SUB", "ACCOUNT.OPEN_SAVINGS_SUB", "en", "Choose term and rate for the new savings book", now, "seed" },
                    { "vi-ACCOUNT.TERM_LABEL", "ACCOUNT.TERM_LABEL", "vi", "Kỳ hạn", now, "seed" },
                    { "en-ACCOUNT.TERM_LABEL", "ACCOUNT.TERM_LABEL", "en", "Term", now, "seed" },
                    { "vi-ACCOUNT.TERM_MONTHS", "ACCOUNT.TERM_MONTHS", "vi", "{{months}} tháng", now, "seed" },
                    { "en-ACCOUNT.TERM_MONTHS", "ACCOUNT.TERM_MONTHS", "en", "{{months}} months", now, "seed" },
                    { "vi-ACCOUNT.RATE_LABEL", "ACCOUNT.RATE_LABEL", "vi", "Lãi suất (%/năm)", now, "seed" },
                    { "en-ACCOUNT.RATE_LABEL", "ACCOUNT.RATE_LABEL", "en", "Interest rate (%/year)", now, "seed" },
                    { "vi-ACCOUNT.NOTE", "ACCOUNT.NOTE", "vi", "Lưu ý:", now, "seed" },
                    { "en-ACCOUNT.NOTE", "ACCOUNT.NOTE", "en", "Note:", now, "seed" },
                    { "vi-ACCOUNT.NOTE_1", "ACCOUNT.NOTE_1", "vi", "Chỉ được rút tiền khi đáo hạn (hết kỳ hạn) để hưởng lãi suất.", now, "seed" },
                    { "en-ACCOUNT.NOTE_1", "ACCOUNT.NOTE_1", "en", "Withdraw only at maturity to earn interest.", now, "seed" },
                    { "vi-ACCOUNT.NOTE_2", "ACCOUNT.NOTE_2", "vi", "Rút trước hạn (khẩn cấp) sẽ mất toàn bộ lãi của chu kỳ.", now, "seed" },
                    { "en-ACCOUNT.NOTE_2", "ACCOUNT.NOTE_2", "en", "Early withdrawal (emergency) forfeits all cycle interest.", now, "seed" },
                    { "vi-ACCOUNT.NOTE_3", "ACCOUNT.NOTE_3", "vi", "Khoản dư còn lại sau khi rút sẽ được tự động gia hạn kỳ hạn mới.", now, "seed" },
                    { "en-ACCOUNT.NOTE_3", "ACCOUNT.NOTE_3", "en", "The remaining balance after withdrawal is auto-renewed for a new term.", now, "seed" },
                    { "vi-ACCOUNT.OPENING", "ACCOUNT.OPENING", "vi", "Đang mở...", now, "seed" },
                    { "en-ACCOUNT.OPENING", "ACCOUNT.OPENING", "en", "Opening...", now, "seed" },
                    { "vi-ACCOUNT.OPEN", "ACCOUNT.OPEN", "vi", "Mở sổ", now, "seed" },
                    { "en-ACCOUNT.OPEN", "ACCOUNT.OPEN", "en", "Open", now, "seed" },
                    { "vi-ACCOUNT.DELETE_ACCOUNT_TITLE", "ACCOUNT.DELETE_ACCOUNT_TITLE", "vi", "Xóa tài khoản ngân hàng", now, "seed" },
                    { "en-ACCOUNT.DELETE_ACCOUNT_TITLE", "ACCOUNT.DELETE_ACCOUNT_TITLE", "en", "Delete bank account", now, "seed" },
                    { "vi-ACCOUNT.DELETE_ACCOUNT_MSG", "ACCOUNT.DELETE_ACCOUNT_MSG", "vi", "Bạn có chắc muốn xóa tài khoản này? Hành động không thể hoàn tác.", now, "seed" },
                    { "en-ACCOUNT.DELETE_ACCOUNT_MSG", "ACCOUNT.DELETE_ACCOUNT_MSG", "en", "Are you sure you want to delete this account? This cannot be undone.", now, "seed" },
                    { "vi-ACCOUNT.DELETE_USER_MSG", "ACCOUNT.DELETE_USER_MSG", "vi", "Toàn bộ hồ sơ, tài khoản ngân hàng và dữ liệu liên quan sẽ bị xóa vĩnh viễn. Bạn chắc chắn chứ?", now, "seed" },
                    { "en-ACCOUNT.DELETE_USER_MSG", "ACCOUNT.DELETE_USER_MSG", "en", "All profile, bank accounts and related data will be permanently deleted. Are you sure?", now, "seed" },
                    { "vi-ACCOUNT.DELETE_PERMANENT", "ACCOUNT.DELETE_PERMANENT", "vi", "Xóa vĩnh viễn", now, "seed" },
                    { "en-ACCOUNT.DELETE_PERMANENT", "ACCOUNT.DELETE_PERMANENT", "en", "Delete permanently", now, "seed" },
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
                    "ACCOUNT.AVAILABLE_BALANCE", "ACCOUNT.OPENED_FROM", "ACCOUNT.TERM", "ACCOUNT.INTEREST", "ACCOUNT.MATURITY",
                    "ACCOUNT.MATURED", "ACCOUNT.DAYS_LEFT", "ACCOUNT.STATEMENT", "ACCOUNT.NO_ACCOUNTS",
                    "ACCOUNT.EDIT_PROFILE", "ACCOUNT.EDIT_PROFILE_SUB", "ACCOUNT.SECURITY_SUB", "ACCOUNT.SUPPORT", "ACCOUNT.SUPPORT_SUB",
                    "ACCOUNT.DANGER_ZONE", "ACCOUNT.DANGER_DESC", "ACCOUNT.DELETING", "ACCOUNT.DELETE_MINE",
                    "ACCOUNT.OPEN_SAVINGS_SUB", "ACCOUNT.TERM_LABEL", "ACCOUNT.TERM_MONTHS", "ACCOUNT.RATE_LABEL",
                    "ACCOUNT.NOTE", "ACCOUNT.NOTE_1", "ACCOUNT.NOTE_2", "ACCOUNT.NOTE_3",
                    "ACCOUNT.OPENING", "ACCOUNT.OPEN",
                    "ACCOUNT.DELETE_ACCOUNT_TITLE", "ACCOUNT.DELETE_ACCOUNT_MSG", "ACCOUNT.DELETE_USER_MSG", "ACCOUNT.DELETE_PERMANENT",
                });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSavingsAccountTranslations : Migration
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
                    { "vi-SIDEBAR.TRANSLATIONS", "SIDEBAR.TRANSLATIONS", "vi", "Bản dịch", now, "seed" },
                    { "en-SIDEBAR.TRANSLATIONS", "SIDEBAR.TRANSLATIONS", "en", "Translations", now, "seed" },

                    // ================= SAVINGS =================
                    { "vi-SAVINGS.TITLE", "SAVINGS.TITLE", "vi", "Sổ tiết kiệm theo chu kỳ", now, "seed" },
                    { "en-SAVINGS.TITLE", "SAVINGS.TITLE", "en", "Recurring savings", now, "seed" },
                    { "vi-SAVINGS.SUBTITLE", "SAVINGS.SUBTITLE", "vi", "Tự động trích tiền vào tài khoản tiết kiệm đều đặn mỗi kỳ", now, "seed" },
                    { "en-SAVINGS.SUBTITLE", "SAVINGS.SUBTITLE", "en", "Auto-transfer to your savings account every cycle", now, "seed" },
                    { "vi-SAVINGS.TOTAL_SAVED", "SAVINGS.TOTAL_SAVED", "vi", "Tổng tiền đã tiết kiệm", now, "seed" },
                    { "en-SAVINGS.TOTAL_SAVED", "SAVINGS.TOTAL_SAVED", "en", "Total saved", now, "seed" },
                    { "vi-SAVINGS.CREATE_TITLE", "SAVINGS.CREATE_TITLE", "vi", "Tạo kế hoạch gửi tiết kiệm", now, "seed" },
                    { "en-SAVINGS.CREATE_TITLE", "SAVINGS.CREATE_TITLE", "en", "Create a savings plan", now, "seed" },
                    { "vi-SAVINGS.SOURCE", "SAVINGS.SOURCE", "vi", "Tài khoản nguồn (trích tiền)", now, "seed" },
                    { "en-SAVINGS.SOURCE", "SAVINGS.SOURCE", "en", "Source account", now, "seed" },
                    { "vi-SAVINGS.TARGET", "SAVINGS.TARGET", "vi", "Tài khoản tiết kiệm đích", now, "seed" },
                    { "en-SAVINGS.TARGET", "SAVINGS.TARGET", "en", "Target savings account", now, "seed" },
                    { "vi-SAVINGS.AMOUNT", "SAVINGS.AMOUNT", "vi", "Số tiền gửi mỗi kỳ (VND) *", now, "seed" },
                    { "en-SAVINGS.AMOUNT", "SAVINGS.AMOUNT", "en", "Amount per cycle (VND) *", now, "seed" },
                    { "vi-SAVINGS.CYCLE", "SAVINGS.CYCLE", "vi", "Chu kỳ gửi", now, "seed" },
                    { "en-SAVINGS.CYCLE", "SAVINGS.CYCLE", "en", "Cycle", now, "seed" },
                    { "vi-SAVINGS.START_DATE", "SAVINGS.START_DATE", "vi", "Ngày bắt đầu", now, "seed" },
                    { "en-SAVINGS.START_DATE", "SAVINGS.START_DATE", "en", "Start date", now, "seed" },
                    { "vi-SAVINGS.CREATE", "SAVINGS.CREATE", "vi", "Tạo kế hoạch", now, "seed" },
                    { "en-SAVINGS.CREATE", "SAVINGS.CREATE", "en", "Create plan", now, "seed" },
                    { "vi-SAVINGS.CREATING", "SAVINGS.CREATING", "vi", "Đang tạo...", now, "seed" },
                    { "en-SAVINGS.CREATING", "SAVINGS.CREATING", "en", "Creating...", now, "seed" },
                    { "vi-SAVINGS.YOUR_PLANS", "SAVINGS.YOUR_PLANS", "vi", "Kế hoạch của bạn", now, "seed" },
                    { "en-SAVINGS.YOUR_PLANS", "SAVINGS.YOUR_PLANS", "en", "Your plans", now, "seed" },
                    { "vi-SAVINGS.ACTIVE", "SAVINGS.ACTIVE", "vi", "đang hoạt động", now, "seed" },
                    { "en-SAVINGS.ACTIVE", "SAVINGS.ACTIVE", "en", "active", now, "seed" },
                    { "vi-SAVINGS.NO_PLANS", "SAVINGS.NO_PLANS", "vi", "Chưa có kế hoạch tiết kiệm nào. Tạo kế hoạch đầu tiên ở trên nhé!", now, "seed" },
                    { "en-SAVINGS.NO_PLANS", "SAVINGS.NO_PLANS", "en", "No savings plans yet. Create your first plan above!", now, "seed" },
                    { "vi-SAVINGS.NEXT_CYCLE", "SAVINGS.NEXT_CYCLE", "vi", "Kỳ tới:", now, "seed" },
                    { "en-SAVINGS.NEXT_CYCLE", "SAVINGS.NEXT_CYCLE", "en", "Next cycle:", now, "seed" },
                    { "vi-SAVINGS.DEPOSIT_NOW", "SAVINGS.DEPOSIT_NOW", "vi", "Gửi ngay", now, "seed" },
                    { "en-SAVINGS.DEPOSIT_NOW", "SAVINGS.DEPOSIT_NOW", "en", "Deposit now", now, "seed" },
                    { "vi-SAVINGS.CANCELLED", "SAVINGS.CANCELLED", "vi", "Đã hủy", now, "seed" },
                    { "en-SAVINGS.CANCELLED", "SAVINGS.CANCELLED", "en", "Cancelled", now, "seed" },

                    // ================= ACCOUNT =================
                    { "vi-ACCOUNT.TITLE", "ACCOUNT.TITLE", "vi", "Quản lý tài khoản cá nhân", now, "seed" },
                    { "en-ACCOUNT.TITLE", "ACCOUNT.TITLE", "en", "Manage personal account", now, "seed" },
                    { "vi-ACCOUNT.SUBTITLE", "ACCOUNT.SUBTITLE", "vi", "Thông tin hồ sơ và danh sách tài khoản của bạn", now, "seed" },
                    { "en-ACCOUNT.SUBTITLE", "ACCOUNT.SUBTITLE", "en", "Your profile & accounts", now, "seed" },
                    { "vi-ACCOUNT.BACK", "ACCOUNT.BACK", "vi", "Quay lại Dashboard", now, "seed" },
                    { "en-ACCOUNT.BACK", "ACCOUNT.BACK", "en", "Back to Dashboard", now, "seed" },
                    { "vi-ACCOUNT.CUSTOMER", "ACCOUNT.CUSTOMER", "vi", "Khách hàng SmartBank", now, "seed" },
                    { "en-ACCOUNT.CUSTOMER", "ACCOUNT.CUSTOMER", "en", "SmartBank customer", now, "seed" },
                    { "vi-ACCOUNT.MEMBER_SINCE", "ACCOUNT.MEMBER_SINCE", "vi", "Tham gia từ", now, "seed" },
                    { "en-ACCOUNT.MEMBER_SINCE", "ACCOUNT.MEMBER_SINCE", "en", "Member since", now, "seed" },
                    { "vi-ACCOUNT.FULL_NAME", "ACCOUNT.FULL_NAME", "vi", "Họ và tên", now, "seed" },
                    { "en-ACCOUNT.FULL_NAME", "ACCOUNT.FULL_NAME", "en", "Full name", now, "seed" },
                    { "vi-ACCOUNT.EMAIL", "ACCOUNT.EMAIL", "vi", "Email", now, "seed" },
                    { "en-ACCOUNT.EMAIL", "ACCOUNT.EMAIL", "en", "Email", now, "seed" },
                    { "vi-ACCOUNT.PHONE", "ACCOUNT.PHONE", "vi", "Số điện thoại", now, "seed" },
                    { "en-ACCOUNT.PHONE", "ACCOUNT.PHONE", "en", "Phone", now, "seed" },
                    { "vi-ACCOUNT.ID_CARD", "ACCOUNT.ID_CARD", "vi", "Số CCCD/CMND", now, "seed" },
                    { "en-ACCOUNT.ID_CARD", "ACCOUNT.ID_CARD", "en", "ID card number", now, "seed" },
                    { "vi-ACCOUNT.DOB", "ACCOUNT.DOB", "vi", "Ngày sinh", now, "seed" },
                    { "en-ACCOUNT.DOB", "ACCOUNT.DOB", "en", "Date of birth", now, "seed" },
                    { "vi-ACCOUNT.GENDER", "ACCOUNT.GENDER", "vi", "Giới tính", now, "seed" },
                    { "en-ACCOUNT.GENDER", "ACCOUNT.GENDER", "en", "Gender", now, "seed" },
                    { "vi-ACCOUNT.ADDRESS", "ACCOUNT.ADDRESS", "vi", "Địa chỉ thường trú", now, "seed" },
                    { "en-ACCOUNT.ADDRESS", "ACCOUNT.ADDRESS", "en", "Address", now, "seed" },
                    { "vi-ACCOUNT.MY_ACCOUNTS", "ACCOUNT.MY_ACCOUNTS", "vi", "Tài khoản của tôi", now, "seed" },
                    { "en-ACCOUNT.MY_ACCOUNTS", "ACCOUNT.MY_ACCOUNTS", "en", "My accounts", now, "seed" },
                    { "vi-ACCOUNT.ACTIVE_COUNT", "ACCOUNT.ACTIVE_COUNT", "vi", "tài khoản đang hoạt động", now, "seed" },
                    { "en-ACCOUNT.ACTIVE_COUNT", "ACCOUNT.ACTIVE_COUNT", "en", "active accounts", now, "seed" },
                    { "vi-ACCOUNT.OPEN_SAVINGS", "ACCOUNT.OPEN_SAVINGS", "vi", "Mở sổ tiết kiệm", now, "seed" },
                    { "en-ACCOUNT.OPEN_SAVINGS", "ACCOUNT.OPEN_SAVINGS", "en", "Open savings", now, "seed" },
                    { "vi-ACCOUNT.ADD_ACCOUNT", "ACCOUNT.ADD_ACCOUNT", "vi", "Thêm tài khoản", now, "seed" },
                    { "en-ACCOUNT.ADD_ACCOUNT", "ACCOUNT.ADD_ACCOUNT", "en", "Add account", now, "seed" },
                    { "vi-ACCOUNT.TOTAL_BALANCE", "ACCOUNT.TOTAL_BALANCE", "vi", "Tổng số dư", now, "seed" },
                    { "en-ACCOUNT.TOTAL_BALANCE", "ACCOUNT.TOTAL_BALANCE", "en", "Total balance", now, "seed" },
                    { "vi-ACCOUNT.ACTIVE", "ACCOUNT.ACTIVE", "vi", "Hoạt động", now, "seed" },
                    { "en-ACCOUNT.ACTIVE", "ACCOUNT.ACTIVE", "en", "Active", now, "seed" },
                    { "vi-ACCOUNT.CLOSED", "ACCOUNT.CLOSED", "vi", "Đã đóng", now, "seed" },
                    { "en-ACCOUNT.CLOSED", "ACCOUNT.CLOSED", "en", "Closed", now, "seed" },
                    { "vi-ACCOUNT.DELETE", "ACCOUNT.DELETE", "vi", "Xóa tài khoản", now, "seed" },
                    { "en-ACCOUNT.DELETE", "ACCOUNT.DELETE", "en", "Delete account", now, "seed" },
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
                    "SIDEBAR.TRANSLATIONS",
                    "SAVINGS.TITLE", "SAVINGS.SUBTITLE", "SAVINGS.TOTAL_SAVED", "SAVINGS.CREATE_TITLE",
                    "SAVINGS.SOURCE", "SAVINGS.TARGET", "SAVINGS.AMOUNT", "SAVINGS.CYCLE",
                    "SAVINGS.START_DATE", "SAVINGS.CREATE", "SAVINGS.CREATING", "SAVINGS.YOUR_PLANS",
                    "SAVINGS.ACTIVE", "SAVINGS.NO_PLANS", "SAVINGS.NEXT_CYCLE", "SAVINGS.DEPOSIT_NOW",
                    "SAVINGS.CANCELLED", "ACCOUNT.TITLE", "ACCOUNT.SUBTITLE", "ACCOUNT.BACK",
                    "ACCOUNT.CUSTOMER", "ACCOUNT.MEMBER_SINCE", "ACCOUNT.FULL_NAME", "ACCOUNT.EMAIL",
                    "ACCOUNT.PHONE", "ACCOUNT.ID_CARD", "ACCOUNT.DOB", "ACCOUNT.GENDER", "ACCOUNT.ADDRESS",
                    "ACCOUNT.MY_ACCOUNTS", "ACCOUNT.ACTIVE_COUNT", "ACCOUNT.OPEN_SAVINGS", "ACCOUNT.ADD_ACCOUNT",
                    "ACCOUNT.TOTAL_BALANCE", "ACCOUNT.ACTIVE", "ACCOUNT.CLOSED", "ACCOUNT.DELETE",
                });
        }
    }
}

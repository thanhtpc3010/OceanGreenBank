using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Key_Language",
                table: "Translations",
                columns: new[] { "Key", "Language" },
                unique: true);

            var now = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Key", "Language", "Value", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    // ================= COMMON =================
                    { "vi-COMMON.CANCEL", "COMMON.CANCEL", "vi", "Hủy", now, "seed" },
                    { "en-COMMON.CANCEL", "COMMON.CANCEL", "en", "Cancel", now, "seed" },
                    { "vi-COMMON.CONFIRM", "COMMON.CONFIRM", "vi", "Xác nhận", now, "seed" },
                    { "en-COMMON.CONFIRM", "COMMON.CONFIRM", "en", "Confirm", now, "seed" },
                    { "vi-COMMON.BACK", "COMMON.BACK", "vi", "Quay lại", now, "seed" },
                    { "en-COMMON.BACK", "COMMON.BACK", "en", "Back", now, "seed" },
                    { "vi-COMMON.CONTINUE", "COMMON.CONTINUE", "vi", "Tiếp tục", now, "seed" },
                    { "en-COMMON.CONTINUE", "COMMON.CONTINUE", "en", "Continue", now, "seed" },

                    // ================= HEADER =================
                    { "vi-HEADER.BALANCE", "HEADER.BALANCE", "vi", "Số dư tài khoản", now, "seed" },
                    { "en-HEADER.BALANCE", "HEADER.BALANCE", "en", "Account balance", now, "seed" },
                    { "vi-HEADER.NOTIFICATIONS", "HEADER.NOTIFICATIONS", "vi", "Thông báo", now, "seed" },
                    { "en-HEADER.NOTIFICATIONS", "HEADER.NOTIFICATIONS", "en", "Notifications", now, "seed" },
                    { "vi-HEADER.PROFILE", "HEADER.PROFILE", "vi", "Quản lý tài khoản cá nhân", now, "seed" },
                    { "en-HEADER.PROFILE", "HEADER.PROFILE", "en", "Manage personal account", now, "seed" },
                    { "vi-HEADER.LOGOUT", "HEADER.LOGOUT", "vi", "Đăng xuất", now, "seed" },
                    { "en-HEADER.LOGOUT", "HEADER.LOGOUT", "en", "Sign out", now, "seed" },

                    // ================= SIDEBAR =================
                    { "vi-SIDEBAR.OVERVIEW", "SIDEBAR.OVERVIEW", "vi", "Tổng quan", now, "seed" },
                    { "en-SIDEBAR.OVERVIEW", "SIDEBAR.OVERVIEW", "en", "Overview", now, "seed" },
                    { "vi-SIDEBAR.DASHBOARD", "SIDEBAR.DASHBOARD", "vi", "Dashboard", now, "seed" },
                    { "en-SIDEBAR.DASHBOARD", "SIDEBAR.DASHBOARD", "en", "Dashboard", now, "seed" },
                    { "vi-SIDEBAR.ACCOUNT", "SIDEBAR.ACCOUNT", "vi", "Tài khoản", now, "seed" },
                    { "en-SIDEBAR.ACCOUNT", "SIDEBAR.ACCOUNT", "en", "Account", now, "seed" },
                    { "vi-SIDEBAR.TRANSFER", "SIDEBAR.TRANSFER", "vi", "Chuyển tiền", now, "seed" },
                    { "en-SIDEBAR.TRANSFER", "SIDEBAR.TRANSFER", "en", "Transfer", now, "seed" },
                    { "vi-SIDEBAR.DEPOSIT", "SIDEBAR.DEPOSIT", "vi", "Nạp tiền", now, "seed" },
                    { "en-SIDEBAR.DEPOSIT", "SIDEBAR.DEPOSIT", "en", "Deposit", now, "seed" },
                    { "vi-SIDEBAR.TRANSACTIONS", "SIDEBAR.TRANSACTIONS", "vi", "Giao dịch", now, "seed" },
                    { "en-SIDEBAR.TRANSACTIONS", "SIDEBAR.TRANSACTIONS", "en", "Transactions", now, "seed" },
                    { "vi-SIDEBAR.SAVINGS", "SIDEBAR.SAVINGS", "vi", "Tiết kiệm", now, "seed" },
                    { "en-SIDEBAR.SAVINGS", "SIDEBAR.SAVINGS", "en", "Savings", now, "seed" },
                    { "vi-SIDEBAR.DONATE", "SIDEBAR.DONATE", "vi", "Ủng hộ MTTQ", now, "seed" },
                    { "en-SIDEBAR.DONATE", "SIDEBAR.DONATE", "en", "Donate to VFF", now, "seed" },
                    { "vi-SIDEBAR.ADMIN_USERS", "SIDEBAR.ADMIN_USERS", "vi", "Quản lý User", now, "seed" },
                    { "en-SIDEBAR.ADMIN_USERS", "SIDEBAR.ADMIN_USERS", "en", "Manage users", now, "seed" },
                    { "vi-SIDEBAR.AUTO_EARN", "SIDEBAR.AUTO_EARN", "vi", "AutoEarn", now, "seed" },
                    { "en-SIDEBAR.AUTO_EARN", "SIDEBAR.AUTO_EARN", "en", "AutoEarn", now, "seed" },
                    { "vi-SIDEBAR.TRAIN_AI", "SIDEBAR.TRAIN_AI", "vi", "Train AI", now, "seed" },
                    { "en-SIDEBAR.TRAIN_AI", "SIDEBAR.TRAIN_AI", "en", "Train AI", now, "seed" },
                    { "vi-SIDEBAR.PFM_AI_BOT", "SIDEBAR.PFM_AI_BOT", "vi", "PFM AI Bot", now, "seed" },
                    { "en-SIDEBAR.PFM_AI_BOT", "SIDEBAR.PFM_AI_BOT", "en", "PFM AI Bot", now, "seed" },
                    { "vi-SIDEBAR.OTHER_SERVICES", "SIDEBAR.OTHER_SERVICES", "vi", "Dịch vụ khác", now, "seed" },
                    { "en-SIDEBAR.OTHER_SERVICES", "SIDEBAR.OTHER_SERVICES", "en", "Other services", now, "seed" },
                    { "vi-SIDEBAR.COMING_SOON", "SIDEBAR.COMING_SOON", "vi", "Sắp ra mắt", now, "seed" },
                    { "en-SIDEBAR.COMING_SOON", "SIDEBAR.COMING_SOON", "en", "Coming soon", now, "seed" },
                    { "vi-SIDEBAR.PREMIUM", "SIDEBAR.PREMIUM", "vi", "SmartBank Premium", now, "seed" },
                    { "en-SIDEBAR.PREMIUM", "SIDEBAR.PREMIUM", "en", "SmartBank Premium", now, "seed" },
                    { "vi-SIDEBAR.PREMIUM_DESC", "SIDEBAR.PREMIUM_DESC", "vi", "Nâng cấp để hưởng ưu đãi lãi suất & miễn phí chuyển tiền.", now, "seed" },
                    { "en-SIDEBAR.PREMIUM_DESC", "SIDEBAR.PREMIUM_DESC", "en", "Upgrade for better rates & free transfers.", now, "seed" },
                    { "vi-SIDEBAR.LEARN_MORE", "SIDEBAR.LEARN_MORE", "vi", "Tìm hiểu thêm", now, "seed" },
                    { "en-SIDEBAR.LEARN_MORE", "SIDEBAR.LEARN_MORE", "en", "Learn more", now, "seed" },

                    // ================= LOGIN =================
                    { "vi-LOGIN.SUBTITLE", "LOGIN.SUBTITLE", "vi", "Ngân hàng số thông minh", now, "seed" },
                    { "en-LOGIN.SUBTITLE", "LOGIN.SUBTITLE", "en", "Smart digital banking", now, "seed" },
                    { "vi-LOGIN.TAB_LOGIN", "LOGIN.TAB_LOGIN", "vi", "Đăng nhập", now, "seed" },
                    { "en-LOGIN.TAB_LOGIN", "LOGIN.TAB_LOGIN", "en", "Sign in", now, "seed" },
                    { "vi-LOGIN.TAB_REGISTER", "LOGIN.TAB_REGISTER", "vi", "Đăng ký", now, "seed" },
                    { "en-LOGIN.TAB_REGISTER", "LOGIN.TAB_REGISTER", "en", "Sign up", now, "seed" },
                    { "vi-LOGIN.EMAIL", "LOGIN.EMAIL", "vi", "Email / SĐT", now, "seed" },
                    { "en-LOGIN.EMAIL", "LOGIN.EMAIL", "en", "Email / Phone", now, "seed" },
                    { "vi-LOGIN.PASSWORD", "LOGIN.PASSWORD", "vi", "Mật khẩu", now, "seed" },
                    { "en-LOGIN.PASSWORD", "LOGIN.PASSWORD", "en", "Password", now, "seed" },
                    { "vi-LOGIN.FORGOT", "LOGIN.FORGOT", "vi", "Quên mật khẩu?", now, "seed" },
                    { "en-LOGIN.FORGOT", "LOGIN.FORGOT", "en", "Forgot password?", now, "seed" },
                    { "vi-LOGIN.SUBMIT", "LOGIN.SUBMIT", "vi", "Đăng nhập", now, "seed" },
                    { "en-LOGIN.SUBMIT", "LOGIN.SUBMIT", "en", "Sign in", now, "seed" },
                    { "vi-LOGIN.LOADING", "LOGIN.LOADING", "vi", "Đang đăng nhập...", now, "seed" },
                    { "en-LOGIN.LOADING", "LOGIN.LOADING", "en", "Signing in...", now, "seed" },
                    { "vi-LOGIN.REGISTER_LOADING", "LOGIN.REGISTER_LOADING", "vi", "Đang đăng ký...", now, "seed" },
                    { "en-LOGIN.REGISTER_LOADING", "LOGIN.REGISTER_LOADING", "en", "Signing up...", now, "seed" },
                    { "vi-LOGIN.FULL_NAME", "LOGIN.FULL_NAME", "vi", "Họ và tên", now, "seed" },
                    { "en-LOGIN.FULL_NAME", "LOGIN.FULL_NAME", "en", "Full name", now, "seed" },
                    { "vi-LOGIN.EMAIL_LABEL", "LOGIN.EMAIL_LABEL", "vi", "Email", now, "seed" },
                    { "en-LOGIN.EMAIL_LABEL", "LOGIN.EMAIL_LABEL", "en", "Email", now, "seed" },
                    { "vi-LOGIN.PHONE", "LOGIN.PHONE", "vi", "Số điện thoại", now, "seed" },
                    { "en-LOGIN.PHONE", "LOGIN.PHONE", "en", "Phone number", now, "seed" },
                    { "vi-LOGIN.CONFIRM_PASSWORD", "LOGIN.CONFIRM_PASSWORD", "vi", "Xác nhận", now, "seed" },
                    { "en-LOGIN.CONFIRM_PASSWORD", "LOGIN.CONFIRM_PASSWORD", "en", "Confirm", now, "seed" },
                    { "vi-LOGIN.NOT_ROBOT", "LOGIN.NOT_ROBOT", "vi", "Tôi không phải người máy", now, "seed" },
                    { "en-LOGIN.NOT_ROBOT", "LOGIN.NOT_ROBOT", "en", "I'm not a robot", now, "seed" },
                    { "vi-LOGIN.FOOTER", "LOGIN.FOOTER", "vi", "© 2026 SmartBank. Bảo mật bởi OceanGreen.", now, "seed" },
                    { "en-LOGIN.FOOTER", "LOGIN.FOOTER", "en", "© 2026 SmartBank. Secured by OceanGreen.", now, "seed" },
                    { "vi-LOGIN.ERR_EMPTY", "LOGIN.ERR_EMPTY", "vi", "Vui lòng nhập đầy đủ thông tin.", now, "seed" },
                    { "en-LOGIN.ERR_EMPTY", "LOGIN.ERR_EMPTY", "en", "Please fill in all required fields.", now, "seed" },
                    { "vi-LOGIN.ERR_PASSWORD_MISMATCH", "LOGIN.ERR_PASSWORD_MISMATCH", "vi", "Mật khẩu xác nhận không khớp.", now, "seed" },
                    { "en-LOGIN.ERR_PASSWORD_MISMATCH", "LOGIN.ERR_PASSWORD_MISMATCH", "en", "Passwords do not match.", now, "seed" },
                    { "vi-LOGIN.ERR_CAPTCHA", "LOGIN.ERR_CAPTCHA", "vi", "Vui lòng xác nhận bạn không phải người máy.", now, "seed" },
                    { "en-LOGIN.ERR_CAPTCHA", "LOGIN.ERR_CAPTCHA", "en", "Please confirm you are not a robot.", now, "seed" },

                    // ================= DASHBOARD =================
                    { "vi-DASHBOARD.GREETING", "DASHBOARD.GREETING", "vi", "Xin chào, {{name}} 👋", now, "seed" },
                    { "en-DASHBOARD.GREETING", "DASHBOARD.GREETING", "en", "Hello, {{name}} 👋", now, "seed" },
                    { "vi-DASHBOARD.TODAY", "DASHBOARD.TODAY", "vi", "Hôm nay là {{date}}", now, "seed" },
                    { "en-DASHBOARD.TODAY", "DASHBOARD.TODAY", "en", "Today is {{date}}", now, "seed" },
                    { "vi-DASHBOARD.DEPOSIT", "DASHBOARD.DEPOSIT", "vi", "+ Nạp tiền", now, "seed" },
                    { "en-DASHBOARD.DEPOSIT", "DASHBOARD.DEPOSIT", "en", "+ Deposit", now, "seed" },
                    { "vi-DASHBOARD.TRANSFER", "DASHBOARD.TRANSFER", "vi", "Chuyển tiền", now, "seed" },
                    { "en-DASHBOARD.TRANSFER", "DASHBOARD.TRANSFER", "en", "Transfer", now, "seed" },
                    { "vi-DASHBOARD.CASA", "DASHBOARD.CASA", "vi", "Tài khoản thanh toán (CASA)", now, "seed" },
                    { "en-DASHBOARD.CASA", "DASHBOARD.CASA", "en", "Current account (CASA)", now, "seed" },
                    { "vi-DASHBOARD.ACTIVE", "DASHBOARD.ACTIVE", "vi", "Hoạt động", now, "seed" },
                    { "en-DASHBOARD.ACTIVE", "DASHBOARD.ACTIVE", "en", "Active", now, "seed" },
                    { "vi-DASHBOARD.ACCOUNT_NUMBER", "DASHBOARD.ACCOUNT_NUMBER", "vi", "Số tài khoản", now, "seed" },
                    { "en-DASHBOARD.ACCOUNT_NUMBER", "DASHBOARD.ACCOUNT_NUMBER", "en", "Account number", now, "seed" },
                    { "vi-DASHBOARD.AVAILABLE_BALANCE", "DASHBOARD.AVAILABLE_BALANCE", "vi", "Số dư khả dụng", now, "seed" },
                    { "en-DASHBOARD.AVAILABLE_BALANCE", "DASHBOARD.AVAILABLE_BALANCE", "en", "Available balance", now, "seed" },
                    { "vi-DASHBOARD.STATEMENT", "DASHBOARD.STATEMENT", "vi", "Sao kê", now, "seed" },
                    { "en-DASHBOARD.STATEMENT", "DASHBOARD.STATEMENT", "en", "Statement", now, "seed" },
                    { "vi-DASHBOARD.AUTO_EARN", "DASHBOARD.AUTO_EARN", "vi", "Sinh lời tự động (AutoEarn)", now, "seed" },
                    { "en-DASHBOARD.AUTO_EARN", "DASHBOARD.AUTO_EARN", "en", "Auto-earning (AutoEarn)", now, "seed" },
                    { "vi-DASHBOARD.ON", "DASHBOARD.ON", "vi", "ĐANG BẬT", now, "seed" },
                    { "en-DASHBOARD.ON", "DASHBOARD.ON", "en", "ON", now, "seed" },
                    { "vi-DASHBOARD.OFF", "DASHBOARD.OFF", "vi", "TẮT", now, "seed" },
                    { "en-DASHBOARD.OFF", "DASHBOARD.OFF", "en", "OFF", now, "seed" },
                    { "vi-DASHBOARD.INTEREST_RATE", "DASHBOARD.INTEREST_RATE", "vi", "Lãi suất / năm", now, "seed" },
                    { "en-DASHBOARD.INTEREST_RATE", "DASHBOARD.INTEREST_RATE", "en", "Interest rate / year", now, "seed" },
                    { "vi-DASHBOARD.MONTHLY_EARNED", "DASHBOARD.MONTHLY_EARNED", "vi", "Tích lũy tháng này", now, "seed" },
                    { "en-DASHBOARD.MONTHLY_EARNED", "DASHBOARD.MONTHLY_EARNED", "en", "Earned this month", now, "seed" },
                    { "vi-DASHBOARD.AUTO_RUN", "DASHBOARD.AUTO_RUN", "vi", "Ngày tự động trích", now, "seed" },
                    { "en-DASHBOARD.AUTO_RUN", "DASHBOARD.AUTO_RUN", "en", "Auto run", now, "seed" },
                    { "vi-DASHBOARD.DAILY", "DASHBOARD.DAILY", "vi", "Hằng ngày (00:00)", now, "seed" },
                    { "en-DASHBOARD.DAILY", "DASHBOARD.DAILY", "en", "Daily (00:00)", now, "seed" },
                    { "vi-DASHBOARD.PRINCIPAL", "DASHBOARD.PRINCIPAL", "vi", "Tiền gốc tham gia", now, "seed" },
                    { "en-DASHBOARD.PRINCIPAL", "DASHBOARD.PRINCIPAL", "en", "Enrolled principal", now, "seed" },
                    { "vi-DASHBOARD.ADMIN_CONFIG", "DASHBOARD.ADMIN_CONFIG", "vi", "Quản trị viên có thể cấu hình thời gian sinh lời.", now, "seed" },
                    { "en-DASHBOARD.ADMIN_CONFIG", "DASHBOARD.ADMIN_CONFIG", "en", "Admin can configure the earning schedule.", now, "seed" },
                    { "vi-DASHBOARD.PFM_TITLE", "DASHBOARD.PFM_TITLE", "vi", "AI PFM - Thống kê thu chi tháng này", now, "seed" },
                    { "en-DASHBOARD.PFM_TITLE", "DASHBOARD.PFM_TITLE", "en", "AI PFM - Monthly income & spending", now, "seed" },
                    { "vi-DASHBOARD.PFM_DESC", "DASHBOARD.PFM_DESC", "vi", "Phân tích tự động bởi AI PFM Bot", now, "seed" },
                    { "en-DASHBOARD.PFM_DESC", "DASHBOARD.PFM_DESC", "en", "Analyzed automatically by AI PFM Bot", now, "seed" },
                    { "vi-DASHBOARD.PFM_BOT", "DASHBOARD.PFM_BOT", "vi", "BOT PFM tổng hợp", now, "seed" },
                    { "en-DASHBOARD.PFM_BOT", "DASHBOARD.PFM_BOT", "en", "PFM Bot summary", now, "seed" },
                    { "vi-DASHBOARD.TOTAL_INCOME", "DASHBOARD.TOTAL_INCOME", "vi", "Tổng thu", now, "seed" },
                    { "en-DASHBOARD.TOTAL_INCOME", "DASHBOARD.TOTAL_INCOME", "en", "Total income", now, "seed" },
                    { "vi-DASHBOARD.TOTAL_EXPENSE", "DASHBOARD.TOTAL_EXPENSE", "vi", "Tổng chi", now, "seed" },
                    { "en-DASHBOARD.TOTAL_EXPENSE", "DASHBOARD.TOTAL_EXPENSE", "en", "Total expense", now, "seed" },
                    { "vi-DASHBOARD.NET", "DASHBOARD.NET", "vi", "Số dư (Thu − Chi)", now, "seed" },
                    { "en-DASHBOARD.NET", "DASHBOARD.NET", "en", "Net (Income − Expense)", now, "seed" },
                    { "vi-DASHBOARD.EXPENSE_STRUCTURE", "DASHBOARD.EXPENSE_STRUCTURE", "vi", "Cơ cấu chi tiêu", now, "seed" },
                    { "en-DASHBOARD.EXPENSE_STRUCTURE", "DASHBOARD.EXPENSE_STRUCTURE", "en", "Spending breakdown", now, "seed" },
                    { "vi-DASHBOARD.CASHFLOW_6M", "DASHBOARD.CASHFLOW_6M", "vi", "Dòng tiền 6 tháng", now, "seed" },
                    { "en-DASHBOARD.CASHFLOW_6M", "DASHBOARD.CASHFLOW_6M", "en", "6-month cash flow", now, "seed" },
                    { "vi-DASHBOARD.RECENT_TX", "DASHBOARD.RECENT_TX", "vi", "Giao dịch gần đây", now, "seed" },
                    { "en-DASHBOARD.RECENT_TX", "DASHBOARD.RECENT_TX", "en", "Recent transactions", now, "seed" },
                    { "vi-DASHBOARD.VIEW_ALL", "DASHBOARD.VIEW_ALL", "vi", "Xem tất cả", now, "seed" },
                    { "en-DASHBOARD.VIEW_ALL", "DASHBOARD.VIEW_ALL", "en", "View all", now, "seed" },
                    { "vi-DASHBOARD.INCOME", "DASHBOARD.INCOME", "vi", "Thu", now, "seed" },
                    { "en-DASHBOARD.INCOME", "DASHBOARD.INCOME", "en", "Income", now, "seed" },
                    { "vi-DASHBOARD.EXPENSE", "DASHBOARD.EXPENSE", "vi", "Chi", now, "seed" },
                    { "en-DASHBOARD.EXPENSE", "DASHBOARD.EXPENSE", "en", "Expense", now, "seed" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");
        }
    }
}

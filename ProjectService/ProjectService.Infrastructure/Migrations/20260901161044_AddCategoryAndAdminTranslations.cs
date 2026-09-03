using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndAdminTranslations : Migration
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
                    // ================= CATEGORY =================
                    { "vi-CATEGORY.OTHER", "CATEGORY.OTHER", "vi", "Khác", now, "seed" },
                    { "en-CATEGORY.OTHER", "CATEGORY.OTHER", "en", "Other", now, "seed" },
                    { "vi-CATEGORY.FOOD", "CATEGORY.FOOD", "vi", "Ăn uống", now, "seed" },
                    { "en-CATEGORY.FOOD", "CATEGORY.FOOD", "en", "Food & drinks", now, "seed" },
                    { "vi-CATEGORY.SHOPPING", "CATEGORY.SHOPPING", "vi", "Mua sắm", now, "seed" },
                    { "en-CATEGORY.SHOPPING", "CATEGORY.SHOPPING", "en", "Shopping", now, "seed" },
                    { "vi-CATEGORY.BILLS", "CATEGORY.BILLS", "vi", "Hóa đơn", now, "seed" },
                    { "en-CATEGORY.BILLS", "CATEGORY.BILLS", "en", "Bills", now, "seed" },
                    { "vi-CATEGORY.TRANSPORT", "CATEGORY.TRANSPORT", "vi", "Di chuyển", now, "seed" },
                    { "en-CATEGORY.TRANSPORT", "CATEGORY.TRANSPORT", "en", "Transport", now, "seed" },
                    { "vi-CATEGORY.ENTERTAINMENT", "CATEGORY.ENTERTAINMENT", "vi", "Giải trí", now, "seed" },
                    { "en-CATEGORY.ENTERTAINMENT", "CATEGORY.ENTERTAINMENT", "en", "Entertainment", now, "seed" },
                    { "vi-CATEGORY.HEALTH", "CATEGORY.HEALTH", "vi", "Y tế", now, "seed" },
                    { "en-CATEGORY.HEALTH", "CATEGORY.HEALTH", "en", "Health", now, "seed" },
                    { "vi-CATEGORY.EDUCATION", "CATEGORY.EDUCATION", "vi", "Giáo dục", now, "seed" },
                    { "en-CATEGORY.EDUCATION", "CATEGORY.EDUCATION", "en", "Education", now, "seed" },
                    { "vi-CATEGORY.SAVINGS", "CATEGORY.SAVINGS", "vi", "Tiết kiệm", now, "seed" },
                    { "en-CATEGORY.SAVINGS", "CATEGORY.SAVINGS", "en", "Savings", now, "seed" },
                    { "vi-CATEGORY.TRANSFER", "CATEGORY.TRANSFER", "vi", "Chuyển khoản", now, "seed" },
                    { "en-CATEGORY.TRANSFER", "CATEGORY.TRANSFER", "en", "Transfer", now, "seed" },
                    { "vi-CATEGORY.DONATION", "CATEGORY.DONATION", "vi", "Ủng hộ / Từ thiện", now, "seed" },
                    { "en-CATEGORY.DONATION", "CATEGORY.DONATION", "en", "Donation", now, "seed" },

                    // ================= EDIT ACCOUNT =================
                    { "vi-EDIT.TITLE", "EDIT.TITLE", "vi", "Đổi thông tin cá nhân", now, "seed" },
                    { "en-EDIT.TITLE", "EDIT.TITLE", "en", "Edit personal info", now, "seed" },
                    { "vi-EDIT.SUBTITLE", "EDIT.SUBTITLE", "vi", "Cập nhật thông tin hồ sơ của bạn", now, "seed" },
                    { "en-EDIT.SUBTITLE", "EDIT.SUBTITLE", "en", "Update your profile", now, "seed" },
                    { "vi-EDIT.BACK", "EDIT.BACK", "vi", "Quay lại", now, "seed" },
                    { "en-EDIT.BACK", "EDIT.BACK", "en", "Back", now, "seed" },
                    { "vi-EDIT.FULL_NAME", "EDIT.FULL_NAME", "vi", "Họ và tên *", now, "seed" },
                    { "en-EDIT.FULL_NAME", "EDIT.FULL_NAME", "en", "Full name *", now, "seed" },
                    { "vi-EDIT.PHONE", "EDIT.PHONE", "vi", "Số điện thoại *", now, "seed" },
                    { "en-EDIT.PHONE", "EDIT.PHONE", "en", "Phone *", now, "seed" },
                    { "vi-EDIT.GENDER", "EDIT.GENDER", "vi", "Giới tính", now, "seed" },
                    { "en-EDIT.GENDER", "EDIT.GENDER", "en", "Gender", now, "seed" },
                    { "vi-EDIT.ADDRESS", "EDIT.ADDRESS", "vi", "Địa chỉ", now, "seed" },
                    { "en-EDIT.ADDRESS", "EDIT.ADDRESS", "en", "Address", now, "seed" },
                    { "vi-EDIT.IDENTITY_INFO", "EDIT.IDENTITY_INFO", "vi", "Thông tin định danh (không thể thay đổi)", now, "seed" },
                    { "en-EDIT.IDENTITY_INFO", "EDIT.IDENTITY_INFO", "en", "Identity info (cannot be changed)", now, "seed" },

                    // ================= PASSWORD ACCOUNT =================
                    { "vi-PASSWORD.TITLE", "PASSWORD.TITLE", "vi", "Đổi mật khẩu", now, "seed" },
                    { "en-PASSWORD.TITLE", "PASSWORD.TITLE", "en", "Change password", now, "seed" },
                    { "vi-PASSWORD.SUBTITLE", "PASSWORD.SUBTITLE", "vi", "Bảo mật tài khoản của bạn", now, "seed" },
                    { "en-PASSWORD.SUBTITLE", "PASSWORD.SUBTITLE", "en", "Secure your account", now, "seed" },
                    { "vi-PASSWORD.CURRENT", "PASSWORD.CURRENT", "vi", "Mật khẩu hiện tại *", now, "seed" },
                    { "en-PASSWORD.CURRENT", "PASSWORD.CURRENT", "en", "Current password *", now, "seed" },
                    { "vi-PASSWORD.NEW", "PASSWORD.NEW", "vi", "Mật khẩu mới *", now, "seed" },
                    { "en-PASSWORD.NEW", "PASSWORD.NEW", "en", "New password *", now, "seed" },
                    { "vi-PASSWORD.CONFIRM", "PASSWORD.CONFIRM", "vi", "Xác nhận mật khẩu mới *", now, "seed" },
                    { "en-PASSWORD.CONFIRM", "PASSWORD.CONFIRM", "en", "Confirm new password *", now, "seed" },
                    { "vi-PASSWORD.REQ_TITLE", "PASSWORD.REQ_TITLE", "vi", "Yêu cầu mật khẩu:", now, "seed" },
                    { "en-PASSWORD.REQ_TITLE", "PASSWORD.REQ_TITLE", "en", "Password requirements:", now, "seed" },
                    { "vi-PASSWORD.TP_TITLE", "PASSWORD.TP_TITLE", "vi", "Mật khẩu giao dịch (cấp 2)", now, "seed" },
                    { "en-PASSWORD.TP_TITLE", "PASSWORD.TP_TITLE", "en", "Transaction password (2FA)", now, "seed" },
                    { "vi-PASSWORD.TP_DESC", "PASSWORD.TP_DESC", "vi", "Lớp bảo mật thứ 2 — bắt buộc nhập khi chuyển tiền", now, "seed" },
                    { "en-PASSWORD.TP_DESC", "PASSWORD.TP_DESC", "en", "Second security layer — required for transfers", now, "seed" },
                    { "vi-PASSWORD.TP_NEW", "PASSWORD.TP_NEW", "vi", "Mật khẩu giao dịch mới *", now, "seed" },
                    { "en-PASSWORD.TP_NEW", "PASSWORD.TP_NEW", "en", "New transaction password *", now, "seed" },
                    { "vi-PASSWORD.TP_CONFIRM", "PASSWORD.TP_CONFIRM", "vi", "Xác nhận mật khẩu giao dịch *", now, "seed" },
                    { "en-PASSWORD.TP_CONFIRM", "PASSWORD.TP_CONFIRM", "en", "Confirm transaction password *", now, "seed" },

                    // ================= ADMIN =================
                    { "vi-ADMIN.USERS_TITLE", "ADMIN.USERS_TITLE", "vi", "Quản lý người dùng", now, "seed" },
                    { "en-ADMIN.USERS_TITLE", "ADMIN.USERS_TITLE", "en", "Manage users", now, "seed" },
                    { "vi-ADMIN.ADD_USER", "ADMIN.ADD_USER", "vi", "Thêm người dùng", now, "seed" },
                    { "en-ADMIN.ADD_USER", "ADMIN.ADD_USER", "en", "Add user", now, "seed" },
                    { "vi-ADMIN.AUTO_EARN_TITLE", "ADMIN.AUTO_EARN_TITLE", "vi", "Quản lý AutoEarn", now, "seed" },
                    { "en-ADMIN.AUTO_EARN_TITLE", "ADMIN.AUTO_EARN_TITLE", "en", "Manage AutoEarn", now, "seed" },
                    { "vi-ADMIN.TRAIN_AI_TITLE", "ADMIN.TRAIN_AI_TITLE", "vi", "Train trợ lý AI", now, "seed" },
                    { "en-ADMIN.TRAIN_AI_TITLE", "ADMIN.TRAIN_AI_TITLE", "en", "Train AI assistant", now, "seed" },
                    { "vi-ADMIN.ADD_KNOWLEDGE", "ADMIN.ADD_KNOWLEDGE", "vi", "Thêm kiến thức", now, "seed" },
                    { "en-ADMIN.ADD_KNOWLEDGE", "ADMIN.ADD_KNOWLEDGE", "en", "Add knowledge", now, "seed" },

                    // ================= ERROR MESSAGES =================
                    { "vi-ERR.NO_ACCOUNT", "ERR.NO_ACCOUNT", "vi", "Vui lòng chọn tài khoản nguồn.", now, "seed" },
                    { "en-ERR.NO_ACCOUNT", "ERR.NO_ACCOUNT", "en", "Please select a source account.", now, "seed" },
                    { "vi-ERR.AMOUNT", "ERR.AMOUNT", "vi", "Số tiền phải lớn hơn 0.", now, "seed" },
                    { "en-ERR.AMOUNT", "ERR.AMOUNT", "en", "Amount must be greater than 0.", now, "seed" },
                    { "vi-ERR.NO_PIN", "ERR.NO_PIN", "vi", "Vui lòng nhập mật khẩu giao dịch (cấp 2).", now, "seed" },
                    { "en-ERR.NO_PIN", "ERR.NO_PIN", "en", "Please enter your transaction password (2FA).", now, "seed" },
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
                    "CATEGORY.OTHER", "CATEGORY.FOOD", "CATEGORY.SHOPPING", "CATEGORY.BILLS", "CATEGORY.TRANSPORT",
                    "CATEGORY.ENTERTAINMENT", "CATEGORY.HEALTH", "CATEGORY.EDUCATION", "CATEGORY.SAVINGS",
                    "CATEGORY.TRANSFER", "CATEGORY.DONATION",
                    "EDIT.TITLE", "EDIT.SUBTITLE", "EDIT.BACK", "EDIT.FULL_NAME", "EDIT.PHONE", "EDIT.GENDER",
                    "EDIT.ADDRESS", "EDIT.IDENTITY_INFO",
                    "PASSWORD.TITLE", "PASSWORD.SUBTITLE", "PASSWORD.CURRENT", "PASSWORD.NEW", "PASSWORD.CONFIRM",
                    "PASSWORD.REQ_TITLE", "PASSWORD.TP_TITLE", "PASSWORD.TP_DESC", "PASSWORD.TP_NEW", "PASSWORD.TP_CONFIRM",
                    "ADMIN.USERS_TITLE", "ADMIN.ADD_USER", "ADMIN.AUTO_EARN_TITLE", "ADMIN.TRAIN_AI_TITLE", "ADMIN.ADD_KNOWLEDGE",
                    "ERR.NO_ACCOUNT", "ERR.AMOUNT", "ERR.NO_PIN",
                });
        }
    }
}

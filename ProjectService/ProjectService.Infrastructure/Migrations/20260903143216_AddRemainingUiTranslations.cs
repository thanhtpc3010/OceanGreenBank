using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingUiTranslations : Migration
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
                    // ================= DEPOSIT =================
                    { "vi-DEPOSIT.HISTORY_SUB", "DEPOSIT.HISTORY_SUB", "vi", "Các giao dịch MoMo / ZaloPay của bạn", now, "seed" },
                    { "en-DEPOSIT.HISTORY_SUB", "DEPOSIT.HISTORY_SUB", "en", "Your MoMo / ZaloPay transactions", now, "seed" },
                    { "vi-DEPOSIT.EMPTY", "DEPOSIT.EMPTY", "vi", "Chưa có giao dịch nạp tiền nào.", now, "seed" },
                    { "en-DEPOSIT.EMPTY", "DEPOSIT.EMPTY", "en", "No top-up transactions yet.", now, "seed" },
                    { "vi-DEPOSIT.COL_ORDER", "DEPOSIT.COL_ORDER", "vi", "Mã đơn", now, "seed" },
                    { "en-DEPOSIT.COL_ORDER", "DEPOSIT.COL_ORDER", "en", "Order code", now, "seed" },
                    { "vi-DEPOSIT.COL_WALLET", "DEPOSIT.COL_WALLET", "vi", "Ví", now, "seed" },
                    { "en-DEPOSIT.COL_WALLET", "DEPOSIT.COL_WALLET", "en", "Wallet", now, "seed" },
                    { "vi-DEPOSIT.COL_TIME", "DEPOSIT.COL_TIME", "vi", "Thời gian", now, "seed" },
                    { "en-DEPOSIT.COL_TIME", "DEPOSIT.COL_TIME", "en", "Time", now, "seed" },
                    { "vi-DEPOSIT.COL_ACCOUNT", "DEPOSIT.COL_ACCOUNT", "vi", "Tài khoản", now, "seed" },
                    { "en-DEPOSIT.COL_ACCOUNT", "DEPOSIT.COL_ACCOUNT", "en", "Account", now, "seed" },
                    { "vi-DEPOSIT.COL_AMOUNT", "DEPOSIT.COL_AMOUNT", "vi", "Số tiền", now, "seed" },
                    { "en-DEPOSIT.COL_AMOUNT", "DEPOSIT.COL_AMOUNT", "en", "Amount", now, "seed" },
                    { "vi-DEPOSIT.COL_STATUS", "DEPOSIT.COL_STATUS", "vi", "Trạng thái", now, "seed" },
                    { "en-DEPOSIT.COL_STATUS", "DEPOSIT.COL_STATUS", "en", "Status", now, "seed" },
                    { "vi-DEPOSIT.DEMO_NOTE", "DEPOSIT.DEMO_NOTE", "vi", "Bản demo: sau khi bấm nút, bạn sẽ được chuyển sang trang ví mô phỏng để xác nhận thanh toán.", now, "seed" },
                    { "en-DEPOSIT.DEMO_NOTE", "DEPOSIT.DEMO_NOTE", "en", "Demo: after clicking, you'll be redirected to the mock wallet to confirm payment.", now, "seed" },
                    { "vi-DEPOSIT.EWALLET_MOMO", "DEPOSIT.EWALLET_MOMO", "vi", "Ví điện tử MoMo", now, "seed" },
                    { "en-DEPOSIT.EWALLET_MOMO", "DEPOSIT.EWALLET_MOMO", "en", "MoMo E-wallet", now, "seed" },
                    { "vi-DEPOSIT.EWALLET_ZALOPAY", "DEPOSIT.EWALLET_ZALOPAY", "vi", "Ví điện tử ZaloPay", now, "seed" },
                    { "en-DEPOSIT.EWALLET_ZALOPAY", "DEPOSIT.EWALLET_ZALOPAY", "en", "ZaloPay E-wallet", now, "seed" },

                    // ================= TRANSFER =================
                    { "vi-TRANSFER.EARLY_NOTE", "TRANSFER.EARLY_NOTE", "vi", "Tôi hiểu và muốn rút trước hạn (khẩn cấp) — sẽ mất toàn bộ lãi của chu kỳ này. Khoản dư còn lại được gia hạn kỳ hạn mới.", now, "seed" },
                    { "en-TRANSFER.EARLY_NOTE", "TRANSFER.EARLY_NOTE", "en", "I understand and want to withdraw early (emergency) — forfeiting all interest of this cycle. The remaining balance is renewed for a new term.", now, "seed" },
                    { "vi-TRANSFER.WITHDRAW_TYPE", "TRANSFER.WITHDRAW_TYPE", "vi", "Loại rút tiết kiệm", now, "seed" },
                    { "en-TRANSFER.WITHDRAW_TYPE", "TRANSFER.WITHDRAW_TYPE", "en", "Savings withdrawal type", now, "seed" },
                    { "vi-TRANSFER.WITHDRAW_MATURED", "TRANSFER.WITHDRAW_MATURED", "vi", "Đáo hạn (cộng lãi)", now, "seed" },
                    { "en-TRANSFER.WITHDRAW_MATURED", "TRANSFER.WITHDRAW_MATURED", "en", "Maturity (with interest)", now, "seed" },
                    { "vi-TRANSFER.WITHDRAW_EARLY", "TRANSFER.WITHDRAW_EARLY", "vi", "Rút trước hạn (mất lãi chu kỳ)", now, "seed" },
                    { "en-TRANSFER.WITHDRAW_EARLY", "TRANSFER.WITHDRAW_EARLY", "en", "Early withdrawal (forfeit cycle interest)", now, "seed" },
                    { "vi-TRANSFER.HISTORY_COUNT", "TRANSFER.HISTORY_COUNT", "vi", "{{count}} giao dịch", now, "seed" },
                    { "en-TRANSFER.HISTORY_COUNT", "TRANSFER.HISTORY_COUNT", "en", "{{count}} transactions", now, "seed" },

                    // ================= ACCOUNT =================
                    { "vi-ACCOUNT.TYPE_SAVINGS", "ACCOUNT.TYPE_SAVINGS", "vi", "Tiết kiệm (SAVINGS)", now, "seed" },
                    { "en-ACCOUNT.TYPE_SAVINGS", "ACCOUNT.TYPE_SAVINGS", "en", "Savings (SAVINGS)", now, "seed" },
                    { "vi-ACCOUNT.TYPE_CASA", "ACCOUNT.TYPE_CASA", "vi", "Thanh toán (CASA)", now, "seed" },
                    { "en-ACCOUNT.TYPE_CASA", "ACCOUNT.TYPE_CASA", "en", "Payment (CASA)", now, "seed" },

                    // ================= AI / TRANSACTIONS =================
                    { "vi-AI.ERR_DEFAULT", "AI.ERR_DEFAULT", "vi", "Có lỗi khi kết nối AI. Vui lòng thử lại sau.", now, "seed" },
                    { "en-AI.ERR_DEFAULT", "AI.ERR_DEFAULT", "en", "Failed to connect to AI. Please try again later.", now, "seed" },
                    { "vi-TRANSACTIONS.ERR_DEFAULT", "TRANSACTIONS.ERR_DEFAULT", "vi", "Có lỗi xảy ra. Vui lòng thử lại.", now, "seed" },
                    { "en-TRANSACTIONS.ERR_DEFAULT", "TRANSACTIONS.ERR_DEFAULT", "en", "Something went wrong. Please try again.", now, "seed" },
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
                    "DEPOSIT.HISTORY_SUB", "DEPOSIT.EMPTY", "DEPOSIT.COL_ORDER", "DEPOSIT.COL_WALLET", "DEPOSIT.COL_TIME",
                    "DEPOSIT.COL_ACCOUNT", "DEPOSIT.COL_AMOUNT", "DEPOSIT.COL_STATUS", "DEPOSIT.DEMO_NOTE",
                    "DEPOSIT.EWALLET_MOMO", "DEPOSIT.EWALLET_ZALOPAY",
                    "TRANSFER.EARLY_NOTE", "TRANSFER.WITHDRAW_TYPE", "TRANSFER.WITHDRAW_MATURED", "TRANSFER.WITHDRAW_EARLY", "TRANSFER.HISTORY_COUNT",
                    "ACCOUNT.TYPE_SAVINGS", "ACCOUNT.TYPE_CASA",
                    "AI.ERR_DEFAULT", "TRANSACTIONS.ERR_DEFAULT",
                });
        }
    }
}

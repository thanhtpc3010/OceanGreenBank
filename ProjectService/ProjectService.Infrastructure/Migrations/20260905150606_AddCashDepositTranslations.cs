using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCashDepositTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = new DateTime(2026, 9, 5, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Key", "Language", "Value", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    { "vi-DEPOSIT.CASH", "DEPOSIT.CASH", "vi", "Tiền mặt", now, "seed" },
                    { "en-DEPOSIT.CASH", "DEPOSIT.CASH", "en", "Cash", now, "seed" },
                    { "vi-DEPOSIT.CASH_SUB", "DEPOSIT.CASH_SUB", "vi", "Nạp tiền mặt tại quầy / ATM", now, "seed" },
                    { "en-DEPOSIT.CASH_SUB", "DEPOSIT.CASH_SUB", "en", "Deposit cash at counter / ATM", now, "seed" },
                    { "vi-DEPOSIT.CASH_SUCCESS", "DEPOSIT.CASH_SUCCESS", "vi", "Nạp tiền mặt {{amount}} VND thành công!", now, "seed" },
                    { "en-DEPOSIT.CASH_SUCCESS", "DEPOSIT.CASH_SUCCESS", "en", "Cash deposit {{amount}} VND successful!", now, "seed" },
                    { "vi-DEPOSIT.METHOD", "DEPOSIT.METHOD", "vi", "Phương thức nạp tiền", now, "seed" },
                    { "en-DEPOSIT.METHOD", "DEPOSIT.METHOD", "en", "Deposit method", now, "seed" },
                });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.DEMO_NOTE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Bản demo: chọn ví sẽ chuyển sang trang xác nhận mô phỏng; chọn Tiền mặt sẽ ghi có ngay vào tài khoản." });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.DEMO_NOTE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Demo: choosing a wallet opens a mock confirmation page; Cash is credited instantly." });
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
                    "DEPOSIT.CASH", "DEPOSIT.CASH_SUB", "DEPOSIT.CASH_SUCCESS", "DEPOSIT.METHOD",
                });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.DEMO_NOTE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Bản demo: sau khi bấm nút, bạn sẽ được chuyển sang trang ví mô phỏng để xác nhận thanh toán." });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.DEMO_NOTE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Demo: after clicking, you'll be redirected to the mock wallet to confirm payment." });
        }
    }
}


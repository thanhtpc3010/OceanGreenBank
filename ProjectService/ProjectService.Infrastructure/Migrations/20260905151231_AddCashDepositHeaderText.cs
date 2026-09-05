using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCashDepositHeaderText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.TITLE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Nạp tiền" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.TITLE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Top-up" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.SUBTITLE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Nạp tiền vào tài khoản SmartBank bằng ví MoMo / ZaloPay hoặc tiền mặt (bản demo)" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.SUBTITLE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Top up your SmartBank account via MoMo / ZaloPay or cash (demo)" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.HISTORY_SUB", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Các giao dịch MoMo / ZaloPay / Tiền mặt của bạn" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.HISTORY_SUB", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Your MoMo / ZaloPay / Cash transactions" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.TITLE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Nạp tiền qua ví" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.TITLE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Top up via e-wallet" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.SUBTITLE", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Nạp tiền vào tài khoản SmartBank bằng ví MoMo / ZaloPay (bản demo)" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.SUBTITLE", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Top up your SmartBank account via MoMo / ZaloPay (demo)" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.HISTORY_SUB", "vi" },
                columns: new[] { "Value" },
                values: new object[] { "Các giao dịch MoMo / ZaloPay của bạn" });

            migrationBuilder.UpdateData(
                table: "Translations",
                keyColumns: new[] { "Key", "Language" },
                keyValues: new object[] { "DEPOSIT.HISTORY_SUB", "en" },
                columns: new[] { "Value" },
                values: new object[] { "Your MoMo / ZaloPay transactions" });
        }
    }
}

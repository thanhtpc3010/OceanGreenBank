using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeEntries", x => x.Id);
                });

            var now = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "KnowledgeEntries",
                columns: new[] { "Id", "Keywords", "Title", "Content", "IsActive", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    { "kn-transfer", "chuyển tiền, chuyển khoản, transfer, chuyển tiền nội bộ, chuyển nội bộ", "Hướng dẫn chuyển tiền nội bộ", "Chọn menu Chuyển tiền -> chọn 'Trong SmartBank' -> nhập số tài khoản người nhận -> bấm Tra cứu để xác nhận chủ tài khoản -> nhập số tiền, nội dung, danh mục chi tiêu -> Xác nhận. Chuyển nội bộ miễn phí, tiền vào ngay lập tức.", true, now, "seed" },
                    { "kn-interbank", "liên ngân hàng, interbank, chuyển tiền ngoài, bin, phí chuyển tiền", "Hướng dẫn chuyển tiền liên ngân hàng", "Chọn Chuyển tiền -> chọn 'Liên ngân hàng' -> nhập tên người nhận, số tài khoản, mã ngân hàng (BIN, ví dụ 970436 = VCB) -> nhập số tiền -> Xác nhận. Phí liên ngân hàng là 5.000 VND/giao dịch.", true, now, "seed" },
                    { "kn-savings-account", "tài khoản tiết kiệm, sổ tiết kiệm, savings, kỳ hạn, đáo hạn, lãi suất, rút trước hạn", "Quy định tài khoản tiết kiệm", "Tài khoản tiết kiệm (SAVINGS) có kỳ hạn 1/3/6/12 tháng với lãi suất %/năm. Chỉ được rút/chuyển tiền khi ĐÁO HẠN (hết kỳ hạn) để được hưởng lãi suất. Nếu rút TRƯỚC HẠN (khẩn cấp) sẽ MẤT TOÀN BỘ lãi của chu kỳ đó. Khoản tiền còn lại sau khi rút sẽ được tự động gia hạn kỳ hạn mới.", true, now, "seed" },
                    { "kn-open-savings", "mở sổ tiết kiệm, mở tài khoản tiết kiệm, mở số tiết kiệm", "Cách mở sổ tiết kiệm", "Vào màn hình Tài khoản -> bấm nút 'Mở sổ tiết kiệm' -> chọn kỳ hạn (1/3/6/12 tháng) -> lãi suất tự cập nhật theo kỳ hạn -> bấm 'Mở sổ'. Sau đó có thể nạp tiền vào sổ bằng cách chuyển khoản đến số tài khoản sổ tiết kiệm.", true, now, "seed" },
                    { "kn-recurring", "tiết kiệm định kỳ, gửi tự động, định kỳ, chu kỳ, recurring, auto saving", "Tiết kiệm định kỳ (tự động trích)", "Mục Tiết kiệm cho phép tạo kế hoạch gửi tiền định kỳ: chọn tài khoản nguồn, tài khoản đích, số tiền mỗi kỳ, chu kỳ (Hằng ngày / Hằng tuần / Hằng tháng) và ngày bắt đầu. Hệ thống sẽ tự động trích tiền theo chu kỳ. Có thể 'Gửi ngay' để gửi trước hoặc 'Hủy' kế hoạch bất cứ lúc nào.", true, now, "seed" },
                    { "kn-autoearn", "autoearn, sinh lời tự động, lãi suất tự động, tích lũy, tiền gốc tham gia, auto earn", "AutoEarn (sinh lời tự động)", "AutoEarn tự động cộng lãi mỗi ngày cho tài khoản tham gia theo công thức: tiền gốc x lãi suất %/năm / 365. Quản trị viên cấu hình bật/tắt, lãi suất và giờ chạy tự động (mặc định 00:00 giờ VN). Người dùng xem số tiền tích lũy tháng này ở Dashboard. Để tham gia, admin đăng ký tài khoản và nhập tiền gốc trong màn hình AutoEarn.", true, now, "seed" },
                    { "kn-pfm", "pfm, thống kê, thu chi, chi tiêu, danh mục, biểu đồ, quản lý tài chính", "Thống kê thu chi (PFM)", "Dashboard có mục PFM tổng hợp thu/chi của bạn theo danh mục (ăn uống, mua sắm, hóa đơn, di chuyển, giải trí, y tế, giáo dục, tiết kiệm, chuyển khoản...). Khi chuyển tiền hãy chọn đúng danh mục để báo cáo chính xác. Biểu đồ tròn thể hiện cơ cấu chi tiêu tháng này.", true, now, "seed" },
                    { "kn-login", "đăng nhập, đăng ký, tài khoản, mật khẩu, forgot password", "Đăng nhập / đăng ký", "Dùng email và mật khẩu để đăng nhập vào SmartBank. Chưa có tài khoản thì bấm 'Đăng ký', nhập họ tên, email, số điện thoại, mật khẩu (xác nhận 2 lần) và xác nhận không phải người máy. Nếu quên mật khẩu, liên hệ tổng đài 1900 0000 để được hỗ trợ đặt lại.", true, now, "seed" },
                    { "kn-admin-users", "quản lý user, quản trị viên, admin, rbac, phân quyền, khóa user, vai trò, quyền", "Quản lý người dùng (admin)", "Admin vào mục 'Quản lý User' để xem/tìm kiếm người dùng, tạo user mới, khóa/mở khóa tài khoản, xóa user, và xem vai trò & quyền (RBAC). Chỉ tài khoản có quyền USER.READ mới truy cập được màn hình này.", true, now, "seed" },
                    { "kn-security", "bảo mật, khóa tài khoản, tài khoản bị khóa, an toàn, otp", "Bảo mật & khóa tài khoản", "Tài khoản bị khóa sẽ không thể đăng nhập và không thể thực hiện giao dịch. Nếu tài khoản của bạn bị khóa, hãy liên hệ tổng đài 1900 0000. Không bao giờ chia sẻ mật khẩu hoặc mã OTP với bất kỳ ai. SmartBank không bao giờ yêu cầu mật khẩu qua điện thoại/chat.", true, now, "seed" },
                    { "kn-contact", "liên hệ, tổng đài, hotline, hỗ trợ, khiếu nại, hỗ trợ khách hàng", "Liên hệ hỗ trợ", "Tổng đài SmartBank: 1900 0000 (hoạt động 24/7). Bạn có thể gọi để hỗ trợ đặt lại mật khẩu, mở/khóa tài khoản, khiếu nại giao dịch hoặc mọi thắc mắc khác.", true, now, "seed" },
                    { "kn-mttq", "ủng hộ mttq, mttq, từ thiện, quyên góp, ủng hộ", "Ủng hộ MTTQ", "Tính năng Ủng hộ MTTQ (Mặt trận Tổ quốc) đang được phát triển và sẽ sớm ra mắt. Hiện tại nếu cần quyên góp, hãy liên hệ tổng đài 1900 0000.", true, now, "seed" },
                    { "kn-transfer-savings", "rút tiền tiết kiệm, rút sổ tiết kiệm, chuyển tiền từ tiết kiệm, đáo hạn tiết kiệm", "Rút tiền từ tài khoản tiết kiệm", "Để rút tiền từ sổ tiết kiệm, dùng mục Chuyển tiền với tài khoản nguồn là sổ tiết kiệm. Nếu sổ đã ĐÁO HẠN: chuyển bình thường và lãi suất được cộng vào số dư. Nếu CHƯA đáo hạn: hệ thống chặn và yêu cầu đánh dấu 'Rút trước hạn (khẩn cấp)' — khi đó toàn bộ lãi chu kỳ bị mất, phần còn lại được gia hạn kỳ hạn mới.", true, now, "seed" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnowledgeEntries");
        }
    }
}

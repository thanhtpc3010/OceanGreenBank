using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeNewFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var now = new DateTime(2026, 9, 6, 0, 0, 0, DateTimeKind.Utc);

            // Thêm kiến thức cho các tính năng mới.
            migrationBuilder.InsertData(
                table: "KnowledgeEntries",
                columns: new[] { "Id", "Keywords", "Title", "Content", "IsActive", "CreatedDate", "CreatedBy" },
                values: new object[,]
                {
                    { "kn-deposit", "nạp tiền, nạp, top up, momo, zalopay, ví điện tử, tiền mặt, nạp tiền mặt, cash, deposit", "Hướng dẫn nạp tiền vào tài khoản", "Vào menu Nạp tiền, chọn 1 trong 3 phương thức: (1) Ví MoMo — chuyển sang trang ví mô phỏng để xác nhận, mã đơn MOMO; (2) Ví ZaloPay — tương tự, mã đơn ZLP; (3) Tiền mặt — nạp tại quầy/ATM, được ghi có NGAY vào tài khoản, mã đơn CSH, không qua ví. Nhập số tiền, chọn tài khoản CASA nhận rồi bấm Tạo đơn thanh toán.", true, now, "seed" },
                    { "kn-register-casa", "đăng ký, tạo tài khoản, tài khoản thanh toán, casa, tài khoản ngân hàng, có tài khoản không, tự tạo tài khoản", "Tự động tạo tài khoản CASA khi đăng ký", "Khi đăng ký thành công, hệ thống TỰ ĐỘNG tạo ngay 1 tài khoản thanh toán CASA (loại thanh toán, tiền tệ VND, số dư 0, số tài khoản tự sinh) cho người dùng mới. Vì vậy sau khi đăng ký/đăng nhập, bạn đã có sẵn tài khoản để nhận chuyển tiền, nạp tiền hay mở sổ tiết kiệm. Đăng ký yêu cầu đầy đủ: họ tên, email, số điện thoại, CCCD/CMND, ngày sinh, địa chỉ, mật khẩu.", true, now, "seed" },
                    { "kn-statement", "lịch sử giao dịch, sao kê, giao dịch, xem giao dịch, lịch sử", "Xem lịch sử giao dịch & sao kê", "Mục Giao dịch cho phép xem toàn bộ lịch sử chuyển/nhận tiền của tài khoản (chuyển khoản, nạp tiền, ủng hộ...), gồm mã giao dịch, thời gian, nội dung, số tiền và trạng thái. Trên Dashboard cũng có khối 'Giao dịch gần đây' để xem nhanh các giao dịch mới nhất.", true, now, "seed" },
                });

            // Cập nhật các mục cũ cho khớp với tính năng hiện tại.
            migrationBuilder.UpdateData(
                table: "KnowledgeEntries",
                keyColumn: "Id",
                keyValue: "kn-login",
                columns: new[] { "Content" },
                values: new object[] { "Dùng email và mật khẩu để đăng nhập vào SmartBank. Chưa có tài khoản thì bấm 'Đăng ký' và nhập ĐẦY ĐỦ: họ tên, email, số điện thoại, số CCCD/CMND, ngày sinh, địa chỉ, mật khẩu (xác nhận 2 lần) và xác nhận không phải người máy. Ngay sau khi đăng ký, hệ thống tự tạo cho bạn 1 tài khoản thanh toán CASA (VND). Nếu quên mật khẩu, liên hệ tổng đài 1900 0000 để được hỗ trợ đặt lại." });

            migrationBuilder.UpdateData(
                table: "KnowledgeEntries",
                keyColumn: "Id",
                keyValue: "kn-mttq",
                columns: new[] { "Content" },
                values: new object[] { "Mục Ủng hộ MTTQ cho phép quyên góp đến các quỹ chính thức (Ban vận động cứu trợ Trung ương, Quỹ vì người nghèo, Quỹ phòng chống thiên tai, Hội Chữ thập đỏ...) với phí giao dịch 0đ. Chọn quỹ, nhập số tiền, chọn tài khoản nguồn rồi xác nhận bằng mật khẩu giao dịch." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeEntries",
                keyColumn: "Id",
                keyColumnType: "text",
                keyValues: new object[] { "kn-deposit", "kn-register-casa", "kn-statement" });

            migrationBuilder.UpdateData(
                table: "KnowledgeEntries",
                keyColumn: "Id",
                keyValue: "kn-login",
                columns: new[] { "Content" },
                values: new object[] { "Dùng email và mật khẩu để đăng nhập vào SmartBank. Chưa có tài khoản thì bấm 'Đăng ký', nhập họ tên, email, số điện thoại, mật khẩu (xác nhận 2 lần) và xác nhận không phải người máy. Nếu quên mật khẩu, liên hệ tổng đài 1900 0000 để được hỗ trợ đặt lại." });

            migrationBuilder.UpdateData(
                table: "KnowledgeEntries",
                keyColumn: "Id",
                keyValue: "kn-mttq",
                columns: new[] { "Content" },
                values: new object[] { "Tính năng Ủng hộ MTTQ (Mặt trận Tổ quốc) đang được phát triển và sẽ sớm ra mắt. Hiện tại nếu cần quyên góp, hãy liên hệ tổng đài 1900 0000." });
        }
    }
}


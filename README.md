# OceanGreenBank — SmartBank

**Smart Online Banking & Financial Management Platform** (Ngân hàng số & nền tảng quản lý tài chính cá nhân thông minh)

Hệ thống ngân hàng số mô phỏng đầy đủ nghiệp vụ: đăng nhập bảo mật, quản lý tài khoản, chuyển tiền nội bộ & liên ngân hàng, nạp tiền qua ví điện tử, tiết kiệm có kỳ hạn, sinh lời tự động (AutoEarn), thống kê thu chi (PFM) và trợ lý AI.

---

## Mục lục

- [1. Công nghệ sử dụng](#1-công-nghệ-sử-dụng)
- [2. Cấu trúc dự án](#2-cấu-trúc-dự-án)
- [3. Yêu cầu hệ thống](#3-yêu-cầu-hệ-thống)
- [4. Cài đặt & Chạy dự án](#4-cài-đặt--chạy-dự-án)
- [5. Tài khoản demo](#5-tài-khoản-demo)
- [6. Hướng dẫn sử dụng từng chức năng](#6-hướng-dẫn-sử-dụng-từng-chức-năng)
- [7. Kiến trúc hệ thống](#7-kiến-trúc-hệ-thống)

---

## 1. Công nghệ sử dụng

| Tầng | Công nghệ |
|------|-----------|
| **Frontend** | Angular 21, PrimeNG (UI Component Library), Tailwind CSS v4, RxJS |
| **Backend** | .NET 10 (ASP.NET Core Web API), Clean Architecture, CQRS (MediatR) |
| **Database** | PostgreSQL (Entity Framework Core — Npgsql) |
| **Job định kỳ** | ASP.NET Core BackgroundService (sinh lời tự động AutoEarn) |
| **AI** | Google Gemini (trợ lý chat, kết hợp Knowledge Base RAG) |
| **Bảo mật** | JWT Bearer, BCrypt hash mật khẩu, phân quyền RBAC |

---

## 2. Cấu trúc dự án

```
OceanGreenBank/
├── Document/
│   └── OceanGreenBank.md          # Tài liệu kỹ thuật tổng thể (spec)
├── ProjectApp/                    # Frontend Angular
│   └── src/app/
│       ├── core/                  # Auth guard/service, các service gọi API
│       ├── layout/                # Shell layout: header, sidebar, floating AI...
│       └── pages/                 # Các màn hình (login, dashboard, transfer...)
└── ProjectService/                # Backend .NET 10 (Clean Architecture)
    ├── ProjectService.Api/        # Tầng API (Controllers, Middleware, Services)
    ├── ProjectService.Application/# Tầng ứng dụng (CQRS: Commands/Queries/DTOs)
    ├── ProjectService.Domain/     # Tầng miền (Entity, Enum, Exceptions)
    └── ProjectService.Infrastructure/ # Tầng hạ tầng (EF Core, Quartz, Repositories)
```

---

## 3. Yêu cầu hệ thống

- **.NET SDK 10** (tải tại [dotnet.microsoft.com](https://dotnet.microsoft.com))
- **Node.js 18+** và **npm** (tải tại [nodejs.org](https://nodejs.org))
- Trình duyệt hiện đại (Chrome, Edge, Firefox...)

---

## 4. Cài đặt & Chạy dự án

### 4.1. Backend (API)

```powershell
# Cài dependencies & build
dotnet build ProjectService/OceanGreenBank.slnx

# Cập nhật database (chạy EF migrations)
dotnet ef database update -c ApplicationWriteDbContext --project ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj --startup-project ProjectService/ProjectService.Api/ProjectService.Api.csproj

# Chạy API tại http://localhost:5081
dotnet run --project ProjectService/ProjectService.Api --launch-profile http
```

API sẽ lắng nghe tại: **http://localhost:5081**

> Cấu hình kết nối database nằm ở `ProjectService/ProjectService.Api/appsettings.json` (mục `ConnectionStrings`). Mặc định kết nối PostgreSQL (Supabase).
>
> **Trợ lý AI (Gemini):** điền API key vào `ProjectService/ProjectService.Api/appsettings.json` (mục `Gemini.ApiKey`) nếu muốn bật chatbot. Để trống thì bot AI sẽ hiển thị trạng thái "chưa cấu hình".

### 4.2. Frontend (Angular)

```powershell
# Cài dependencies
npm --prefix ProjectApp install

# Chạy dev server tại http://localhost:4200
npm --prefix ProjectApp start -- --host 0.0.0.0 --port 4200
```

Mở trình duyệt truy cập: **http://localhost:4200**

> ⚠️ **Lưu ý:** Phải dùng `npm --prefix ProjectApp ...` khi đứng ở thư mục gốc repo. Không dùng `npm start` trực tiếp từ thư mục gốc.

---

## 5. Tài khoản demo

| Vai trò | Email | Mật khẩu | Ghi chú |
|---------|-------|----------|---------|
| **Người dùng** | `nguyenvana@gmail.com` | `password123` | Nguyễn Văn A — có tài khoản & giao dịch mẫu |
| **Người dùng** | `test@oceangreenbank.vn` | `Test@123456` | Tài khoản test |
| **Quản trị viên** | `admin@smartbank.vn` | `password123` | Có toàn quyền quản trị (RBAC) |

Hoặc bạn có thể **tự đăng ký** tài khoản mới ngay trên màn hình Đăng nhập.

---

## 6. Hướng dẫn sử dụng từng chức năng

### 6.1. Đăng nhập / Đăng ký (`/login`)

**Mục đích:** Xác thực người dùng, cấp JWT token để truy cập hệ thống.

- **Đăng nhập:** Nhập email + mật khẩu → hệ thống xác thực và trả về token + vai trò + quyền.
- **Đăng ký:** Chuyển sang tab "Đăng ký", nhập họ tên, email, số điện thoại, mật khẩu (xác nhận 2 lần) và tích chọn "Tôi không phải người máy".
- Nếu tài khoản bị khóa hoặc sai mật khẩu, hệ thống sẽ hiển thị thông báo lỗi rõ ràng.

---

### 6.2. Dashboard — Tổng quan (`/dashboard`)

**Mục đích:** Màn hình chính sau đăng nhập, tổng hợp tình hình tài chính của người dùng.

Gồm các khối:
- **Thẻ CASA (tài khoản thanh toán):** hiển thị số tài khoản và số dư khả dụng.
- **Thẻ AutoEarn (sinh lời tự động):** lãi suất/năm, tiền lãi tích lũy tháng này, tiền gốc tham gia.
- **AI PFM — Thống kê thu chi:** tổng thu, tổng chi, số dư, cơ cấu chi tiêu theo danh mục (biểu đồ tròn) và dòng tiền 6 tháng (biểu đồ cột).
- **Giao dịch gần đây:** danh sách các giao dịch mới nhất.

---

### 6.3. Tài khoản (`/account`)

**Mục đích:** Quản lý thông tin cá nhân và tài khoản ngân hàng.

- **Xem hồ sơ:** thông tin cá nhân (họ tên, email, số điện thoại, CMND, ngày sinh...).
- **Chỉnh sửa hồ sơ** (`/account/edit`): cập nhật họ tên, số điện thoại, địa chỉ.
- **Đổi mật khẩu giao dịch** (`/account/password`): đặt/đổi mã PIN giao dịch 6 số (bảo vệ cấp 2 khi chuyển tiền).
- **Danh sách tài khoản:** xem và thêm mới tài khoản (thanh toán / tiết kiệm).

---

### 6.4. Chuyển tiền (`/transfer`)

**Mục đích:** Chuyển khoản giữa các tài khoản trong hệ thống hoặc ra ngân hàng khác.

- **Chuyển nội bộ (Trong SmartBank):** chọn tài khoản nguồn, nhập số tài khoản người nhận → tra cứu để xác nhận chủ tài khoản → nhập số tiền, nội dung, danh mục chi tiêu → xác nhận. **Miễn phí, tiền vào ngay lập tức.**
- **Chuyển liên ngân hàng:** nhập tên người nhận, số tài khoản, mã ngân hàng (BIN) → nhập số tiền → xác nhận. **Phí 5.000 VND/giao dịch.**
- Yêu cầu **mã PIN giao dịch** (đã đặt ở mục 6.3) để xác nhận.

---

### 6.5. Nạp tiền (`/deposit`)

**Mục đích:** Nạp tiền vào tài khoản thông qua ví điện tử (mô phỏng).

- Chọn ví **MoMo** hoặc **ZaloPay**, chọn tài khoản nhận tiền, nhập số tiền.
- Hệ thống tạo đơn nạp tiền và chuyển sang trang ví mô phỏng (`/wallet-pay/:id`) để xác nhận thanh toán.
- Sau khi xác nhận, tiền được ghi có vào tài khoản ngay lập tức.

---

### 6.6. Giao dịch (`/transactions`)

**Mục đích:** Xem lịch sử và sao kê toàn bộ giao dịch của tài khoản.

- Lọc theo tài khoản, loại giao dịch (thu/chi), trạng thái (chờ xử lý/thành công/thất bại).
- Hủy giao dịch đang ở trạng thái chờ xử lý (nếu có).

---

### 6.7. Tiết kiệm (`/savings`)

**Mục đích:** Gửi tiết kiệm có kỳ hạn và tạo kế hoạch tiết kiệm định kỳ.

- **Mở sổ tiết kiệm:** chọn kỳ hạn (1/3/6/12 tháng), lãi suất tự cập nhật theo kỳ hạn.
- **Tiết kiệm định kỳ:** tạo kế hoạch tự động trích tiền từ tài khoản nguồn sang tài khoản tiết kiệm theo chu kỳ (hằng ngày / hằng tuần / hằng tháng).
- **Gửi ngay** một kỳ hoặc **hủy** kế hoạch bất kỳ lúc nào.
- Quy định: chỉ rút tiền khi **đáo hạn** để hưởng lãi; rút **trước hạn** sẽ mất toàn bộ lãi của chu kỳ đó.

---

### 6.8. AutoEarn — Sinh lời tự động

**Mục đích:** Tự động cộng lãi hằng ngày cho các tài khoản tham gia.

- Công thức: `tiền gốc × lãi suất %/năm ÷ 365`.
- Người dùng xem tiền lãi tích lũy tháng này ngay trên Dashboard.
- **Quản trị viên** cấu hình bật/tắt, lãi suất, giờ chạy tự động và đăng ký tài khoản + tiền gốc tham gia (xem mục 6.10.2).

---

### 6.9. Trợ lý AI — PFM AI Bot

**Mục đích:** Trò chuyện với trợ lý ảo (Google Gemini) về chức năng ngân hàng và dữ liệu cá nhân của chính bạn.

- Bấm nút **"PFM AI Bot"** ở góc phải màn hình để mở khung chat.
- Bot có thể trả lời: số dư tài khoản, giao dịch gần đây, lãi suất tiết kiệm, AutoEarn, cách dùng từng chức năng...
- Bot kết hợp **Knowledge Base** (mục 6.10.3) để trả lời đúng theo tài liệu hướng dẫn nội bộ.

> Bot cần cấu hình API key Gemini ở `appsettings.json` (mục `Gemini.ApiKey`) để hoạt động.

---

### 6.10. Quản trị (Admin) — chỉ dành cho tài khoản ADMIN

#### 6.10.1. Quản lý User (`/admin/users`)

**Mục đích:** Quản lý người dùng và phân quyền (RBAC).

- Xem, tìm kiếm, tạo mới, khóa/mở khóa, xóa người dùng.
- Xem vai trò & quyền của từng người dùng.

#### 6.10.2. Cấu hình AutoEarn (`/admin/auto-earn`)

**Mục đích:** Quản lý tính năng sinh lời tự động.

- Bật/tắt AutoEarn, chỉnh lãi suất, giờ chạy hằng ngày.
- Đăng ký tài khoản tham gia và nhập tiền gốc.
- Xem nhật ký sinh lời (AutoEarn logs).
- Chạy job ngay lập tức (Run now).

#### 6.10.3. Train AI — Knowledge Base (`/admin/knowledge`)

**Mục đích:** Quản lý kho kiến thức để "huấn luyện" trợ lý AI.

- Thêm / sửa / xóa / bật-tắt các mục kiến thức (từ khóa, tiêu đề, nội dung).
- Bot AI sẽ tự động đối chiếu câu hỏi với kho kiến thức và trả lời theo đúng tài liệu.

### 6.11. Ủng hộ Mặt trận Tổ quốc — Donate (`/donate`)

**Mục đích:** Ủng hộ tiền vào các quỹ chính thức của Việt Nam (Ban Vận động Cứu trợ Trung ương, Quỹ Vì người nghèo, Quỹ phòng chống thiên tai, Hội Chữ thập đỏ...).

- Chọn quỹ tiếp nhận và ngân hàng thụ hưởng (danh sách tài khoản có sẵn của từng quỹ).
- Nhập số tiền và nội dung ủng hộ → xác nhận bằng mã PIN giao dịch.
- Giao dịch ủng hộ **miễn phí** (không mất phí chuyển khoản) và được lưu đầy đủ trong lịch sử giao dịch.

### 6.12. Bản dịch / Đa ngôn ngữ (`/admin/translations`)

**Mục đích:** Quản lý nội dung ngôn ngữ (Tiếng Việt / English) của toàn ứng dụng — dữ liệu dịch lưu trong database.

- Xem, tìm kiếm, lọc theo ngôn ngữ toàn bộ các key dịch.
- Thêm / sửa / xóa bản dịch (có hiệu lực ngay, không cần build lại).
- Người dùng chuyển đổi ngôn ngữ bằng nút cờ 🇻🇳/🇬🇧 ở header.

---

## 7. Kiến trúc hệ thống

Hệ thống thiết kế theo **Clean Architecture + CQRS**, phân tách rõ ràng 4 tầng:

| Tầng | Trách nhiệm |
|------|-------------|
| **Api** | Xử lý HTTP, xác thực JWT, middleware, CORS |
| **Application** | Nghiệp vụ CQRS: Commands (ghi) / Queries (đọc), DTOs |
| **Domain** | Entity, Enum, Value Object, Domain Events, Exception |
| **Infrastructure** | EF Core (Read/Write DbContext), Repository, BackgroundService, Services |

**Quy tắc phụ thuộc:** `Api → Application + Infrastructure`; `Infrastructure → Application → Domain` (không phụ thuộc ngược).

Cơ sở dữ liệu tách **Read DbContext** (NoTracking, phục vụ truy vấn) và **Write DbContext** (tracking, phục vụ ghi dữ liệu) để tối ưu hiệu năng.

---

## Liên hệ & Hỗ trợ

- **Tổng đài (mô phỏng):** 1900 0000 (24/7)
- **Repository:** https://github.com/thanhtpc3010/OceanGreenBank

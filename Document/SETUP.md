# HƯỚNG DẪN CÀI ĐẶT & CHẠY SAU KHI PULL CODE VỀ (SETUP GUIDE)

Tài liệu này hướng dẫn đầy đủ các bước để chạy được dự án **OceanGreenBank** từ mã nguồn mới pull về, trên một máy sạch.

---

## 1. Yêu cầu (Prerequisites)

| Thành phần | Phiên bản | Kiểm tra |
|------------|-----------|----------|
| .NET SDK | 10.x | `dotnet --version` |
| Node.js | 18+ | `node --version` |
| npm | 9+ (đi kèm Node) | `npm --version` |
| dotnet-ef (global tool) | 10.0.10 | `dotnet ef --version` |

> Cài `dotnet-ef` nếu chưa có:
> ```powershell
> dotnet tool install --global dotnet-ef --version 10.0.10
> ```

---

## 2. Clone repository

```powershell
git clone https://github.com/thanhtpc3010/OceanGreenBank.git
cd OceanGreenBank
```

---

## 3. Cấu hình Backend (`.NET 10`)

### 3.1. Cấu hình kết nối database

Mở file `ProjectService/ProjectService.Api/appsettings.json`, kiểm tra mục `ConnectionStrings`.

Mặc định project dùng **PostgreSQL (Supabase)** thông qua connection string có sẵn. Nếu muốn trỏ sang database khác, sửa connection string tại đây.

### 3.2. Cấu hình JWT & Gemini (AI)

Trong cùng file `appsettings.json`:

- **`Jwt`**: Key / Issuer / Audience / ExpiryMinutes — giữ mặc định là được.
- **`Gemini`**:
  - `ApiKey`: để trống nếu chưa dùng chatbot AI; điền key nếu muốn bật trợ lý AI.
  - `Model`: mặc định `gemini-3.6-flash`.

### 3.3. Build & chạy

```powershell
# Build toàn bộ solution
dotnet build ProjectService/OceanGreenBank.slnx

# Cập nhật database (chạy EF migrations)
dotnet ef database update -c ApplicationWriteDbContext --project ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj --startup-project ProjectService/ProjectService.Api/ProjectService.Api.csproj

# Chạy API tại http://localhost:5081
dotnet run --project ProjectService/ProjectService.Api --launch-profile http
```

> ⚠️ **Bắt buộc chạy `dotnet ef database update`** sau khi pull code lần đầu (hoặc khi có migration mới), nếu không backend sẽ báo lỗi thiếu bảng.

---

## 4. Cấu hình Frontend (Angular 21)

```powershell
# Cài dependencies (đứng từ thư mục gốc repo)
npm --prefix ProjectApp install

# Chạy dev server tại http://localhost:4200
npm --prefix ProjectApp start -- --host 0.0.0.0 --port 4200
```

> ⚠️ Phải dùng `npm --prefix ProjectApp ...`. Không chạy `npm start` trực tiếp từ thư mục gốc (sẽ lỗi).

---

## 5. Kiểm tra hệ thống chạy thành công

1. Mở trình duyệt: **http://localhost:4200**
2. Đăng nhập bằng tài khoản demo:

| Vai trò | Email | Mật khẩu |
|---------|-------|----------|
| Người dùng | `nguyenvana@gmail.com` | `password123` |
| Người dùng | `test@oceangreenbank.vn` | `Test@123456` |
| Admin | `admin@smartbank.vn` | `password123` |

3. Kiểm tra API: `http://localhost:5081/api/translations/vi` trả về JSON.

---

## 6. Các lệnh hữu ích

| Mục đích | Lệnh |
|----------|------|
| Build backend | `dotnet build ProjectService/OceanGreenBank.slnx` |
| Chạy backend | `dotnet run --project ProjectService/ProjectService.Api --launch-profile http` |
| Build frontend (production) | `npm --prefix ProjectApp run build` |
| Chạy frontend (dev) | `npm --prefix ProjectApp start -- --host 0.0.0.0 --port 4200` |
| Regenerate Tailwind CSS | `npm --prefix ProjectApp run tailwind` |
| Thêm migration mới | `dotnet ef migrations add <Tên> -c ApplicationWriteDbContext --project ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj --startup-project ProjectService/ProjectService.Api/ProjectService.Api.csproj` |

---

## 7. Xử lý lỗi thường gặp (Troubleshooting)

| Lỗi | Nguyên nhân | Cách khắc phục |
|-----|-------------|----------------|
| Backend báo lỗi thiếu bảng / 42P01 | Chưa chạy migration | Chạy lại `dotnet ef database update` (mục 3.3) |
| Frontend không gọi được API (CORS) | Truy cập qua IP khác `localhost` | Mở bằng `http://localhost:4200` (CORS chỉ cho phép origin này) |
| Trợ lý AI không trả lời | Chưa điền `Gemini.ApiKey` | Điền key vào `appsettings.json` |
| `dotnet ef` không nhận diện | Chưa cài global tool | `dotnet tool install --global dotnet-ef --version 10.0.10` |
| Không build được backend | File `.dll` bị khóa do backend đang chạy | Tắt tiến trình backend đang chạy rồi build lại |
| Cổng 5081/4200 bị chiếm | Tiến trình cũ còn chạy | Tắt tiến trình cũ, hoặc đổi port |

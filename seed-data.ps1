# Seed dữ liệu mẫu vào Supabase qua backend API (localhost:5081)
$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5081'
$headers = @{ 'Content-Type' = 'application/json' }

function Post-Json($path, $obj) {
    $body = $obj | ConvertTo-Json -Compress
    # Gửi UTF-8 bytes để ký tự tiếng Việt không bị lỗi encoding
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($body)
    return Invoke-RestMethod -Uri "$base$path" -Method Post -ContentType "application/json; charset=utf-8" -Body $bytes
}

# ===== User 1: Nguyễn Văn A =====
$r1 = Post-Json '/api/users' @{
    FullName     = 'Nguyễn Văn A'
    Email        = 'nguyenvana@gmail.com'
    Phone        = '0912345678'
    IdentityCard = '079301012345'
    DateOfBirth  = '1998-05-15T00:00:00'
    Password     = 'password123'
    Address      = '123 Lê Lợi, Quận 1, TP. Hồ Chí Minh'
}
Write-Output "User1: $($r1.id) | $($r1.fullName) | $($r1.email)"

$a1 = Post-Json '/api/accounts' @{ UserId = $r1.id; Currency = 'VND' }
Write-Output "  Acc1: $($a1.accountNumber) | $($a1.balance) | $($a1.currency)"
$a2 = Post-Json '/api/accounts' @{ UserId = $r1.id; Currency = 'VND' }
Write-Output "  Acc2: $($a2.accountNumber) | $($a2.balance) | $($a2.currency)"

# ===== User 2: Trần Thị B =====
$r2 = Post-Json '/api/users' @{
    FullName     = 'Trần Thị B'
    Email        = 'tranthib@gmail.com'
    Phone        = '0909123456'
    IdentityCard = '079302045678'
    DateOfBirth  = '1995-11-20T00:00:00'
    Password     = 'password123'
    Address      = '45 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh'
}
Write-Output "User2: $($r2.id) | $($r2.fullName) | $($r2.email)"

$a3 = Post-Json '/api/accounts' @{ UserId = $r2.id; Currency = 'VND' }
Write-Output "  Acc3: $($a3.accountNumber) | $($a3.balance) | $($a3.currency)"

Write-Output "=== SEED DONE ==="

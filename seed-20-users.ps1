# Seed 20 users mới vào Supabase qua backend API
$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5081'
$headers = @{ 'Content-Type' = 'application/json' }

function Post-Json($path, $obj) {
    $body = $obj | ConvertTo-Json -Compress
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($body)
    return Invoke-RestMethod -Uri "$base$path" -Method Post -ContentType "application/json; charset=utf-8" -Body $bytes
}

$users = @(
    @{ FullName = 'Lê Văn Cường';    Email = 'lecuong@gmail.com';    Phone = '0901111111'; IdentityCard = '079301011101'; DateOfBirth = '1991-03-12T00:00:00'; Address = '12 Nguyễn Trãi, Q1, TP.HCM' },
    @{ FullName = 'Phạm Thị Dung';   Email = 'phamdung@gmail.com';   Phone = '0902222222'; IdentityCard = '079301011102'; DateOfBirth = '1994-07-25T00:00:00'; Address = '23 Lê Thánh Tôn, Q1, TP.HCM' },
    @{ FullName = 'Hoàng Văn Em';    Email = 'hoangem@gmail.com';    Phone = '0903333333'; IdentityCard = '079301011103'; DateOfBirth = '1990-01-08T00:00:00'; Address = '34 Hai Bà Trưng, Q3, TP.HCM' },
    @{ FullName = 'Võ Thị Phương';   Email = 'vophuong@gmail.com';   Phone = '0904444444'; IdentityCard = '079301011104'; DateOfBirth = '1996-11-19T00:00:00'; Address = '45 Điện Biên Phủ, Q3, TP.HCM' },
    @{ FullName = 'Đặng Văn Giang';  Email = 'danggiang@gmail.com';  Phone = '0905555555'; IdentityCard = '079301011105'; DateOfBirth = '1988-09-02T00:00:00'; Address = '56 Võ Văn Tần, Q3, TP.HCM' },
    @{ FullName = 'Bùi Thị Hoa';     Email = 'buihoa@gmail.com';     Phone = '0906666666'; IdentityCard = '079301011106'; DateOfBirth = '1993-05-30T00:00:00'; Address = '67 Nguyễn Đình Chiểu, Q3, TP.HCM' },
    @{ FullName = 'Đỗ Văn Hùng';     Email = 'dohung@gmail.com';     Phone = '0907777777'; IdentityCard = '079301011107'; DateOfBirth = '1987-12-14T00:00:00'; Address = '78 Cao Thắng, Q10, TP.HCM' },
    @{ FullName = 'Hồ Thị Hương';    Email = 'hohuong@gmail.com';    Phone = '0908888888'; IdentityCard = '079301011108'; DateOfBirth = '1995-02-21T00:00:00'; Address = '89 Lý Thường Kiệt, Q10, TP.HCM' },
    @{ FullName = 'Ngô Văn Khánh';   Email = 'ngokhanh@gmail.com';   Phone = '0909999999'; IdentityCard = '079301011109'; DateOfBirth = '1992-08-17T00:00:00'; Address = '90 Cách Mạng T8, Q10, TP.HCM' },
    @{ FullName = 'Dương Thị Lan';   Email = 'duonglan@gmail.com';   Phone = '0910000000'; IdentityCard = '079301011110'; DateOfBirth = '1997-04-03T00:00:00'; Address = '101 Ngô Gia Tự, Q10, TP.HCM' },
    @{ FullName = 'Lý Văn Long';     Email = 'lylong@gmail.com';     Phone = '0911111111'; IdentityCard = '079301011111'; DateOfBirth = '1989-06-28T00:00:00'; Address = '112 Trần Hưng Đạo, Q5, TP.HCM' },
    @{ FullName = 'Trịnh Thị Mai';   Email = 'trinhmai@gmail.com';   Phone = '0912222222'; IdentityCard = '079301011112'; DateOfBirth = '1994-10-09T00:00:00'; Address = '123 An Dương Vương, Q5, TP.HCM' },
    @{ FullName = 'Phan Văn Minh';   Email = 'phanminh@gmail.com';   Phone = '0913333333'; IdentityCard = '079301011113'; DateOfBirth = '1991-01-23T00:00:00'; Address = '134 Hùng Vương, Q5, TP.HCM' },
    @{ FullName = 'Vũ Thị Ngọc';     Email = 'vungoc@gmail.com';     Phone = '0914444444'; IdentityCard = '079301011114'; DateOfBirth = '1998-09-11T00:00:00'; Address = '145 Trần Phú, Q5, TP.HCM' },
    @{ FullName = 'Đoàn Văn Phúc';   Email = 'doanphuc@gmail.com';   Phone = '0915555555'; IdentityCard = '079301011115'; DateOfBirth = '1990-03-05T00:00:00'; Address = '156 Nguyễn Văn Cừ, Q1, TP.HCM' },
    @{ FullName = 'Tạ Thị Quỳnh';    Email = 'taquynh@gmail.com';    Phone = '0916666666'; IdentityCard = '079301011116'; DateOfBirth = '1996-12-01T00:00:00'; Address = '167 Pasteur, Q3, TP.HCM' },
    @{ FullName = 'Lương Văn Sơn';   Email = 'luongson@gmail.com';   Phone = '0917777777'; IdentityCard = '079301011117'; DateOfBirth = '1988-07-16T00:00:00'; Address = '178 Nam Kỳ Khởi Nghĩa, Q3, TP.HCM' },
    @{ FullName = 'Cao Thị Thu';     Email = 'caothu@gmail.com';     Phone = '0918888888'; IdentityCard = '079301011118'; DateOfBirth = '1993-05-07T00:00:00'; Address = '189 Lê Lai, Q1, TP.HCM' },
    @{ FullName = 'Quách Văn Tuấn';  Email = 'quachtuan@gmail.com';  Phone = '0919999999'; IdentityCard = '079301011119'; DateOfBirth = '1992-11-26T00:00:00'; Address = '200 Cống Quỳnh, Q1, TP.HCM' },
    @{ FullName = 'La Thị Vân';      Email = 'lavan@gmail.com';      Phone = '0920000000'; IdentityCard = '079301011120'; DateOfBirth = '1995-02-14T00:00:00'; Address = '211 Nguyễn Thị Minh Khai, Q1, TP.HCM' }
)

$count = 0
foreach ($u in $users) {
    $userBody = @{
        FullName     = $u.FullName
        Email        = $u.Email
        Phone        = $u.Phone
        IdentityCard = $u.IdentityCard
        DateOfBirth  = $u.DateOfBirth
        Password     = 'password123'
        Address      = $u.Address
    }
    $r = Post-Json '/api/users' $userBody
    $count++
    Write-Output "OK [$count] $($r.id) | $($r.fullName) | $($r.email)"
}

Write-Output "=== SEEDED $count USERS ==="

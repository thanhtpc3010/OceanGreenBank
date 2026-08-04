BẢN KẾ HOẠCH & THIẾT KẾ HỆ THỐNG TỔNG THỂ (MASTER TECHNICAL SPECIFICATION)
Dự án: Smart Online Banking & Financial Management Platform

Vai trò: Senior Software Architect / Technical Lead

Tech Stack:

Backend: .NET 9 (Clean Architecture, CQRS with MediatR, Entity Framework Core, Quartz.NET, System.Threading.Channels).

Frontend: Angular (Container / Presentational Pattern, Shell Layout, RxJS, PrimeNG System Design).

Database & Tooling: SQL Server / PostgreSQL (Quản trị & tối ưu qua DBeaver).

I. KIẾN TRÚC HỆ THỐNG CHỦ ĐẠO (SYSTEM ARCHITECTURE)
Hệ thống được thiết kế theo mô hình Distributed Monolith / Clean Architecture hướng Event-Driven nội bộ (In-Memory Async Queue), đảm bảo khả năng mở rộng (Scalability), tính toàn vẹn dữ liệu tài chính (ACID) và hiệu năng phản hồi cao.

┌───────────────────────────────────────────────────────────────────────────────────┐
│                               ANGULAR SPA (CLIENT)                                │
│        [Shell Layout] ──► [Container Components] ──► [Presentational UI]          │
└─────────────────────────────────────────┬─────────────────────────────────────────┘
                                          │ HTTPS / REST API / JSON
                                          ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│                               .NET 9 API GATEWAY                                  │
│                 [Authentication & Captcha Middleware / CORS / Rate-Limit]         │
└─────────────────────────────────────────┬─────────────────────────────────────────┘
                                          │
                                          ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│                             APPLICATION LAYER (CQRS)                              │
│         ┌──────────────────────────────┐     ┌──────────────────────────────┐     │
│         │      Commands (Write)        │     │       Queries (Read)         │     │
│         └──────────────┬───────────────┘     └──────────────┬───────────────┘     │
└────────────────────────┼────────────────────────────────────┼─────────────────────┘
                         │                                    │
                         ▼                                    ▼
┌────────────────────────────────────────┐   ┌──────────────────────────────────────┐
│        INFRASTRUCTURE LAYER            │   │            DOMAIN LAYER              │
│  ├── EF Core (DB Transactions / ACID)  │   │  ├── Entities & Enums                │
│  ├── Quartz.NET (Cron Jobs / Interest) │   │  ├── Domain Events                   │
│  ├── System.Threading.Channels (Queue) │   │  └── Value Objects                   │
│  ├── Smtp/SMS Notification Worker      │   └──────────────────────────────────────┘
│  └── AI Engine (Gemini / RAG Integration)  │
└────────────────────────┬───────────────┘
                         │
                         ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│                           DATABASE (SQL SERVER / DBEAVER)                         │
└───────────────────────────────────────────────────────────────────────────────────┘
II. THIẾT KẾ CƠ SỞ DỮ LIỆU CHUYÊN SÂU (DATABASE SCHEMA - DBEAVER)
Cơ sở dữ liệu được thiết kế đạt chuẩn 3NF, cài đặt Index hợp lý tại các trường tìm kiếm/truy vấn giao dịch cao.

┌──────────────┐       1:N       ┌──────────────────┐       1:N       ┌──────────────────┐
│    Users     ├─────────────────►     Accounts     ├─────────────────►   Transactions   │
└──────┬───────┘                 └────────┬─────────┘                 └──────────────────┘
       │                                  │
       │ 1:N                              │ 1:1
       ▼                                  ▼
┌──────────────┐                 ┌──────────────────┐
│SavingsAccounts                 │AutoEarningSubs   │
└──────────────┘                 └────────┬─────────┘
                                          │ 1:N
                                          ▼
                                 ┌──────────────────┐
                                 │DailyInterestLogs │
                                 └──────────────────┘
# OceanGreenBank — SmartBank

**Smart Online Banking & Financial Management Platform**

A simulated digital banking system covering full business operations: secure login, account management, internal & interbank transfers, e-wallet top-ups, term savings, automated yield generation (AutoEarn), personal finance analytics (PFM), and an AI assistant.

---

## Table of Contents

- [1. Technologies](#1-technologies)
- [2. Project Structure](#2-project-structure)
- [3. System Requirements](#3-system-requirements)
- [4. Installation & Running](#4-installation--running)
- [5. Demo Accounts](#5-demo-accounts)
- [6. Feature Usage Guides](#6-feature-usage-guides)
- [7. System Architecture](#7-system-architecture)

---

## 1. Technologies

| Layer | Technology |
|------|-----------|
| **Frontend** | Angular 21, PrimeNG (UI Component Library), Tailwind CSS v4, RxJS |
| **Backend** | .NET 10 (ASP.NET Core Web API), Clean Architecture, CQRS (MediatR) |
| **Database** | PostgreSQL (Entity Framework Core — Npgsql) |
| **Scheduled jobs** | ASP.NET Core BackgroundService (AutoEarn automated yield) |
| **AI** | Google Gemini (chat assistant, combined with Knowledge Base RAG) |
| **Security** | JWT Bearer, BCrypt password hashing, RBAC authorization |

---

## 2. Project Structure

```
OceanGreenBank/
├── Document/
│   └── OceanGreenBank.md          # Overall technical documentation (spec)
├── ProjectApp/                    # Angular frontend
│   └── src/app/
│       ├── core/                  # Auth guard/service, API services
│       ├── layout/                # Shell layout: header, sidebar, floating AI...
│       └── pages/                 # Screens (login, dashboard, transfer...)
└── ProjectService/                # .NET 10 backend (Clean Architecture)
    ├── ProjectService.Api/        # API layer (Controllers, Middleware, Services)
    ├── ProjectService.Application/# Application layer (CQRS: Commands/Queries/DTOs)
    ├── ProjectService.Domain/     # Domain layer (Entity, Enum, Exceptions)
    └── ProjectService.Infrastructure/ # Infrastructure layer (EF Core, BackgroundService, Repositories)
```

---

## 3. System Requirements

- **.NET SDK 10** (download at [dotnet.microsoft.com](https://dotnet.microsoft.com))
- **Node.js 18+** and **npm** (download at [nodejs.org](https://nodejs.org))
- A modern browser (Chrome, Edge, Firefox...)

---

## 4. Installation & Running

### 4.1. Backend (API)

```powershell
# Build
dotnet build ProjectService/OceanGreenBank.slnx

# Update the database (run EF migrations)
dotnet ef database update -c ApplicationWriteDbContext --project ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj --startup-project ProjectService/ProjectService.Api/ProjectService.Api.csproj

# Run the API at http://localhost:5081
dotnet run --project ProjectService/ProjectService.Api --launch-profile http
```

The API listens at: **http://localhost:5081**

> The database connection string is in `ProjectService/ProjectService.Api/appsettings.json` (the `ConnectionStrings` section). It connects to PostgreSQL (Supabase) by default.
>
> **AI assistant (Gemini):** set the API key in `ProjectService/ProjectService.Api/appsettings.json` (the `Gemini.ApiKey` section) to enable the chatbot. If left empty, the AI bot shows an "not configured" status.

### 4.2. Frontend (Angular)

```powershell
# Install dependencies
npm --prefix ProjectApp install

# Run the dev server at http://localhost:4200
npm --prefix ProjectApp start -- --host 0.0.0.0 --port 4200
```

Open your browser at: **http://localhost:4200**

> ⚠️ **Note:** You must use `npm --prefix ProjectApp ...` when running from the repo root. Do not run `npm start` directly from the root folder.

---

## 5. Demo Accounts

| Role | Email | Password | Notes |
|---------|-------|----------|---------|
| **User** | `nguyenvana@gmail.com` | `password123` | Nguyễn Văn A — has sample accounts & transactions |
| **User** | `test@oceangreenbank.vn` | `Test@123456` | Test account |
| **Administrator** | `admin@smartbank.vn` | `password123` | Full admin access (RBAC) |

You can also **register** a new account directly on the Login screen.

---

## 6. Feature Usage Guides

### 6.1. Login / Register (`/login`)

**Purpose:** Authenticate users and issue a JWT token for system access.

- **Login:** Enter email + password → the system authenticates and returns a token + roles + permissions.
- **Register:** Switch to the "Register" tab, fill in full name, email, phone, password (confirm twice) and check "I'm not a robot".
- If the account is locked or the password is wrong, the system shows a clear error message.

---

### 6.2. Dashboard — Overview (`/dashboard`)

**Purpose:** The main screen after login, summarizing the user's financial situation.

Blocks include:
- **CASA card (payment account):** shows account number and available balance.
- **AutoEarn card:** annual interest rate, interest earned this month, enrolled principal.
- **AI PFM — income & spending stats:** total income, total expense, balance, spending breakdown by category (donut chart) and 6-month cash flow (bar chart).
- **Recent transactions:** the latest transactions list.

---

### 6.3. Account (`/account`)

**Purpose:** Manage personal information and bank accounts.

- **View profile:** personal info (full name, email, phone, ID card, date of birth...).
- **Edit profile** (`/account/edit`): update full name, phone, address.
- **Change transaction password** (`/account/password`): set/change the 6-digit transaction PIN (second-layer protection for transfers).
- **Account list:** view and add new accounts (payment / savings).

---

### 6.4. Transfer (`/transfer`)

**Purpose:** Transfer between internal accounts or to another bank.

- **Internal transfer (within SmartBank):** choose source account, enter receiver account number → lookup to confirm the owner → enter amount, note, spending category → confirm. **Free, instant.**
- **Interbank transfer:** enter receiver name, account number, bank code (BIN) → enter amount → confirm. **Fee 5,000 VND/transaction.**
- Requires the **transaction PIN** (set in section 6.3) to confirm.

---

### 6.5. Top-up (`/deposit`)

**Purpose:** Top up the account via e-wallets (simulated).

- Choose **MoMo** or **ZaloPay**, choose the receiving account, enter the amount.
- The system creates a top-up order and redirects to the mock wallet page (`/wallet-pay/:id`) to confirm payment.
- After confirmation, the money is credited to the account immediately.

---

### 6.6. Transactions (`/transactions`)

**Purpose:** View the full transaction history and statement.

- Filter by account, transaction type (in/out), status (pending/success/failed).
- Cancel a pending transaction (if any).

---

### 6.7. Savings (`/savings`)

**Purpose:** Open term savings and create recurring savings plans.

- **Open a savings book:** choose term (1/3/6/12 months); the interest rate updates automatically by term.
- **Recurring savings:** create a plan that auto-debits from a source account into a savings account on a cycle (daily / weekly / monthly).
- **Deposit now** a cycle or **cancel** the plan at any time.
- Rule: only withdraw at **maturity** to earn interest; early withdrawal forfeits all interest of that cycle.

---

### 6.8. AutoEarn — Automated Yield

**Purpose:** Automatically credit interest daily to enrolled accounts.

- Formula: `principal × annual rate % ÷ 365`.
- Users see the month's accumulated interest on the Dashboard.
- **Administrators** configure on/off, interest rate, auto-run time, and enroll accounts + principal (see section 6.10.2).

---

### 6.9. AI Assistant — PFM AI Bot

**Purpose:** Chat with a virtual assistant (Google Gemini) about banking features and your own data.

- Click the **"PFM AI Bot"** button in the bottom-right corner to open the chat panel.
- The bot can answer about: account balance, recent transactions, savings interest, AutoEarn, how to use each feature...
- The bot combines the **Knowledge Base** (section 6.10.3) to answer according to internal documentation.

> The bot needs the Gemini API key configured in `appsettings.json` (the `Gemini.ApiKey` section) to work.

---

### 6.10. Administration — ADMIN accounts only

#### 6.10.1. User Management (`/admin/users`)

**Purpose:** Manage users and RBAC permissions.

- View, search, create, lock/unlock, delete users.
- View each user's roles & permissions.

#### 6.10.2. AutoEarn Configuration (`/admin/auto-earn`)

**Purpose:** Manage the automated yield feature.

- Enable/disable AutoEarn, adjust interest rate and daily run time.
- Enroll accounts and enter principal.
- View the AutoEarn log.
- Run the job immediately (Run now).

#### 6.10.3. Train AI — Knowledge Base (`/admin/knowledge`)

**Purpose:** Manage the knowledge base to "train" the AI assistant.

- Add / edit / delete / enable-disable knowledge entries (keywords, title, content).
- The AI bot automatically matches questions against the knowledge base and answers accordingly.

---

### 6.11. Donation — Vietnam Fatherland Front (`/donate`)

**Purpose:** Donate to official Vietnamese funds (Central Relief Mobilization Committee, Fund for the Poor, Disaster Prevention Fund, Red Cross...).

- Select the receiving fund and beneficiary bank (each fund has pre-configured accounts).
- Enter the amount and note → confirm with the transaction PIN.
- Donations are **free of charge** and fully recorded in the transaction history.

### 6.12. Translations / Multi-language (`/admin/translations`)

**Purpose:** Manage the app's language content (Vietnamese / English) — translations are stored in the database.

- View, search and filter all translation keys by language.
- Add / edit / delete translations (takes effect immediately, no rebuild).
- Users switch language via the flag button 🇻🇳/🇬🇧 in the header.

---

## 7. System Architecture

The system follows **Clean Architecture + CQRS**, with a clear 4-layer separation:

| Layer | Responsibility |
|------|-------------|
| **Api** | HTTP handling, JWT authentication, middleware, CORS |
| **Application** | CQRS business logic: Commands (write) / Queries (read), DTOs |
| **Domain** | Entities, Enums, Value Objects, Domain Events, Exceptions |
| **Infrastructure** | EF Core (Read/Write DbContext), Repository, BackgroundService, Services |

**Dependency rule:** `Api → Application + Infrastructure`; `Infrastructure → Application → Domain` (no reverse dependencies).

The database is split into a **Read DbContext** (NoTracking, for queries) and a **Write DbContext** (tracking, for writes) to optimize performance.

---

## Contact & Support

- **Hotline (simulated):** 1900 0000 (24/7)
- **Repository:** https://github.com/thanhtpc3010/OceanGreenBank

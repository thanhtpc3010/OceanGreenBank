import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { AuthService } from '../auth/auth.service';

/** Hồ sơ người dùng (frontend view model). */
export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  identityCard: string;
  dateOfBirth: string;
  gender: string;
  address: string;
  isActive: boolean;
  memberSince: string;
}

/** Tài khoản ngân hàng (frontend view model). */
export interface BankAccount {
  id: string;
  accountNumber: string;
  type: number; // 0 = CASA (thông thường), 1 = SAVINGS (tiết kiệm)
  typeLabel: string;
  balance: number;
  currency: string;
  isActive: boolean;
  savingsTermMonths: number | null;
  interestRate: number | null;
  savingsStartDate: string | null;
  savingsMaturityDate: string | null;
  openedDate: string;
}

/* ============ Backend DTO ============ */
interface UserDto {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  identityCard: string;
  dateOfBirth: string;
  address: string | null;
  isActive: boolean;
  createdDate: string;
}

interface AccountDto {
  id: string;
  userId: string;
  accountNumber: string;
  balance: number;
  currency: string;
  isActive: boolean;
  type: number;
  savingsTermMonths: number | null;
  interestRate: number | null;
  savingsStartDate: string | null;
  savingsMaturityDate: string | null;
  createdDate: string;
}

interface UserPermissionsDto {
  userId: string;
  fullName: string;
  email: string;
  roles: { id: string; roleName: string; code: string | null; description: string | null }[];
  permissionCodes: string[];
}

/**
 * Tầng dữ liệu người dùng — gọi backend API thật (đã kết nối Supabase).
 *
 * API base: http://localhost:5081/api
 *   - getProfile()     → GET  /users (tìm theo email user đang đăng nhập)
 *   - getAccounts()    → GET  /accounts/by-user/{userId}
 *   - updateProfile()  → PUT  /users/{id}
 *   - addAccount()     → POST /accounts
 *   - deleteAccount()  → DELETE /accounts/{id}
 *   - deleteUser()     → DELETE /users/{id}
 */
@Injectable({ providedIn: 'root' })
export class UserService {
  /** Profile hiện tại (signal để UI tự cập nhật). */
  readonly profile = signal<UserProfile | null>(null);
  readonly accounts = signal<BankAccount[]>([]);
  readonly loading = signal(false);

  private readonly apiUrl = 'http://localhost:5081/api';
  private readonly GENDER_KEY = 'smartbank.gender';

  constructor(
    private readonly http: HttpClient,
    private readonly auth: AuthService,
  ) {}

  /* ================= ĐỌC ================= */

  /** Lấy hồ sơ người dùng từ API, tìm theo email đang đăng nhập. */
  async getProfile(): Promise<UserProfile> {
    const email = this.auth.currentUser()?.email;
    if (!email) {
      throw new Error('Vui lòng đăng nhập trước.');
    }

    const users = await firstValueFrom(this.http.get<UserDto[]>(`${this.apiUrl}/users`));
    const user = users.find((u) => u.email.toLowerCase() === email.toLowerCase());
    if (!user) {
      throw new Error('Không tìm thấy người dùng với email đã đăng nhập.');
    }

    const profile = this.toProfile(user);
    this.profile.set(profile);
    this.auth.setCurrentUser(profile.fullName, profile.email);
    // Tải vai trò & quyền (RBAC) cho user đang đăng nhập.
    await this.loadPermissions(user.id);
    return profile;
  }

  /** Nạp vai trò & quyền của user vào AuthService — GET /api/users/{id}/permissions. */
  async loadPermissions(userId: string): Promise<void> {
    try {
      const data = await firstValueFrom(
        this.http.get<UserPermissionsDto>(`${this.apiUrl}/users/${userId}/permissions`),
      );
      this.auth.setAuthorization(
        data.roles.map((r) => r.code).filter((c): c is string => !!c),
        data.permissionCodes,
      );
    } catch {
      // Không có quyền → bỏ qua
      this.auth.setAuthorization([], []);
    }
  }

  /** Lấy danh sách tài khoản của user từ API. */
  async getAccounts(): Promise<BankAccount[]> {
    const profile = await this.getProfile();
    const list = await firstValueFrom(
      this.http.get<AccountDto[]>(`${this.apiUrl}/accounts/by-user/${profile.id}`),
    );
    const accounts = list.map((a) => this.toAccount(a));
    this.accounts.set(accounts);
    return accounts;
  }

  /** Load toàn bộ dữ liệu hồ sơ + tài khoản. */
  async loadAll(): Promise<void> {
    this.loading.set(true);
    try {
      await this.getProfile();
      await this.getAccounts();
    } finally {
      this.loading.set(false);
    }
  }

  /** Tìm user theo email (cho màn hình đăng nhập) — trả null nếu không tồn tại. */
  async getUserByEmail(
    email: string,
  ): Promise<{ id: string; fullName: string; email: string; isActive: boolean } | null> {
    const users = await firstValueFrom(this.http.get<UserDto[]>(`${this.apiUrl}/users`));
    const user = users.find((u) => u.email.toLowerCase() === email.trim().toLowerCase());
    return user
      ? { id: user.id, fullName: user.fullName, email: user.email, isActive: user.isActive }
      : null;
  }

  /* ================= SỬA (UPDATE) ================= */

  /** Cập nhật hồ sơ — PUT /api/users/{id}. */
  async updateProfile(patch: Partial<UserProfile>): Promise<UserProfile> {
    const current = await this.getProfile();

    // Backend UpdateUserRequest chỉ nhận: FullName, Phone, Address, IsActive
    const updated = await firstValueFrom(
      this.http.put<UserDto>(`${this.apiUrl}/users/${current.id}`, {
        fullName: patch.fullName ?? current.fullName,
        phone: patch.phone ?? current.phone,
        address: patch.address ?? current.address,
        isActive: patch.isActive ?? current.isActive,
      }),
    );

    // Gender là field local-only (backend chưa có) → lưu localStorage.
    if (patch.gender) {
      localStorage.setItem(this.GENDER_KEY, patch.gender);
    }

    const profile = this.toProfile(updated);
    this.profile.set(profile);
    this.auth.setCurrentUser(profile.fullName, profile.email);
    return profile;
  }

  /* ================= ĐỔI MẬT KHẨU ================= */

  /**
   * Đổi mật khẩu.
   * ⚠️ Backend chưa có endpoint → tạm dùng mock (localStorage) với mật khẩu mặc định.
   * Khi có endpoint POST /api/users/{id}/change-password, thay phần thân bằng API call.
   */
  async changePassword(current: string, next: string): Promise<void> {
    const saved = localStorage.getItem('smartbank.password') ?? 'password123';
    if (current !== saved) {
      throw new Error('Mật khẩu hiện tại không đúng.');
    }
    localStorage.setItem('smartbank.password', next);
  }

  /* ================= THÊM / XÓA TÀI KHOẢN ================= */

  /** Thêm tài khoản mới — POST /api/accounts. */
  async addAccount(): Promise<BankAccount> {
    const profile = await this.getProfile();
    const created = await firstValueFrom(
      this.http.post<AccountDto>(`${this.apiUrl}/accounts`, {
        userId: profile.id,
        currency: 'VND',
      }),
    );
    const account = this.toAccount(created);
    this.accounts.update((list) => [...list, account]);
    return account;
  }

  /** Mở sổ tiết kiệm mới — POST /api/accounts (type=1, kỳ hạn + lãi suất). */
  async addSavingsAccount(savingsTermMonths: number, interestRate: number): Promise<BankAccount> {
    const profile = await this.getProfile();
    const created = await firstValueFrom(
      this.http.post<AccountDto>(`${this.apiUrl}/accounts`, {
        userId: profile.id,
        currency: 'VND',
        type: 1,
        savingsTermMonths,
        interestRate,
        savingsStartDate: new Date().toISOString(),
      }),
    );
    const account = this.toAccount(created);
    this.accounts.update((list) => [...list, account]);
    return account;
  }

  /** Xóa tài khoản — DELETE /api/accounts/{id}. */
  async deleteAccount(id: string): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.apiUrl}/accounts/${id}`));
    this.accounts.update((list) => list.filter((a) => a.id !== id));
  }

  /* ================= XÓA USER ================= */

  /** Xóa tài khoản người dùng — DELETE /api/users/{id}. */
  async deleteUser(): Promise<void> {
    const profile = await this.getProfile();
    await firstValueFrom(this.http.delete(`${this.apiUrl}/users/${profile.id}`));
    localStorage.removeItem(this.GENDER_KEY);
    this.profile.set(null);
    this.accounts.set([]);
    this.auth.logout();
  }

  /* ================= ADMIN: QUẢN LÝ USER ================= */

  /** Danh sách tất cả user — GET /api/users. */
  async getAllUsers(): Promise<UserDto[]> {
    return firstValueFrom(this.http.get<UserDto[]>(`${this.apiUrl}/users`));
  }

  /** Tạo user mới (admin) — POST /api/users. */
  async createUserByAdmin(data: {
    fullName: string;
    email: string;
    phone: string;
    identityCard: string;
    dateOfBirth: string;
    password: string;
    address?: string;
  }): Promise<UserDto> {
    return firstValueFrom(
      this.http.post<UserDto>(`${this.apiUrl}/users`, {
        fullName: data.fullName,
        email: data.email,
        phone: data.phone,
        identityCard: data.identityCard,
        dateOfBirth: data.dateOfBirth,
        password: data.password,
        address: data.address ?? null,
      }),
    );
  }

  /** Cập nhật user (admin) — PUT /api/users/{id}. */
  async updateUserByAdmin(
    id: string,
    patch: { fullName?: string; phone?: string; address?: string; isActive?: boolean },
  ): Promise<UserDto> {
    return firstValueFrom(
      this.http.put<UserDto>(`${this.apiUrl}/users/${id}`, {
        fullName: patch.fullName ?? null,
        phone: patch.phone ?? null,
        address: patch.address ?? null,
        isActive: patch.isActive ?? null,
      }),
    );
  }

  /** Xóa user bất kỳ (admin) — DELETE /api/users/{id}. */
  async deleteUserById(id: string): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.apiUrl}/users/${id}`));
  }

  /** Vai trò & quyền của 1 user (admin) — GET /api/users/{id}/permissions. */
  async getUserPermissions(userId: string): Promise<UserPermissionsDto> {
    return firstValueFrom(
      this.http.get<UserPermissionsDto>(`${this.apiUrl}/users/${userId}/permissions`),
    );
  }

  /* ================= MAPPER ================= */

  private toProfile(dto: UserDto): UserProfile {
    const gender = localStorage.getItem(this.GENDER_KEY) ?? 'Nam';
    return {
      id: dto.id,
      fullName: dto.fullName,
      email: dto.email,
      phone: dto.phone,
      identityCard: dto.identityCard,
      dateOfBirth: dto.dateOfBirth,
      gender,
      address: dto.address ?? '',
      isActive: dto.isActive,
      memberSince: dto.createdDate,
    };
  }

  private toAccount(dto: AccountDto): BankAccount {
    const isSavings = dto.type === 1;
    return {
      id: dto.id,
      accountNumber: dto.accountNumber,
      type: dto.type,
      typeLabel: isSavings ? 'Tiết kiệm (SAVINGS)' : 'Thanh toán (CASA)',
      balance: dto.balance,
      currency: dto.currency,
      isActive: dto.isActive,
      savingsTermMonths: dto.savingsTermMonths,
      interestRate: dto.interestRate,
      savingsStartDate: dto.savingsStartDate,
      savingsMaturityDate: dto.savingsMaturityDate,
      openedDate: dto.createdDate,
    };
  }
}

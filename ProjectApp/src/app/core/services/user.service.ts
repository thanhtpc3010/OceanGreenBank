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
  type: 'CASA' | 'SAVINGS';
  typeLabel: string;
  balance: number;
  currency: string;
  isActive: boolean;
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
  createdDate: string;
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
    return profile;
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
    return {
      id: dto.id,
      accountNumber: dto.accountNumber,
      type: 'CASA',
      typeLabel: 'Thanh toán (CASA)',
      balance: dto.balance,
      currency: dto.currency,
      isActive: dto.isActive,
      openedDate: dto.createdDate,
    };
  }
}

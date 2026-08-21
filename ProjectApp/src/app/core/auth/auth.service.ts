import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

interface StoredSession {
  token: string;
  userId: string;
  fullName: string;
  email: string;
  roles?: string[];
  permissions?: string[];
}

interface AuthResponse {
  token: string;
  expiresAt: string;
  user: {
    id: string;
    fullName: string;
    email: string;
    roles: string[];
    permissions: string[];
  };
}

export interface RegisterPayload {
  fullName: string;
  email: string;
  phone: string;
  identityCard: string;
  dateOfBirth: string;
  password: string;
  address?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly isLoggedIn = signal(false);
  readonly currentUser = signal<{ id: string; fullName: string; email: string } | null>(null);
  readonly token = signal<string | null>(null);
  readonly roles = signal<string[]>([]);
  readonly permissions = signal<string[]>([]);

  private readonly apiUrl = 'http://localhost:5081/api';
  private readonly SESSION_KEY = 'smartbank.session';

  constructor(private readonly http: HttpClient) {
    this.restoreSession();
  }

  /** Lưu vai trò & quyền (RBAC). */
  setAuthorization(roles: string[], permissions: string[]): void {
    this.roles.set(roles);
    this.permissions.set(permissions);
    const raw = localStorage.getItem(this.SESSION_KEY);
    if (raw) {
      const session = JSON.parse(raw) as StoredSession;
      localStorage.setItem(
        this.SESSION_KEY,
        JSON.stringify({ ...session, roles, permissions } satisfies StoredSession),
      );
    }
  }

  /** Kiểm tra quyền — ADMIN.ALL cho phép tất cả. */
  hasPermission(code: string): boolean {
    const perms = this.permissions();
    return perms.includes('ADMIN.ALL') || perms.includes(code);
  }

  /** Kiểm tra vai trò. */
  hasRole(code: string): boolean {
    return this.roles().includes(code);
  }

  /** Đăng nhập thật — POST /api/auth/login → JWT + user (roles/permissions). */
  async login(email: string, password: string): Promise<void> {
    const res = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, { email, password }),
    );
    this.applySession(res);
  }

  /** Đăng ký thật — POST /api/auth/register → JWT + user. */
  async register(payload: RegisterPayload): Promise<void> {
    const res = await firstValueFrom(
      this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, payload),
    );
    this.applySession(res);
  }

  /** Khôi phục phiên từ localStorage. */
  private restoreSession(): void {
    try {
      const raw = localStorage.getItem(this.SESSION_KEY);
      if (!raw) return;
      const session = JSON.parse(raw) as StoredSession;
      this.token.set(session.token);
      this.currentUser.set({
        id: session.userId,
        fullName: session.fullName,
        email: session.email,
      });
      this.roles.set(session.roles ?? []);
      this.permissions.set(session.permissions ?? []);
      this.isLoggedIn.set(true);
    } catch {
      localStorage.removeItem(this.SESSION_KEY);
    }
  }

  /** Lưu session sau khi login/register. */
  private applySession(res: AuthResponse): void {
    this.token.set(res.token);
    this.currentUser.set({
      id: res.user.id,
      fullName: res.user.fullName,
      email: res.user.email,
    });
    this.roles.set(res.user.roles ?? []);
    this.permissions.set(res.user.permissions ?? []);
    this.isLoggedIn.set(true);
    localStorage.setItem(
      this.SESSION_KEY,
      JSON.stringify({
        token: res.token,
        userId: res.user.id,
        fullName: res.user.fullName,
        email: res.user.email,
        roles: res.user.roles,
        permissions: res.user.permissions,
      } satisfies StoredSession),
    );
  }

  /** Cập nhật thông tin user hiện tại (sau khi sửa hồ sơ). */
  setCurrentUser(fullName: string, email: string): void {
    const cur = this.currentUser();
    this.currentUser.set({ id: cur?.id ?? '', fullName, email });
    const raw = localStorage.getItem(this.SESSION_KEY);
    if (raw) {
      const session = JSON.parse(raw) as StoredSession;
      localStorage.setItem(
        this.SESSION_KEY,
        JSON.stringify({ ...session, fullName, email } satisfies StoredSession),
      );
    }
  }

  logout(): void {
    this.token.set(null);
    this.currentUser.set(null);
    this.roles.set([]);
    this.permissions.set([]);
    this.isLoggedIn.set(false);
    localStorage.removeItem(this.SESSION_KEY);
  }
}

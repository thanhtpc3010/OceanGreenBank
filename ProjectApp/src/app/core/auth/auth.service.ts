import { Injectable, signal } from '@angular/core';

interface StoredSession {
  token: string;
  fullName: string;
  email: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly isLoggedIn = signal(false);
  readonly currentUser = signal<{ fullName: string; email: string } | null>(null);
  readonly token = signal<string | null>(null);

  private readonly SESSION_KEY = 'smartbank.session';

  constructor() {
    this.restoreSession();
  }

  /** Khôi phục phiên từ localStorage (để refresh trang vẫn giữ đăng nhập). */
  private restoreSession(): void {
    try {
      const raw = localStorage.getItem(this.SESSION_KEY);
      if (!raw) return;
      const session = JSON.parse(raw) as StoredSession;
      this.token.set(session.token);
      this.currentUser.set({ fullName: session.fullName, email: session.email });
      this.isLoggedIn.set(true);
    } catch {
      localStorage.removeItem(this.SESSION_KEY);
    }
  }

  /** Mock đăng nhập thành công, lưu token & user. */
  login(email: string, fullName: string): void {
    // TODO: gọi API backend thật, nhận JWT.
    const token = 'mock-jwt-token-' + Date.now();
    this.token.set(token);
    this.currentUser.set({ fullName, email });
    this.isLoggedIn.set(true);
    localStorage.setItem(
      this.SESSION_KEY,
      JSON.stringify({ token, fullName, email } satisfies StoredSession),
    );
  }

  /** Mock đăng ký thành công (tự động đăng nhập luôn). */
  register(email: string, fullName: string): void {
    this.login(email, fullName);
  }

  /** Cập nhật thông tin user hiện tại (sau khi sửa hồ sơ). */
  setCurrentUser(fullName: string, email: string): void {
    this.currentUser.set({ fullName, email });
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
    this.isLoggedIn.set(false);
    localStorage.removeItem(this.SESSION_KEY);
  }
}

import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  readonly isLoggedIn = signal(false);
  readonly currentUser = signal<{ fullName: string; email: string } | null>(null);
  readonly token = signal<string | null>(null);

  /** Mock đăng nhập thành công, lưu token & user. */
  login(email: string, fullName: string): void {
    // TODO: gọi API backend thật, nhận JWT.
    this.token.set('mock-jwt-token-' + Date.now());
    this.currentUser.set({ fullName, email });
    this.isLoggedIn.set(true);
  }

  /** Mock đăng ký thành công (tự động đăng nhập luôn). */
  register(email: string, fullName: string): void {
    this.login(email, fullName);
  }

  logout(): void {
    this.token.set(null);
    this.currentUser.set(null);
    this.isLoggedIn.set(false);
  }
}

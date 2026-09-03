import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { TabsModule } from 'primeng/tabs';

import { AuthService } from '../../core/auth/auth.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    ButtonModule,
    CheckboxModule,
    InputGroupModule,
    InputGroupAddonModule,
    InputTextModule,
    MessageModule,
    PasswordModule,
    TabsModule,
    TranslatePipe,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly language = inject(LanguageService);

  protected readonly activeTab = signal<'login' | 'register'>('login');
  protected readonly loading = signal(false);
  protected readonly error = signal('');

  /* ---- Login ---- */
  protected readonly loginEmail = signal('');
  protected readonly loginPassword = signal('');

  /* ---- Register ---- */
  protected readonly regFullName = signal('');
  protected readonly regEmail = signal('');
  protected readonly regPhone = signal('');
  protected readonly regPassword = signal('');
  protected readonly regConfirmPassword = signal('');
  protected readonly recaptcha = signal(false);

  protected readonly submitLabel = computed(() => {
    const isLogin = this.activeTab() === 'login';
    if (this.loading()) {
      return this.language.t(isLogin ? 'LOGIN.LOADING' : 'LOGIN.REGISTER_LOADING');
    }
    return this.language.t(isLogin ? 'LOGIN.SUBMIT' : 'LOGIN.TAB_REGISTER');
  });

  constructor(private readonly router: Router, private readonly auth: AuthService) {}

  protected setTab(tab: unknown): void {
    this.activeTab.set(tab === 'register' ? 'register' : 'login');
    this.error.set('');
    this.recaptcha.set(false);
  }

  protected async submit(): Promise<void> {
    if (this.activeTab() === 'login') await this.doLogin();
    else await this.doRegister();
  }

  private async doLogin(): Promise<void> {
    const email = this.loginEmail().trim();
    const password = this.loginPassword();
    if (!email || !password.trim()) {
      this.error.set(this.language.t('LOGIN.ERR_EMPTY'));
      return;
    }
    this.loading.set(true);
    this.error.set('');
    try {
      // Đăng nhập thật: POST /api/auth/login → JWT + roles/permissions
      await this.auth.login(email, password);
      this.router.navigate(['/dashboard']);
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.loading.set(false);
    }
  }

  private async doRegister(): Promise<void> {
    const fullName = this.regFullName().trim();
    const email = this.regEmail().trim();
    if (!fullName || !email || !this.regPassword().trim()) {
      this.error.set(this.language.t('LOGIN.ERR_EMPTY'));
      return;
    }
    if (this.regPassword() !== this.regConfirmPassword()) {
      this.error.set(this.language.t('LOGIN.ERR_PASSWORD_MISMATCH'));
      return;
    }
    if (!this.recaptcha()) {
      this.error.set(this.language.t('LOGIN.ERR_CAPTCHA'));
      return;
    }
    this.loading.set(true);
    this.error.set('');
    try {
      // Đăng ký thật: POST /api/auth/register → JWT + roles/permissions
      await this.auth.register({
        fullName,
        email,
        phone: this.regPhone().trim() || '0900000000',
        identityCard: '',
        dateOfBirth: '2000-01-01',
        password: this.regPassword(),
      });
      this.router.navigate(['/dashboard']);
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.loading.set(false);
    }
  }

  /** Lấy message từ lỗi API (vd: "Tài khoản đã bị khóa", "Email hoặc mật khẩu không đúng"). */
  private extractMessage(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('LOGIN.ERR_GENERIC'));
  }
}

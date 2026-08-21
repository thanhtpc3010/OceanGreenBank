import { Component, signal } from '@angular/core';
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
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
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

  constructor(
    private readonly router: Router,
    private readonly auth: AuthService,
  ) {}

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
    if (!email || !this.loginPassword().trim()) {
      this.error.set('Vui lòng nhập đầy đủ thông tin.');
      return;
    }
    this.loading.set(true);
    this.error.set('');
    // TODO: gọi API backend thật POST /api/auth/login
    await new Promise((r) => setTimeout(r, 800));
    this.auth.login(email, 'Nguyễn Văn A');
    this.loading.set(false);
    this.router.navigate(['/dashboard']);
  }

  private async doRegister(): Promise<void> {
    const email = this.regEmail().trim();
    if (!this.regFullName().trim() || !email || !this.regPassword().trim()) {
      this.error.set('Vui lòng nhập đầy đủ thông tin.');
      return;
    }
    if (this.regPassword() !== this.regConfirmPassword()) {
      this.error.set('Mật khẩu xác nhận không khớp.');
      return;
    }
    if (!this.recaptcha()) {
      this.error.set('Vui lòng xác nhận bạn không phải người máy.');
      return;
    }
    this.loading.set(true);
    this.error.set('');
    // TODO: gọi API backend thật POST /api/auth/register
    await new Promise((r) => setTimeout(r, 1000));
    this.auth.register(email, this.regFullName().trim());
    this.loading.set(false);
    this.router.navigate(['/dashboard']);
  }
}

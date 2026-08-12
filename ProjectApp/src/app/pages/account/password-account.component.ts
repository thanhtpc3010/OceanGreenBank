import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-account-password',
  imports: [FormsModule, RouterLink],
  templateUrl: './password-account.component.html',
  styleUrl: './password-account.component.scss',
})
export class PasswordAccountComponent {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');

  protected readonly current = signal('');
  protected readonly next = signal('');
  protected readonly confirm = signal('');

  protected async save(): Promise<void> {
    this.error.set('');
    this.success.set('');

    // ---- Validation ----
    if (!this.current()) {
      this.error.set('Vui lòng nhập mật khẩu hiện tại.');
      return;
    }
    if (this.next().length < 8) {
      this.error.set('Mật khẩu mới phải có ít nhất 8 ký tự.');
      return;
    }
    if (this.next() === this.current()) {
      this.error.set('Mật khẩu mới phải khác mật khẩu hiện tại.');
      return;
    }
    if (this.next() !== this.confirm()) {
      this.error.set('Mật khẩu xác nhận không khớp.');
      return;
    }

    this.saving.set(true);
    try {
      await this.userService.changePassword(this.current(), this.next());
      this.success.set('Đổi mật khẩu thành công!');
      setTimeout(() => this.router.navigate(['/account']), 900);
    } catch (e) {
      this.error.set(e instanceof Error ? e.message : 'Đổi mật khẩu thất bại.');
    } finally {
      this.saving.set(false);
    }
  }
}

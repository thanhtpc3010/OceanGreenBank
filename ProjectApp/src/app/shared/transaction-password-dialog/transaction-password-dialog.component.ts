import { Component, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TransactionPasswordService } from '../../core/services/transaction-password.service';

/**
 * Popup nhập mật khẩu giao dịch (cấp 2) — dùng chung toàn app.
 * Chỉ cần sửa file này khi muốn thay đổi giao diện / logic nhập mật khẩu.
 */
@Component({
  selector: 'app-transaction-password-dialog',
  imports: [FormsModule, TranslatePipe],
  templateUrl: './transaction-password-dialog.component.html',
  styleUrl: './transaction-password-dialog.component.scss',
})
export class TransactionPasswordDialogComponent {
  private readonly service = inject(TransactionPasswordService);
  private readonly language = inject(LanguageService);

  protected readonly state = this.service.state;
  protected readonly password = signal('');
  protected readonly error = signal('');

  constructor() {
    // Reset input + lỗi mỗi khi popup mở.
    effect(() => {
      if (this.state()) {
        this.password.set('');
        this.error.set('');
      }
    });
  }

  protected cancel(): void {
    this.service.resolve(null);
  }

  protected confirm(): void {
    const pwd = this.password().trim();
    if (!/^\d{6}$/.test(pwd)) {
      this.error.set(this.language.t('PASSWORD.PIN_REQUIRED'));
      return;
    }
    this.service.resolve(pwd);
  }
}

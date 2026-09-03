import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../core/services/user.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-account-password',
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './password-account.component.html',
  styleUrl: './password-account.component.scss',
})
export class PasswordAccountComponent {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);

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
      this.error.set(this.language.t('PASSWORD.ERR_CURRENT_REQUIRED'));
      return;
    }
    if (this.next().length < 8) {
      this.error.set(this.language.t('PASSWORD.ERR_MIN_LENGTH'));
      return;
    }
    if (this.next() === this.current()) {
      this.error.set(this.language.t('PASSWORD.ERR_SAME'));
      return;
    }
    if (this.next() !== this.confirm()) {
      this.error.set(this.language.t('PASSWORD.ERR_MISMATCH'));
      return;
    }

    this.saving.set(true);
    try {
      await this.userService.changePassword(this.current(), this.next());
      this.success.set(this.language.t('PASSWORD.SUCCESS'));
      setTimeout(() => this.router.navigate(['/account']), 900);
    } catch (e) {
      this.error.set(e instanceof Error ? e.message : this.language.t('PASSWORD.ERR_FAILED'));
    } finally {
      this.saving.set(false);
    }
  }

  /* ================= MẬT KHẨU GIAO DỊCH (CẤP 2) ================= */
  protected readonly tpSaving = signal(false);
  protected readonly tpError = signal('');
  protected readonly tpSuccess = signal('');
  protected readonly tpNext = signal('');
  protected readonly tpConfirm = signal('');

  protected async saveTransactionPassword(): Promise<void> {
    this.tpError.set('');
    this.tpSuccess.set('');

    if (!/^\d{6}$/.test(this.tpNext())) {
      this.tpError.set(this.language.t('PASSWORD.PIN_REQUIRED'));
      return;
    }
    if (this.tpNext() !== this.tpConfirm()) {
      this.tpError.set(this.language.t('PASSWORD.ERR_MISMATCH'));
      return;
    }

    this.tpSaving.set(true);
    try {
      await this.userService.setTransactionPassword(this.tpNext());
      this.tpSuccess.set(this.language.t('PASSWORD.TP_SUCCESS'));
      this.tpNext.set('');
      this.tpConfirm.set('');
    } catch (e) {
      const body = (e as { error?: { message?: string } })?.error;
      this.tpError.set(body?.message ?? (e instanceof Error ? e.message : this.language.t('PASSWORD.ERR_DEFAULT')));
    } finally {
      this.tpSaving.set(false);
    }
  }
}

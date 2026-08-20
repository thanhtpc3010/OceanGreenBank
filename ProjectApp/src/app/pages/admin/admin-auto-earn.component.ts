import { DecimalPipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import {
  AutoEarnService,
  AutoEarnLog,
  AutoEarnAccountAdmin,
} from '../../core/services/auto-earn.service';

@Component({
  selector: 'app-admin-auto-earn',
  imports: [FormsModule, DecimalPipe, RouterLink],
  templateUrl: './admin-auto-earn.component.html',
  styleUrl: './admin-auto-earn.component.scss',
})
export class AdminAutoEarnComponent implements OnInit {
  private readonly svc = inject(AutoEarnService);

  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly success = signal('');
  protected readonly saving = signal(false);
  protected readonly running = signal(false);

  /* ---- Form cấu hình ---- */
  protected readonly isActive = signal(true);
  protected readonly interestRate = signal(5);
  protected readonly runTime = signal('00:00');
  protected readonly lastRunAt = signal<string | null>(null);
  protected readonly nextRunAt = signal<string | null>(null);

  /* ---- Tài khoản tham gia + nhật ký ---- */
  protected readonly accounts = signal<AutoEarnAccountAdmin[]>([]);
  protected readonly logs = signal<AutoEarnLog[]>([]);

  async ngOnInit(): Promise<void> {
    await this.reloadAll();
  }

  private async reloadAll(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      const s = await this.svc.getSettings();
      this.isActive.set(s.isActive);
      this.interestRate.set(s.annualInterestRate);
      this.runTime.set(s.runTime);
      this.lastRunAt.set(s.lastRunAt);
      this.nextRunAt.set(s.nextRunAt);
      this.accounts.set(await this.svc.getAccounts());
      this.logs.set(await this.svc.getLogs());
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.loading.set(false);
    }
  }

  /** Lưu cấu hình: bật/tắt + lãi suất + thời gian chạy tự động. */
  protected async saveSettings(): Promise<void> {
    this.saving.set(true);
    this.error.set('');
    this.success.set('');
    try {
      const s = await this.svc.updateSettings({
        isActive: this.isActive(),
        annualInterestRate: this.interestRate(),
        runTime: this.runTime(),
      });
      this.isActive.set(s.isActive);
      this.interestRate.set(s.annualInterestRate);
      this.runTime.set(s.runTime);
      this.lastRunAt.set(s.lastRunAt);
      this.nextRunAt.set(s.nextRunAt);
      this.success.set(
        `Đã lưu cấu hình. Job sẽ chạy tự động mỗi ngày lúc ${s.runTime} (giờ VN).`,
      );
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.saving.set(false);
    }
  }

  /** Chạy job sinh lời ngay (dùng để kiểm tra). */
  protected async runNow(): Promise<void> {
    this.running.set(true);
    this.error.set('');
    this.success.set('');
    try {
      const s = await this.svc.runNow();
      this.lastRunAt.set(s.lastRunAt);
      this.nextRunAt.set(s.nextRunAt);
      this.success.set('Đã chạy job sinh lời. Xem nhật ký bên dưới.');
      this.logs.set(await this.svc.getLogs());
      this.accounts.set(await this.svc.getAccounts());
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.running.set(false);
    }
  }

  /** Bật/tắt đăng ký 1 tài khoản tham gia AutoEarn. */
  protected async toggleAccount(acc: AutoEarnAccountAdmin, enabled: boolean): Promise<void> {
    const principal = enabled ? (acc.principal > 0 ? acc.principal : 10_000_000) : 0;
    try {
      await this.svc.updateAccountEnrollment(acc.accountId, enabled, principal);
      this.accounts.update((list) =>
        list.map((a) =>
          a.accountId === acc.accountId ? { ...a, isEnrolled: enabled, principal } : a,
        ),
      );
      this.success.set(
        `Đã ${enabled ? 'đăng ký' : 'hủy đăng ký'} tài khoản ${acc.accountNumber} tham gia AutoEarn.`,
      );
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  /** Lưu tiền gốc tham gia của 1 tài khoản. */
  protected async savePrincipal(acc: AutoEarnAccountAdmin): Promise<void> {
    try {
      await this.svc.updateAccountEnrollment(acc.accountId, acc.isEnrolled, acc.principal);
      this.success.set(`Đã cập nhật tiền gốc tài khoản ${acc.accountNumber}.`);
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  protected formatDateTime(iso: string | null): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime())
      ? iso
      : d.toLocaleString('vi-VN', { dateStyle: 'short', timeStyle: 'short' });
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : 'Có lỗi xảy ra. Vui lòng thử lại.');
  }
}

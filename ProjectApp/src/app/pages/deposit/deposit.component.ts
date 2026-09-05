import { DecimalPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { PaymentService, PaymentProvider, PaymentDto } from '../../core/services/payment.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { UserService, BankAccount } from '../../core/services/user.service';

@Component({
  selector: 'app-deposit',
  imports: [FormsModule, DecimalPipe, DatePipe, TranslatePipe],
  templateUrl: './deposit.component.html',
  styleUrl: './deposit.component.scss',
})
export class DepositComponent implements OnInit {
  private readonly paymentService = inject(PaymentService);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly language = inject(LanguageService);

  /* ---- Form ---- */
  protected readonly provider = signal<number>(PaymentProvider.Momo);
  protected readonly amount = signal<number | null>(null);
  protected readonly accountId = signal('');
  protected readonly description = signal('');
  protected readonly submitting = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');

  protected readonly accounts = signal<BankAccount[]>([]);
  protected readonly history = signal<PaymentDto[]>([]);
  protected readonly historyLoading = signal(false);

  protected readonly providerOptions = [
    {
      value: PaymentProvider.Momo,
      label: 'MoMo',
      sub: this.language.t('DEPOSIT.EWALLET_MOMO'),
      brand: '#d82d8b',
      short: 'M',
    },
    {
      value: PaymentProvider.ZaloPay,
      label: 'ZaloPay',
      sub: this.language.t('DEPOSIT.EWALLET_ZALOPAY'),
      brand: '#0068ff',
      short: 'Z',
    },
    {
      value: PaymentProvider.Cash,
      label: this.language.t('DEPOSIT.CASH'),
      sub: this.language.t('DEPOSIT.CASH_SUB'),
      brand: '#16a34a',
      short: '₫',
    },
  ];

  protected readonly selectedAccount = computed(
    () => this.accounts().find((a) => a.id === this.accountId()) ?? null,
  );

  async ngOnInit(): Promise<void> {
    await this.refreshAccounts();
    await this.loadHistory();
  }

  /** Nạp danh sách tài khoản CASA đang hoạt động (giữ lựa chọn hiện tại nếu còn). */
  private async refreshAccounts(): Promise<void> {
    const accounts = await this.userService.getAccounts();
    const casas = accounts.filter((a) => a.type === 0 && a.isActive);
    this.accounts.set(casas);
    if (casas.length && !casas.some((a) => a.id === this.accountId())) {
      this.accountId.set(casas[0].id);
    }
  }

  protected async loadHistory(): Promise<void> {
    this.historyLoading.set(true);
    try {
      const profile = await this.userService.getProfile();
      this.history.set(await this.paymentService.getPayments(profile.id));
    } catch {
      this.history.set([]);
    } finally {
      this.historyLoading.set(false);
    }
  }

  /** Tạo đơn nạp tiền. Ví → redirect trang mô phỏng; tiền mặt → vào ngay. */
  protected async createPayment(): Promise<void> {
    this.error.set('');
    this.success.set('');
    const amt = this.amount();
    if (!amt || amt <= 0) {
      this.error.set(this.language.t('DEPOSIT.ERR_AMOUNT'));
      return;
    }
    if (!this.accountId()) {
      this.error.set(this.language.t('DEPOSIT.ERR_ACCOUNT'));
      return;
    }
    this.submitting.set(true);
    try {
      const payment = await this.paymentService.createPayment({
        provider: this.provider(),
        accountId: this.accountId(),
        amount: amt,
        description: this.description().trim() || undefined,
      });

      // Nạp tiền mặt: backend đã credit ngay → hiện thông báo thành công, không sang trang ví.
      if (this.provider() === PaymentProvider.Cash) {
        const amountStr = new Intl.NumberFormat('vi-VN').format(amt!);
        this.success.set(this.language.t('DEPOSIT.CASH_SUCCESS', { amount: amountStr }));
        await this.refreshAccounts();
        await this.loadHistory();
        return;
      }

      // Giống redirect sang app MoMo/ZaloPay.
      await this.router.navigate([payment.mockPaymentUrl]);
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.submitting.set(false);
    }
  }

  protected statusBadge(status: number): string {
    return status === 2
      ? 'bg-emerald-100 text-emerald-700'
      : status === 3
        ? 'bg-rose-100 text-rose-700'
        : 'bg-amber-100 text-amber-700';
  }

  protected providerLabel(v: number): string {
    if (v === PaymentProvider.Momo) return 'MoMo';
    if (v === PaymentProvider.ZaloPay) return 'ZaloPay';
    return this.language.t('DEPOSIT.CASH');
  }

  protected providerShort(v: number): string {
    if (v === PaymentProvider.Momo) return 'M';
    if (v === PaymentProvider.ZaloPay) return 'Z';
    return '₫';
  }

  protected providerColor(v: number): string {
    if (v === PaymentProvider.Momo) return '#d82d8b';
    if (v === PaymentProvider.ZaloPay) return '#0068ff';
    return '#16a34a';
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('DEPOSIT.ERR_DEFAULT'));
  }
}

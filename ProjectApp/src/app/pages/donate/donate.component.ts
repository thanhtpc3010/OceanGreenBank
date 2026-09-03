import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { DonationService, DonationFund } from '../../core/services/donation.service';
import { TransactionDto, TxStatus } from '../../core/services/transaction.service';
import { TransactionPasswordService } from '../../core/services/transaction-password.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { UserService, BankAccount } from '../../core/services/user.service';

type Step = 'form' | 'confirm' | 'result';

@Component({
  selector: 'app-donate',
  imports: [FormsModule, DecimalPipe, RouterLink, TranslatePipe],
  templateUrl: './donate.component.html',
  styleUrl: './donate.component.scss',
})
export class DonateComponent implements OnInit {
  private readonly donationService = inject(DonationService);
  private readonly userService = inject(UserService);
  private readonly txPasswordService = inject(TransactionPasswordService);
  private readonly language = inject(LanguageService);

  protected readonly step = signal<Step>('form');
  protected readonly submitting = signal(false);
  protected readonly error = signal('');
  protected readonly loading = signal(false);

  /* ---- Danh sách quỹ + tài khoản nguồn ---- */
  protected readonly funds = signal<DonationFund[]>([]);
  protected readonly accounts = signal<BankAccount[]>([]);

  /* ---- Form ---- */
  protected readonly fromAccountId = signal('');
  protected readonly selectedFundId = signal('');
  protected readonly selectedBankCode = signal('');
  protected readonly amount = signal<number | null>(null);
  protected readonly message = signal('');

  /* ---- Kết quả ---- */
  protected readonly result = signal<TransactionDto | null>(null);

  protected readonly selectedFund = computed(
    () => this.funds().find((f) => f.id === this.selectedFundId()) ?? null,
  );

  protected readonly fromAccount = computed(
    () => this.accounts().find((a) => a.id === this.fromAccountId()) ?? null,
  );

  protected readonly isSuccess = computed(() => this.result()?.status === TxStatus.Success);

  async ngOnInit(): Promise<void> {
    this.loading.set(true);
    try {
      const [funds, accounts] = await Promise.all([
        this.donationService.getFunds(),
        this.userService.getAccounts(),
      ]);
      this.funds.set(funds);
      this.accounts.set(accounts);
      const firstActive = accounts.find((a) => a.isActive);
      if (firstActive) this.fromAccountId.set(firstActive.id);
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.loading.set(false);
    }
  }

  /** Chọn quỹ — reset lựa chọn ngân hàng. */
  protected selectFund(id: string): void {
    this.selectedFundId.set(id);
    this.selectedBankCode.set('');
  }

  /** Số tài khoản tiếp nhận đang chọn. */
  protected readonly selectedAccount = computed(() => {
    const fund = this.selectedFund();
    if (!fund) return null;
    return fund.accounts.find((a) => a.bankCode === this.selectedBankCode()) ?? null;
  });

  /** Tổng tiền ghi nợ (ủng hộ miễn phí). */
  protected readonly totalDebit = computed(() => this.amount() ?? 0);

  protected continueToConfirm(): void {
    this.error.set('');
    if (!this.fromAccountId()) {
      this.error.set(this.language.t('ERR.NO_ACCOUNT'));
      return;
    }
    if (!this.selectedFundId()) {
      this.error.set(this.language.t('DONATE.ERR_FUND'));
      return;
    }
    if (!this.selectedBankCode()) {
      this.error.set(this.language.t('DONATE.ERR_BANK'));
      return;
    }
    const amount = this.amount() ?? 0;
    if (amount <= 0) {
      this.error.set(this.language.t('ERR.AMOUNT'));
      return;
    }
    const acc = this.fromAccount();
    if (acc && acc.balance < amount) {
      this.error.set(
        this.language.t('DONATE.ERR_BALANCE', {
          need: amount.toLocaleString('vi-VN'),
          have: acc.balance.toLocaleString('vi-VN'),
        }),
      );
      return;
    }
    this.step.set('confirm');
  }

  protected async submit(): Promise<void> {
    // Mở popup nhập mật khẩu giao dịch (dùng chung toàn app).
    const password = await this.txPasswordService.ask({
      message: `Ủng hộ ${(this.amount() ?? 0).toLocaleString('vi-VN')} VND cho ${this.selectedFund()?.name ?? ''}.`,
    });
    if (password === null) return; // user hủy

    this.submitting.set(true);
    this.error.set('');
    try {
      const tx = await this.donationService.donate({
        fromAccountId: this.fromAccountId(),
        fundId: this.selectedFundId(),
        bankCode: this.selectedBankCode(),
        amount: this.amount() ?? 0,
        message: this.message().trim() || undefined,
        transactionPassword: password,
      });
      this.result.set(tx);
      this.step.set('result');
    } catch (e) {
      this.error.set(this.extractMessage(e));
    } finally {
      this.submitting.set(false);
    }
  }

  protected reset(): void {
    this.step.set('form');
    this.amount.set(null);
    this.message.set('');
    this.selectedFundId.set('');
    this.selectedBankCode.set('');
    this.result.set(null);
    this.error.set('');
  }

  /** Lấy message từ lỗi API. */
  private extractMessage(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('DONATE.ERR_DEFAULT'));
  }
}

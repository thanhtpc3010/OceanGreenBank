import { DecimalPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { TransactionService, TxStatus, TxType, TransactionDto, ReceiverInfo, TxCategory, CATEGORY_OPTIONS, CATEGORY_KEYS } from '../../core/services/transaction.service';
import { TransactionPasswordService } from '../../core/services/transaction-password.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { UserService, BankAccount } from '../../core/services/user.service';

type Step = 'form' | 'confirm' | 'result';

@Component({
  selector: 'app-transfer',
  imports: [FormsModule, DecimalPipe, DatePipe, TranslatePipe],
  templateUrl: './transfer.component.html',
  styleUrl: './transfer.component.scss',
})
export class TransferComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly txService = inject(TransactionService);
  private readonly txPasswordService = inject(TransactionPasswordService);
  private readonly language = inject(LanguageService);

  protected readonly step = signal<Step>('form');
  protected readonly submitting = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');

  /* ---- Form ---- */
  protected readonly fromAccountId = signal('');
  protected readonly transferType = signal<'internal' | 'interbank'>('internal');
  protected readonly receiverAccount = signal('');
  protected readonly receiverName = signal('');
  protected readonly receiverBankCode = signal('');
  protected readonly amount = signal<number | null>(null);
  protected readonly description = signal('');
  protected readonly category = signal<TxCategory>(TxCategory.Other);
  protected readonly categoryOptions = CATEGORY_OPTIONS;
  protected readonly CATEGORY_KEYS = CATEGORY_KEYS;
  protected readonly isEarlyWithdrawal = signal(false);

  /* ---- Trạng thái tra cứu người nhận ---- */
  protected readonly resolved = signal<ReceiverInfo | null>(null);
  protected readonly resolving = signal(false);
  protected readonly resolveError = signal('');

  /* ---- Kết quả giao dịch ---- */
  protected readonly result = signal<TransactionDto | null>(null);

  /* ---- Lịch sử ---- */
  protected readonly history = signal<TransactionDto[]>([]);
  protected readonly historyLoading = signal(false);

  protected readonly accounts = signal<BankAccount[]>([]);

  protected readonly fromAccount = computed(
    () => this.accounts().find((a) => a.id === this.fromAccountId()) ?? null,
  );

  /** Nguồn có phải tài khoản tiết kiệm? */
  protected readonly sourceIsSavings = computed(
    () => this.fromAccount()?.type === 1,
  );

  /** Tài khoản tiết kiệm nguồn đã đáo hạn chưa? */
  protected readonly sourceMatured = computed(() => {
    const acc = this.fromAccount();
    if (!acc || acc.type !== 1 || !acc.savingsMaturityDate) return true;
    return new Date(acc.savingsMaturityDate).getTime() <= Date.now();
  });

  /** Ngày đáo hạn hiển thị. */
  protected readonly sourceMaturityLabel = computed(() => {
    const acc = this.fromAccount();
    if (!acc?.savingsMaturityDate) return '';
    const d = new Date(acc.savingsMaturityDate);
    return isNaN(d.getTime()) ? '' : d.toLocaleDateString('vi-VN');
  });

  /** Phí chuyển tiền: nội bộ 0đ, liên ngân hàng 5.000đ. */
  protected readonly fee = computed(() =>
    this.transferType() === 'interbank' ? 5000 : 0,
  );

  protected readonly totalDebit = computed(
    () => (this.amount() ?? 0) + this.fee(),
  );

  async ngOnInit(): Promise<void> {
    const accounts = await this.userService.getAccounts();
    this.accounts.set(accounts);
    const firstActive = accounts.find((a) => a.isActive);
    if (firstActive) this.fromAccountId.set(firstActive.id);
    await this.loadHistory(this.fromAccountId());
  }

  protected setType(type: 'internal' | 'interbank'): void {
    this.transferType.set(type);
    this.resolved.set(null);
    this.resolveError.set('');
  }

  /** Tra cứu tài khoản nhận (nội bộ) theo số tài khoản. */
  protected async lookupReceiver(): Promise<void> {
    const number = this.receiverAccount().trim();
    if (!number) return;
    this.resolving.set(true);
    this.resolveError.set('');
    this.resolved.set(null);
    try {
      const found = await this.txService.findAccountByNumber(number);
      if (!found) {
        this.resolveError.set(this.language.t('TRANSFER.ERR_RECEIVER_NOT_FOUND'));
      } else if (found.accountId === this.fromAccountId()) {
        this.resolveError.set(this.language.t('TRANSFER.ERR_SELF'));
      } else {
        this.resolved.set(found);
      }
    } finally {
      this.resolving.set(false);
    }
  }

  /** Validate bước form → sang bước xác nhận. */
  protected continueToConfirm(): void {
    this.error.set('');
    this.success.set('');

    if (!this.fromAccountId()) {
      this.error.set(this.language.t('ERR.NO_ACCOUNT'));
      return;
    }
    const amount = this.amount() ?? 0;
    if (amount <= 0) {
      this.error.set(this.language.t('ERR.AMOUNT'));
      return;
    }
    const acc = this.fromAccount();
    if (acc && acc.balance < this.totalDebit()) {
      this.error.set(
        this.language.t('TRANSFER.ERR_BALANCE', {
          need: this.totalDebit().toLocaleString('vi-VN'),
          have: acc.balance.toLocaleString('vi-VN'),
        }),
      );
      return;
    }

    // Tài khoản tiết kiệm: chưa đáo hạn → bắt buộc xác nhận rút trước hạn.
    if (this.sourceIsSavings() && !this.sourceMatured() && !this.isEarlyWithdrawal()) {
      this.error.set(
        this.language.t('TRANSFER.ERR_NOT_MATURED', { date: this.sourceMaturityLabel() }),
      );
      return;
    }

    if (this.transferType() === 'internal') {
      if (!this.receiverAccount().trim()) {
        this.error.set(this.language.t('TRANSFER.ERR_RECEIVER_REQUIRED'));
        return;
      }
      if (!this.resolved()) {
        this.error.set(this.language.t('TRANSFER.ERR_RESOLVE_REQUIRED'));
        return;
      }
    } else {
      if (!this.receiverName().trim() || !this.receiverAccount().trim() || !this.receiverBankCode().trim()) {
        this.error.set(this.language.t('TRANSFER.ERR_RECEIVER_INFO'));
        return;
      }
    }

    this.step.set('confirm');
  }

  /** Gọi API chuyển tiền. */
  protected async confirmTransfer(): Promise<void> {
    // Mở popup nhập mật khẩu giao dịch (dùng chung toàn app).
    const password = await this.txPasswordService.ask({
      message: this.language.t('TRANSFER.ASK_MSG', {
        amount: (this.amount() ?? 0).toLocaleString('vi-VN'),
        account: this.fromAccount()?.accountNumber ?? '',
      }),
    });
    if (password === null) return; // user hủy

    this.submitting.set(true);
    this.error.set('');
    try {
      const isInternal = this.transferType() === 'internal';
      const tx = await this.txService.transfer({
        fromAccountId: this.fromAccountId(),
        type: isInternal ? TxType.InternalTransfer : TxType.InterbankTransfer,
        amount: this.amount() ?? 0,
        description: this.description().trim() || undefined,
        category: this.category(),
        toAccountId: isInternal ? (this.resolved()?.accountId ?? undefined) : undefined,
        receiverAccount: isInternal ? undefined : this.receiverAccount().trim(),
        receiverName: isInternal ? (this.resolved()?.ownerName ?? undefined) : this.receiverName().trim(),
        receiverBankCode: isInternal ? undefined : this.receiverBankCode().trim(),
        isEarlyWithdrawal: this.isEarlyWithdrawal(),
        transactionPassword: password,
      });
      this.result.set(tx);
      this.step.set('result');
      // Làm mới số dư + lịch sử
      await this.userService.getAccounts();
      await this.loadHistory(this.fromAccountId());
    } catch (e) {
      this.error.set(this.extractError(e));
      this.step.set('form');
    } finally {
      this.submitting.set(false);
    }
  }

  protected resetForm(): void {
    this.step.set('form');
    this.result.set(null);
    this.error.set('');
    this.success.set('');
    this.receiverAccount.set('');
    this.receiverName.set('');
    this.receiverBankCode.set('');
    this.resolved.set(null);
    this.resolveError.set('');
    this.amount.set(null);
    this.description.set('');
    this.category.set(TxCategory.Other);
    this.isEarlyWithdrawal.set(false);
  }

  /** Tải lịch sử giao dịch của tài khoản. */
  protected async loadHistory(accountId: string): Promise<void> {
    if (!accountId) return;
    this.historyLoading.set(true);
    try {
      const list = await this.txService.getTransactions(accountId);
      // Sắp xếp mới nhất trước
      this.history.set([...list].sort((a, b) => b.createdDate.localeCompare(a.createdDate)));
    } finally {
      this.historyLoading.set(false);
    }
  }

  protected onFromAccountChange(): void {
    void this.loadHistory(this.fromAccountId());
    this.resolved.set(null);
    this.resolveError.set('');
  }

  protected statusLabel(s: TxStatus): string {
    return s === TxStatus.Success
      ? this.language.t('TRANSFER.STATUS_SUCCESS')
      : s === TxStatus.Pending
        ? this.language.t('TRANSFER.STATUS_PENDING')
        : this.language.t('TRANSFER.STATUS_FAILED');
  }

  protected categoryLabel(c: TxCategory): string {
    return this.language.t(CATEGORY_KEYS[c] ?? 'CATEGORY.OTHER');
  }

  protected typeLabel(t: TxType): string {
    return t === TxType.InternalTransfer
      ? this.language.t('TRANSFER.INTERNAL')
      : this.language.t('TRANSFER.INTERBANK');
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : this.language.t('TRANSFER.ERR_DEFAULT'));
  }
}

import { DecimalPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { TransactionService, TxStatus, TxType, TransactionDto, ReceiverInfo, TxCategory, CATEGORY_OPTIONS } from '../../core/services/transaction.service';
import { UserService, BankAccount } from '../../core/services/user.service';

type Step = 'form' | 'confirm' | 'result';

@Component({
  selector: 'app-transfer',
  imports: [FormsModule, DecimalPipe, DatePipe],
  templateUrl: './transfer.component.html',
  styleUrl: './transfer.component.scss',
})
export class TransferComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly txService = inject(TransactionService);

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
        this.resolveError.set('Không tìm thấy tài khoản nhận. Vui lòng kiểm tra lại số tài khoản.');
      } else if (found.accountId === this.fromAccountId()) {
        this.resolveError.set('Không thể chuyển tiền vào chính tài khoản của bạn.');
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
      this.error.set('Vui lòng chọn tài khoản nguồn.');
      return;
    }
    const amount = this.amount() ?? 0;
    if (amount <= 0) {
      this.error.set('Số tiền phải lớn hơn 0.');
      return;
    }
    const acc = this.fromAccount();
    if (acc && acc.balance < this.totalDebit()) {
      this.error.set(
        `Số dư không đủ. Bạn cần ${this.totalDebit().toLocaleString('vi-VN')} VND (gồm phí) nhưng chỉ có ${acc.balance.toLocaleString('vi-VN')} VND.`,
      );
      return;
    }

    if (this.transferType() === 'internal') {
      if (!this.receiverAccount().trim()) {
        this.error.set('Vui lòng nhập số tài khoản nhận.');
        return;
      }
      if (!this.resolved()) {
        this.error.set('Vui lòng xác nhận tài khoản nhận hợp lệ trước khi tiếp tục.');
        return;
      }
    } else {
      if (!this.receiverName().trim() || !this.receiverAccount().trim() || !this.receiverBankCode().trim()) {
        this.error.set('Vui lòng nhập đầy đủ thông tin người nhận (tên, số tài khoản, mã ngân hàng).');
        return;
      }
    }

    this.step.set('confirm');
  }

  /** Gọi API chuyển tiền. */
  protected async confirmTransfer(): Promise<void> {
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
    return s === TxStatus.Success ? 'Thành công' : s === TxStatus.Pending ? 'Chờ xử lý' : 'Thất bại';
  }

  protected categoryLabel(c: TxCategory): string {
    return CATEGORY_OPTIONS.find((o) => o.value === c)?.label ?? 'Khác';
  }

  protected typeLabel(t: TxType): string {
    return t === TxType.InternalTransfer ? 'Nội bộ' : 'Liên ngân hàng';
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : 'Chuyển tiền thất bại. Vui lòng thử lại.');
  }
}

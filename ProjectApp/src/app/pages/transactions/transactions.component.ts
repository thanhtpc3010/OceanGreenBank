import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import {
  TransactionService,
  TransactionDto,
  TxType,
  TxStatus,
  TxCategory,
  CATEGORY_LABELS,
} from '../../core/services/transaction.service';
import { UserService, BankAccount } from '../../core/services/user.service';

interface TxView {
  id: string;
  transactionCode: string;
  createdDate: string;
  description: string;
  category: TxCategory;
  categoryName: string;
  amount: number;
  type: TxType;
  status: TxStatus;
  accountNumber: string;
  receiverName: string | null;
}

@Component({
  selector: 'app-transactions',
  imports: [DatePipe, DecimalPipe, FormsModule, RouterLink],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.scss',
})
export class TransactionsComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly txService = inject(TransactionService);

  protected readonly loading = signal(true);
  protected readonly error = signal('');
  protected readonly accounts = signal<BankAccount[]>([]);
  protected readonly selectedAccountId = signal('all');
  protected readonly rows = signal<TxView[]>([]);
  protected readonly search = signal('');

  protected readonly filteredRows = computed(() => {
    const q = this.search().toLowerCase().trim();
    if (!q) return this.rows();
    return this.rows().filter(
      (r) =>
        r.description.toLowerCase().includes(q) ||
        r.categoryName.toLowerCase().includes(q) ||
        (r.receiverName ?? '').toLowerCase().includes(q) ||
        r.transactionCode.toLowerCase().includes(q),
    );
  });

  protected readonly totalIn = computed(() =>
    this.filteredRows()
      .filter((r) => r.amount > 0)
      .reduce((s, r) => s + r.amount, 0),
  );
  protected readonly totalOut = computed(() =>
    this.filteredRows()
      .filter((r) => r.amount < 0)
      .reduce((s, r) => s + r.amount, 0),
  );

  async ngOnInit(): Promise<void> {
    await this.load();
  }

  private async load(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      const accounts = await this.userService.getAccounts();
      this.accounts.set(accounts);
      const all = await this.loadAll();
      this.rows.set(this.toViews(all));
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.loading.set(false);
    }
  }

  /** Load giao dịch tất cả tài khoản đang hoạt động (gộp lại + loại trùng theo mã GD). */
  private async loadAll(): Promise<TransactionDto[]> {
    const active = this.accounts().filter((a) => a.isActive);
    const map = new Map<string, TransactionDto>();
    for (const a of active) {
      try {
        for (const t of await this.txService.getTransactions(a.id)) map.set(t.id, t);
      } catch {
        // Bỏ qua tài khoản lỗi.
      }
    }
    return [...map.values()];
  }

  /** Khi đổi bộ lọc tài khoản. */
  protected async onAccountChange(): Promise<void> {
    this.loading.set(true);
    this.error.set('');
    try {
      const id = this.selectedAccountId();
      if (id === 'all') {
        this.rows.set(this.toViews(await this.loadAll()));
      } else {
        const txns = await this.txService.getTransactions(id);
        this.rows.set(this.toViews(txns));
      }
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.loading.set(false);
    }
  }

  private toViews(txns: TransactionDto[]): TxView[] {
    const byId = new Map(this.accounts().map((a) => [a.id, a.accountNumber] as const));
    return [...txns]
      .map((t) => ({
        id: t.id,
        transactionCode: t.transactionCode,
        createdDate: t.createdDate,
        description: t.description || 'Chuyển tiền',
        category: t.category,
        categoryName: CATEGORY_LABELS[t.category] ?? 'Khác',
        // Tiền ra (từ tài khoản của user) → âm; tiền vào → dương.
        amount: byId.has(t.fromAccountId) ? -t.amount : t.amount,
        type: t.type,
        status: t.status,
        accountNumber: byId.get(t.fromAccountId) ?? '',
        receiverName: t.receiverName ?? (t.toAccountId ? (byId.get(t.toAccountId) ?? null) : null),
      }))
      .sort((a, b) => b.createdDate.localeCompare(a.createdDate));
  }

  protected statusLabel(s: TxStatus): string {
    return s === TxStatus.Success ? 'Thành công' : s === TxStatus.Pending ? 'Chờ xử lý' : 'Thất bại';
  }

  protected typeLabel(t: TxType): string {
    return t === TxType.InternalTransfer ? 'Nội bộ' : 'Liên ngân hàng';
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : 'Có lỗi xảy ra. Vui lòng thử lại.');
  }
}

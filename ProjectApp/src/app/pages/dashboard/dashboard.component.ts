import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';
import { AutoEarnService } from '../../core/services/auto-earn.service';
import { TransactionService, TxCategory, TransactionDto } from '../../core/services/transaction.service';
import { UserService } from '../../core/services/user.service';

interface DonutSegment {
  label: string;
  value: number;
  color: string;
}

interface CashflowItem {
  month: string;
  income: number;
  expense: number;
}

/** Màu cho từng danh mục chi tiêu (donut chart). */
const CATEGORY_COLORS: Record<TxCategory, string> = {
  [TxCategory.Other]: '#94a3b8',
  [TxCategory.Food]: '#10b981',
  [TxCategory.Shopping]: '#0ea5e9',
  [TxCategory.Bills]: '#8b5cf6',
  [TxCategory.Transport]: '#f59e0b',
  [TxCategory.Entertainment]: '#f43f5e',
  [TxCategory.Health]: '#14b8a6',
  [TxCategory.Education]: '#6366f1',
  [TxCategory.Savings]: '#22c55e',
  [TxCategory.Transfer]: '#64748b',
};

const CATEGORY_NAMES: Record<TxCategory, string> = {
  [TxCategory.Other]: 'Khác',
  [TxCategory.Food]: 'Ăn uống',
  [TxCategory.Shopping]: 'Mua sắm',
  [TxCategory.Bills]: 'Hóa đơn',
  [TxCategory.Transport]: 'Di chuyển',
  [TxCategory.Entertainment]: 'Giải trí',
  [TxCategory.Health]: 'Y tế',
  [TxCategory.Education]: 'Giáo dục',
  [TxCategory.Savings]: 'Tiết kiệm',
  [TxCategory.Transfer]: 'Chuyển khoản',
};

@Component({
  selector: 'app-dashboard',
  imports: [DecimalPipe, DatePipe, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly txService = inject(TransactionService);
  private readonly autoEarnService = inject(AutoEarnService);
  private readonly auth = inject(AuthService);

  protected readonly today = new Date();
  protected readonly userName = signal('Nguyễn Văn A');
  protected readonly isAdmin = computed(() => this.auth.hasPermission('USER.READ'));

  // CASA
  protected readonly accountNumber = signal('9999 8888');
  protected readonly balance = signal(15_000_000);

  // AutoEarn (dữ liệu thật từ API)
  protected readonly autoEarnActive = signal(true);
  protected readonly interestRate = signal(4.5);
  protected readonly monthlyAccum = signal(0);
  protected readonly autoEarnRunTime = signal('00:00');
  protected readonly autoEarnPrincipal = signal(0);

  // ===== PFM (dữ liệu thật từ API) =====
  protected readonly donutSegments = signal<DonutSegment[]>([]);
  protected readonly pfmIncome = signal(0);
  protected readonly pfmExpense = signal(0);
  protected readonly pfmNet = signal(0);
  protected readonly pfmLoading = signal(true);

  protected readonly donutTotal = () =>
    this.donutSegments().reduce((s, x) => s + x.value, 0);
  private readonly circumference = 2 * Math.PI * 15.9;

  // ===== Giao dịch gần đây (thật) =====
  protected readonly recentTransactions = signal<
    { desc: string; amount: number; time: string; color: string }[]
  >([]);

  async ngOnInit(): Promise<void> {
    const profile = await this.userService.getProfile();
    this.userName.set(profile.fullName);
    const accounts = await this.userService.getAccounts();
    const firstActive = accounts.find((a) => a.isActive);
    if (firstActive) {
      this.accountNumber.set(firstActive.accountNumber);
      this.balance.set(firstActive.balance);
      this.loadRecent(firstActive.id);
    }
    await this.loadPfm(profile.id);
    await this.loadAutoEarn(profile.id);
  }

  private async loadAutoEarn(userId: string): Promise<void> {
    try {
      const ae = await this.autoEarnService.getSummary(userId);
      this.autoEarnActive.set(ae.isActive);
      this.interestRate.set(ae.annualInterestRate);
      this.monthlyAccum.set(ae.monthlyAccum);
      this.autoEarnRunTime.set(ae.runTime);
      this.autoEarnPrincipal.set(ae.totalPrincipal);
    } catch {
      // Giữ giá trị mặc định nếu API lỗi.
    }
  }

  private async loadPfm(userId: string): Promise<void> {
    this.pfmLoading.set(true);
    try {
      const summary = await this.txService.getPfmSummary(userId);
      this.pfmIncome.set(summary.totalIncome);
      this.pfmExpense.set(summary.totalExpense);
      this.pfmNet.set(summary.net);
      this.donutSegments.set(
        summary.expenseByCategory.map((c) => ({
          label: c.categoryName,
          value: c.total,
          color: CATEGORY_COLORS[c.category] ?? '#94a3b8',
        })),
      );
    } finally {
      this.pfmLoading.set(false);
    }
  }

  private async loadRecent(accountId: string): Promise<void> {
    try {
      const list = await this.txService.getTransactions(accountId);
      this.recentTransactions.set(
        [...list]
          .sort((a, b) => b.createdDate.localeCompare(a.createdDate))
          .slice(0, 5)
          .map((t: TransactionDto) => ({
            desc: `${CATEGORY_NAMES[t.category] ?? 'Khác'} — ${t.description || 'Chuyển tiền'}`,
            amount: -t.amount,
            time: new Date(t.createdDate).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
            color: '#f43f5e',
          })),
      );
    } catch {
      // giữ trống nếu lỗi
    }
  }

  protected segDash(seg: DonutSegment): string {
    const total = this.donutTotal();
    if (total === 0) return `0 ${this.circumference.toFixed(2)}`;
    const frac = seg.value / total;
    return `${(frac * this.circumference).toFixed(2)} ${this.circumference.toFixed(2)}`;
  }

  protected segOffset(index: number): number {
    const total = this.donutTotal();
    if (total === 0) return 0;
    let acc = 0;
    for (let i = 0; i < index; i++) {
      acc += (this.donutSegments()[i].value / total) * this.circumference;
    }
    return -acc;
  }

  // ===== Bar chart: cashflow (mock 6 tháng) =====
  protected readonly cashflow: CashflowItem[] = [
    { month: 'T3', income: 16_000_000, expense: 10_500_000 },
    { month: 'T4', income: 17_500_000, expense: 12_000_000 },
    { month: 'T5', income: 15_000_000, expense: 9_800_000 },
    { month: 'T6', income: 18_200_000, expense: 13_400_000 },
    { month: 'T7', income: 19_000_000, expense: 11_200_000 },
    { month: 'T8', income: 16_800_000, expense: 10_900_000 },
  ];
  protected readonly maxFlow = Math.max(...this.cashflow.flatMap((c) => [c.income, c.expense]));

  protected barHeight(value: number): number {
    return Math.round((value / this.maxFlow) * 100);
  }

  protected format(n: number): string {
    return new Intl.NumberFormat('vi-VN').format(n);
  }
}


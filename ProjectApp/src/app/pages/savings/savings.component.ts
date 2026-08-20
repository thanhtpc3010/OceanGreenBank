import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { ConfirmService } from '../../core/services/confirm.service';
import { SavingsService, SavingsPlanDto, CYCLE_OPTIONS } from '../../core/services/savings.service';
import { UserService, BankAccount } from '../../core/services/user.service';

@Component({
  selector: 'app-savings',
  imports: [FormsModule, DecimalPipe],
  templateUrl: './savings.component.html',
  styleUrl: './savings.component.scss',
})
export class SavingsComponent implements OnInit {
  private readonly userService = inject(UserService);
  protected readonly savingsService = inject(SavingsService);
  private readonly confirmService = inject(ConfirmService);

  protected readonly accounts = signal<BankAccount[]>([]);
  protected readonly plans = signal<SavingsPlanDto[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly error = signal('');
  protected readonly success = signal('');

  protected readonly cycleOptions = CYCLE_OPTIONS;

  /* ---- Form ---- */
  protected readonly sourceAccountId = signal('');
  protected readonly targetAccountId = signal('');
  protected readonly amount = signal<number | null>(null);
  protected readonly cycle = signal('MONTHLY');
  protected readonly startDate = signal(new Date().toISOString().slice(0, 10));

  protected readonly sourceAccounts = computed(() =>
    this.accounts().filter((a) => a.isActive),
  );
  protected readonly targetAccounts = computed(() =>
    this.accounts().filter((a) => a.isActive && a.id !== this.sourceAccountId()),
  );

  protected readonly totalPlan = computed(() =>
    this.plans()
      .filter((p) => p.isActive)
      .reduce((sum, p) => sum + p.totalSaved, 0),
  );

  async ngOnInit(): Promise<void> {
    const profile = await this.userService.getProfile();
    const accounts = await this.userService.getAccounts();
    this.accounts.set(accounts);
    const first = accounts.find((a) => a.isActive);
    if (first) {
      this.sourceAccountId.set(first.id);
      const target = accounts.find((a) => a.isActive && a.id !== first.id);
      if (target) this.targetAccountId.set(target.id);
    }
    await this.loadPlans(profile.id);
  }

  private async loadPlans(userId: string): Promise<void> {
    this.loading.set(true);
    try {
      this.plans.set(await this.savingsService.getPlans(userId));
    } finally {
      this.loading.set(false);
    }
  }

  protected async createPlan(): Promise<void> {
    this.error.set('');
    this.success.set('');

    if (!this.sourceAccountId() || !this.targetAccountId()) {
      this.error.set('Vui lòng chọn tài khoản nguồn và tài khoản tiết kiệm đích.');
      return;
    }
    if (this.sourceAccountId() === this.targetAccountId()) {
      this.error.set('Tài khoản nguồn và đích phải khác nhau.');
      return;
    }
    const amount = this.amount() ?? 0;
    if (amount <= 0) {
      this.error.set('Số tiền gửi mỗi kỳ phải lớn hơn 0.');
      return;
    }

    this.saving.set(true);
    try {
      const profile = await this.userService.getProfile();
      await this.savingsService.createPlan({
        userId: profile.id,
        sourceAccountId: this.sourceAccountId(),
        targetAccountId: this.targetAccountId(),
        amount,
        cycle: this.cycle(),
        startDate: this.startDate() + 'T00:00:00',
      });
      this.success.set('Đã tạo kế hoạch tiết kiệm định kỳ thành công!');
      await this.loadPlans(profile.id);
    } catch (e) {
      this.error.set(this.extractError(e));
    } finally {
      this.saving.set(false);
    }
  }

  protected async depositNow(plan: SavingsPlanDto): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: 'Gửi tiền kỳ này',
      message: `Gửi ${plan.amount.toLocaleString('vi-VN')} VND vào sổ tiết kiệm (${plan.targetAccountNumber})?`,
      confirmText: 'Gửi ngay',
    });
    if (!ok) return;
    try {
      const updated = await this.savingsService.deposit(plan.id);
      this.success.set(`Đã gửi ${updated.amount.toLocaleString('vi-VN')} VND thành công!`);
      this.plans.update((list) => list.map((p) => (p.id === updated.id ? updated : p)));
      await this.userService.getAccounts();
      await this.loadPlans(updated.userId);
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  protected async cancelPlan(plan: SavingsPlanDto): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: 'Hủy kế hoạch tiết kiệm',
      message: `Hủy kế hoạch gửi ${plan.amount.toLocaleString('vi-VN')} VND / ${this.savingsService.cycleLabel(plan.cycle)}? Số tiền đã gửi không bị ảnh hưởng.`,
      confirmText: 'Hủy kế hoạch',
      danger: true,
    });
    if (!ok) return;
    try {
      await this.savingsService.cancelPlan(plan.id);
      this.success.set('Đã hủy kế hoạch.');
      await this.loadPlans(plan.userId);
    } catch (e) {
      this.error.set(this.extractError(e));
    }
  }

  protected formatDate(iso: string | null): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN');
  }

  private extractError(e: unknown): string {
    const body = (e as { error?: { message?: string } })?.error;
    return body?.message ?? (e instanceof Error ? e.message : 'Có lỗi xảy ra.');
  }
}

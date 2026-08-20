import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { ConfirmService } from '../../core/services/confirm.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-account',
  imports: [DecimalPipe, RouterLink, FormsModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss',
})
export class AccountComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly confirmService = inject(ConfirmService);

  protected readonly deleting = signal(false);

  /* ---- Modal mở sổ tiết kiệm ---- */
  protected readonly showSavingsModal = signal(false);
  protected readonly savingsTerm = signal(3);
  protected readonly savingsRate = signal(4.5);
  /** Lãi suất mặc định (%/năm) theo kỳ hạn (tháng). */
  protected readonly savingsRates: Record<number, number> = { 1: 3.5, 3: 4.5, 6: 5.5, 12: 6.5 };
  protected readonly savingsTermOptions = [1, 3, 6, 12];
  protected readonly savingsSaving = signal(false);

  /* ---- Dữ liệu lấy từ UserService (signal reactive) ---- */
  protected readonly profile = this.userService.profile;
  protected readonly accounts = this.userService.accounts;
  protected readonly loading = this.userService.loading;

  protected readonly fullName = computed(() => this.profile()?.fullName ?? 'Người dùng');
  protected readonly email = computed(() => this.profile()?.email ?? '');
  protected readonly phone = computed(() => this.profile()?.phone ?? '');
  protected readonly identityCard = computed(() => this.profile()?.identityCard ?? '');
  protected readonly dateOfBirth = computed(() => this.profile()?.dateOfBirth ?? '');
  protected readonly gender = computed(() => this.profile()?.gender ?? '');
  protected readonly address = computed(() => this.profile()?.address ?? '');
  protected readonly memberSince = computed(() => this.profile()?.memberSince ?? '');

  /** Tổng số dư các tài khoản đang hoạt động. */
  protected readonly totalBalance = computed(() =>
    this.accounts()
      .filter((a) => a.isActive)
      .reduce((sum, a) => sum + a.balance, 0),
  );

  /** Số tài khoản đang hoạt động. */
  protected readonly activeCount = computed(
    () => this.accounts().filter((a) => a.isActive).length,
  );

  async ngOnInit(): Promise<void> {
    await this.userService.loadAll();
  }

  /** Format mã số tài khoản hiển thị (giấu 4 số giữa). */
  protected maskAccount(number: string): string {
    const compact = number.replace(/\s/g, '');
    if (compact.length < 8) return number;
    return compact.slice(0, 4) + ' •••• •••• ' + compact.slice(-4);
  }

  /** Format ngày ISO → dd/MM/yyyy. */
  protected formatDate(iso: string | null): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN');
  }

  /** Format ngày ISO → MM/yyyy. */
  protected formatMonthYear(iso: string | null): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN', { month: '2-digit', year: 'numeric' });
  }

  /** Thêm tài khoản mới. */
  protected async addAccount(): Promise<void> {
    await this.userService.addAccount();
  }

  /** Chọn kỳ hạn → tự cập nhật lãi suất mặc định. */
  protected setSavingsTerm(term: number): void {
    this.savingsTerm.set(term);
    this.savingsRate.set(this.savingsRates[term] ?? 4.5);
  }

  /** Mở sổ tiết kiệm — tạo trực tiếp từ modal (đã xác nhận kỳ hạn + lãi suất). */
  protected async openSavingsAccount(): Promise<void> {
    this.showSavingsModal.set(false);
    this.savingsSaving.set(true);
    try {
      await this.userService.addSavingsAccount(this.savingsTerm(), this.savingsRate());
    } finally {
      this.savingsSaving.set(false);
    }
  }

  /** Tài khoản đã đáo hạn chưa (chỉ với tiết kiệm). */
  protected isMatured(acc: { type: number; savingsMaturityDate: string | null }): boolean {
    if (acc.type !== 1 || !acc.savingsMaturityDate) return true;
    const d = new Date(acc.savingsMaturityDate);
    return !isNaN(d.getTime()) && d.getTime() <= Date.now();
  }

  /** Số ngày còn lại đến đáo hạn. */
  protected daysToMaturity(acc: { savingsMaturityDate: string | null }): number {
    if (!acc.savingsMaturityDate) return 0;
    const d = new Date(acc.savingsMaturityDate);
    if (isNaN(d.getTime())) return 0;
    return Math.max(0, Math.ceil((d.getTime() - Date.now()) / 86_400_000));
  }

  /** Xóa 1 tài khoản (có popup xác nhận chung). */
  protected async removeAccount(id: string): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: 'Xóa tài khoản ngân hàng',
      message: 'Bạn có chắc muốn xóa tài khoản này? Hành động không thể hoàn tác.',
      confirmText: 'Xóa',
      danger: true,
    });
    if (!ok) return;
    await this.userService.deleteAccount(id);
  }

  /** Xóa toàn bộ tài khoản người dùng → đăng xuất (có popup xác nhận chung). */
  protected async deleteUser(): Promise<void> {
    const ok = await this.confirmService.confirm({
      title: 'Xóa tài khoản của tôi',
      message: 'Toàn bộ hồ sơ, tài khoản ngân hàng và dữ liệu liên quan sẽ bị xóa vĩnh viễn. Bạn chắc chắn chứ?',
      confirmText: 'Xóa vĩnh viễn',
      danger: true,
    });
    if (!ok) return;
    this.deleting.set(true);
    try {
      await this.userService.deleteUser();
      this.router.navigate(['/login']);
    } finally {
      this.deleting.set(false);
    }
  }
}

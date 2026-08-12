import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-account',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss',
})
export class AccountComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  /** Trạng thái xác nhận xóa user. */
  protected readonly confirmDelete = signal(false);
  protected readonly deleting = signal(false);

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
  protected formatDate(iso: string): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN');
  }

  /** Format ngày ISO → MM/yyyy. */
  protected formatMonthYear(iso: string): string {
    if (!iso) return '—';
    const d = new Date(iso);
    return isNaN(d.getTime()) ? iso : d.toLocaleDateString('vi-VN', { month: '2-digit', year: 'numeric' });
  }

  /** Thêm tài khoản mới. */
  protected async addAccount(): Promise<void> {
    await this.userService.addAccount();
  }

  /** Xóa 1 tài khoản (có xác nhận). */
  protected async removeAccount(id: string): Promise<void> {
    if (!confirm('Bạn có chắc muốn xóa tài khoản này?')) return;
    await this.userService.deleteAccount(id);
  }

  /** Xóa toàn bộ tài khoản người dùng → đăng xuất. */
  protected async deleteUser(): Promise<void> {
    this.deleting.set(true);
    try {
      await this.userService.deleteUser();
      this.router.navigate(['/login']);
    } finally {
      this.deleting.set(false);
    }
  }
}

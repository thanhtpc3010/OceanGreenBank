import { DecimalPipe } from '@angular/common';
import { Component, computed, EventEmitter, inject, OnInit, Output, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

import { AuthService } from '../../core/auth/auth.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import {
  TransactionService,
  TransactionDto,
  CATEGORY_KEYS,
} from '../../core/services/transaction.service';
import { UserService } from '../../core/services/user.service';

interface NotifItem {
  id: string;
  categoryKey: string;
  description: string;
  time: string;
  amount: number;
  color: string;
}

@Component({
  selector: 'app-header',
  imports: [DecimalPipe, RouterLink, ButtonModule, AvatarModule, BadgeModule, TooltipModule, TranslatePipe],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit {
  @Output() menuToggle = new EventEmitter<void>();

  protected readonly balance = signal(15_000_000);

  private readonly language = inject(LanguageService);
  private readonly userService = inject(UserService);
  private readonly txService = inject(TransactionService);
  protected readonly currentLang = this.language.current;
  protected readonly CATEGORY_KEYS = CATEGORY_KEYS;

  protected readonly showNotif = signal(false);
  protected readonly notifLoading = signal(false);
  protected readonly notifications = signal<NotifItem[]>([]);
  protected readonly notificationCount = computed(() => this.notifications().length);

  protected readonly userName = computed(
    () => this.auth.currentUser()?.fullName ?? this.language.t('COMMON.USER_FALLBACK'),
  );
  protected readonly userEmail = computed(
    () => this.auth.currentUser()?.email ?? '',
  );

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router,
  ) {}

  async ngOnInit(): Promise<void> {
    await this.loadNotifications();
  }

  /** Mở/đóng popup thông báo (lịch sử giao dịch gần đây). */
  protected async toggleNotifications(): Promise<void> {
    this.showNotif.update((v) => !v);
    if (this.showNotif()) {
      await this.loadNotifications();
    }
  }

  /** Tải các giao dịch gần đây của tài khoản đang hoạt động đầu tiên. */
  private async loadNotifications(): Promise<void> {
    this.notifLoading.set(true);
    try {
      const accounts = await this.userService.getAccounts();
      const myIds = new Set(accounts.map((a) => a.id));
      const acc = accounts.find((a) => a.isActive);
      if (!acc) {
        this.notifications.set([]);
        return;
      }
      const txns = await this.txService.getTransactions(acc.id);
      this.notifications.set(
        [...txns]
          .sort((a, b) => b.createdDate.localeCompare(a.createdDate))
          .slice(0, 6)
          .map((t: TransactionDto) => {
            const isOut = myIds.has(t.fromAccountId);
            return {
              id: t.id,
              categoryKey: CATEGORY_KEYS[t.category] ?? 'CATEGORY.OTHER',
              description: t.description || '',
              time: new Date(t.createdDate).toLocaleString('vi-VN', {
                day: '2-digit',
                month: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
              }),
              amount: isOut ? -t.amount : t.amount,
              color: isOut ? '#f43f5e' : '#10b981',
            };
          }),
      );
    } catch {
      this.notifications.set([]);
    } finally {
      this.notifLoading.set(false);
    }
  }

  /** Chuyển đổi Việt ↔ Anh. */
  protected toggleLanguage(): void {
    void this.language.setLanguage(this.currentLang() === 'vi' ? 'en' : 'vi');
  }

  protected logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, EventEmitter, inject, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';

import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';

export interface NavItem {
  label: string;
  icon: string;
  /** Có route thật chưa? */
  href?: string;
  comingSoon?: boolean;
  /** Chỉ hiện khi user có quyền USER.READ (admin). */
  adminOnly?: boolean;
}

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, NgTemplateOutlet, TagModule, ButtonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  @Input() open = false;
  @Output() close = new EventEmitter<void>();

  private readonly auth = inject(AuthService);

  /** Chỉ hiện menu quản trị khi user có quyền USER.READ. */
  protected readonly isAdmin = computed(() => this.auth.hasPermission('USER.READ'));

  protected readonly allNavItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', href: '/dashboard' },
    { label: 'Tài khoản', icon: 'account', href: '/account' },
    { label: 'Chuyển tiền', icon: 'transfer', href: '/transfer' },
    { label: 'Giao dịch', icon: 'history', href: '/transactions' },
    { label: 'Tiết kiệm', icon: 'savings', href: '/savings' },
    { label: 'Quản lý User', icon: 'users', href: '/admin/users', adminOnly: true },
    { label: 'AutoEarn', icon: 'autoearn', href: '/admin/auto-earn', adminOnly: true },
    { label: 'Train AI', icon: 'bot', href: '/admin/knowledge', adminOnly: true },
    { label: 'Ủng hộ MTTQ', icon: 'donate', comingSoon: true },
    { label: 'PFM AI Bot', icon: 'bot', comingSoon: true },
    { label: 'Dịch vụ khác', icon: 'services', comingSoon: true },
  ];

  protected readonly navItems = computed(() =>
    this.allNavItems.filter((i) => !i.adminOnly || this.isAdmin()),
  );
}

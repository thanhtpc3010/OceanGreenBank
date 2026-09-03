import { NgTemplateOutlet } from '@angular/common';
import { Component, computed, EventEmitter, inject, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '../../core/auth/auth.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';

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
  imports: [RouterLink, RouterLinkActive, NgTemplateOutlet, TagModule, ButtonModule, TranslatePipe],
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
    { label: 'SIDEBAR.DASHBOARD', icon: 'dashboard', href: '/dashboard' },
    { label: 'SIDEBAR.ACCOUNT', icon: 'account', href: '/account' },
    { label: 'SIDEBAR.TRANSFER', icon: 'transfer', href: '/transfer' },
    { label: 'SIDEBAR.DEPOSIT', icon: 'deposit', href: '/deposit' },
    { label: 'SIDEBAR.TRANSACTIONS', icon: 'history', href: '/transactions' },
    { label: 'SIDEBAR.SAVINGS', icon: 'savings', href: '/savings' },
    { label: 'SIDEBAR.DONATE', icon: 'donate', href: '/donate' },
    { label: 'SIDEBAR.ADMIN_USERS', icon: 'users', href: '/admin/users', adminOnly: true },
    { label: 'SIDEBAR.AUTO_EARN', icon: 'autoearn', href: '/admin/auto-earn', adminOnly: true },
    { label: 'SIDEBAR.TRAIN_AI', icon: 'bot', href: '/admin/knowledge', adminOnly: true },
    { label: 'SIDEBAR.TRANSLATIONS', icon: 'bot', href: '/admin/translations', adminOnly: true },
    { label: 'SIDEBAR.PFM_AI_BOT', icon: 'bot', comingSoon: true },
    { label: 'SIDEBAR.OTHER_SERVICES', icon: 'services', comingSoon: true },
  ];

  protected readonly navItems = computed(() =>
    this.allNavItems.filter((i) => !i.adminOnly || this.isAdmin()),
  );
}

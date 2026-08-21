import { NgTemplateOutlet } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';

export interface NavItem {
  label: string;
  icon: string;
  /** Có route thật chưa? */
  href?: string;
  comingSoon?: boolean;
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

  protected readonly navItems: NavItem[] = [
    { label: 'Dashboard', icon: 'dashboard', href: '/dashboard' },
    { label: 'Chuyển tiền', icon: 'transfer', comingSoon: true },
    { label: 'Tiết kiệm', icon: 'savings', comingSoon: true },
    { label: 'Ủng hộ MTTQ', icon: 'donate', comingSoon: true },
    { label: 'PFM AI Bot', icon: 'bot', comingSoon: true },
    { label: 'Dịch vụ khác', icon: 'services', comingSoon: true },
  ];
}

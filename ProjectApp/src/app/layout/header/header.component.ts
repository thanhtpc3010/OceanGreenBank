import { DecimalPipe } from '@angular/common';
import { Component, computed, EventEmitter, Output, signal } from '@angular/core';
import { Router } from '@angular/router';

import { AvatarModule } from 'primeng/avatar';
import { BadgeModule } from 'primeng/badge';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [DecimalPipe, ButtonModule, AvatarModule, BadgeModule, TooltipModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  @Output() menuToggle = new EventEmitter<void>();

  protected readonly balance = signal(15_000_000);
  protected readonly notificationCount = signal(3);

  protected readonly userName = computed(
    () => this.auth.currentUser()?.fullName ?? 'Người dùng',
  );
  protected readonly userEmail = computed(
    () => this.auth.currentUser()?.email ?? '',
  );

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router,
  ) {}

  protected logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}

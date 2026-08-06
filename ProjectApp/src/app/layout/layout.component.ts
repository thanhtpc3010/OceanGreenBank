import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { FloatingAiComponent } from './floating-ai/floating-ai.component';
import { HeaderComponent } from './header/header.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { SystemBannerComponent } from './system-banner/system-banner.component';

@Component({
  selector: 'app-layout',
  imports: [
    RouterOutlet,
    HeaderComponent,
    SidebarComponent,
    SystemBannerComponent,
    FloatingAiComponent,
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.scss',
})
export class LayoutComponent {
  /** Trạng thái sidebar trên mobile. */
  protected readonly sidebarOpen = signal(false);

  protected toggleSidebar(): void {
    this.sidebarOpen.update((v) => !v);
  }
}

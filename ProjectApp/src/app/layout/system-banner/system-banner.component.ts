import { Component } from '@angular/core';

import { TagModule } from 'primeng/tag';

import { TranslatePipe } from '../../core/i18n/translate.pipe';

@Component({
  selector: 'app-system-banner',
  imports: [TagModule, TranslatePipe],
  templateUrl: './system-banner.component.html',
  styleUrl: './system-banner.component.scss',
})
export class SystemBannerComponent {}
